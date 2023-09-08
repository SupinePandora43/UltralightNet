using System.Runtime.InteropServices;
using UltralightNet.Platform.HighPerformance;

namespace UltralightNet.Platform
{
	namespace HighPerformance
	{
		/// <summary>
		/// <see cref="IClipboard" /> native definition.
		/// </summary>
		public unsafe struct ULClipboard
		{
#if !NETSTANDARD
			public delegate* unmanaged[Cdecl]<void> Clear;
			public delegate* unmanaged[Cdecl]<ULString*, void> ReadPlainText;
			public delegate* unmanaged[Cdecl]<ULString*, void> WritePlainText;
#else
			public void* Clear, ReadPlainText, WritePlainText;
#endif
		}
	}
	public interface IClipboard
	{
		void Clear();
		string ReadPlainText();
		void WritePlainText(string text);

#if !NETSTANDARD2_0
		virtual ULClipboard? GetNativeStruct() => null;
#else
		ULClipboard? GetNativeStruct();
#endif

		internal sealed unsafe class Wrapper : IDisposable
		{
			readonly IClipboard instance;
			readonly ULClipboard _NativeStruct;
			public ULClipboard NativeStruct
			{
				get
				{
					if (IsDisposed) throw new ObjectDisposedException(nameof(Wrapper));
					return _NativeStruct;
				}
				private init => _NativeStruct = value;
			}
			readonly GCHandle[]? handles;
			public bool IsDisposed { get; private set; }

			public Wrapper(IClipboard instance)
			{
				this.instance = instance;
				var nativeStruct = instance.GetNativeStruct();
				if (nativeStruct is not null)
				{
					NativeStruct = nativeStruct.Value;
					return;
				}

				handles = new GCHandle[3];

				NativeStruct = new()
				{
					Clear = (delegate* unmanaged[Cdecl]<void>)Helper.AllocateDelegate(instance.Clear, out handles[0]),
					ReadPlainText = (delegate* unmanaged[Cdecl]<ULString*, void>)Helper.AllocateDelegate(() => new ULString(instance.ReadPlainText().AsSpan()).Allocate(), out handles[1]),
					WritePlainText = (delegate* unmanaged[Cdecl]<ULString*, void>)Helper.AllocateDelegate((ULString* text) => instance.WritePlainText(text->ToString()), out handles[2])
				};
			}

			public void Dispose()
			{
				if (IsDisposed) return;
				if (handles is not null)
				{
					foreach (var handle in handles) if (handle.IsAllocated) handle.Free();
				}

				GC.SuppressFinalize(this);
				IsDisposed = true;
			}
			~Wrapper() => Dispose();
		}
	}
}
