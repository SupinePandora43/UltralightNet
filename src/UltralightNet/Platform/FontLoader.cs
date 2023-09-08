using System.Runtime.InteropServices;
using UltralightNet.Platform.HighPerformance;

namespace UltralightNet.Platform
{
	namespace HighPerformance
	{
		/// <summary>
		/// <see cref="IFontLoader" /> native definition.
		/// </summary>
		public unsafe struct ULFontLoader
		{
#if !NETSTANDARD
			public delegate* unmanaged[Cdecl]<ULString*> GetFallbackFont;
			public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULString*> GetFallbackFontForCharacters;
			public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULFontFile> Load;
#else
			public void* GetFallbackFont, GetFallbackFontForCharacters, Load;
#endif
		}
	}
	public interface IFontLoader
	{
		string GetFallbackFont();
		string GetFallbackFontForCharacters(string text, int weight, bool italic);
		ULFontFile Load(string font, int weight, bool italic);

#if !NETSTANDARD2_0
		virtual ULFontLoader? GetNativeStruct() => null;
#else
		ULFontLoader? GetNativeStruct();
#endif

		internal sealed unsafe class Wrapper : IDisposable
		{
			readonly IFontLoader instance;
			readonly ULFontLoader _NativeStruct;
			public ULFontLoader NativeStruct
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

			public Wrapper(IFontLoader instance)
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
					GetFallbackFont = (delegate* unmanaged[Cdecl]<ULString*>)Helper.AllocateDelegate(() => new ULString(instance.GetFallbackFont().AsSpan()).Allocate(), out handles[0]),
					GetFallbackFontForCharacters = (delegate* unmanaged[Cdecl]<ULString*, int, bool, ULString*>)Helper.AllocateDelegate((ULString* text, int weight, bool italic) => new ULString(instance.GetFallbackFontForCharacters(text->ToString(), weight, italic).AsSpan()).Allocate(), out handles[1]),
					Load = (delegate* unmanaged[Cdecl]<ULString*, int, bool, ULFontFile>)Helper.AllocateDelegate((ULString* font, int weight, bool italic) => instance.Load(font->ToString(), weight, italic), out handles[2])
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
