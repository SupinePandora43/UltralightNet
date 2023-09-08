using System.Runtime.InteropServices;
using UltralightNet.Platform.HighPerformance;

namespace UltralightNet.Platform
{
	namespace HighPerformance
	{
		/// <summary>
		/// <see cref="ILogger" /> native definition.
		/// </summary>
		public unsafe struct ULLogger
		{
#if !NETSTANDARD
			public delegate* unmanaged[Cdecl]<ULLogLevel, ULString*, void> LogMessage;
#else
			public void* LogMessage;
#endif
		}
	}
	public interface ILogger
	{
		void LogMessage(ULLogLevel logLevel, string message);

#if !NETSTANDARD2_0
		virtual ULLogger? GetNativeStruct() => null;
#else
		ULLogger? GetNativeStruct();
#endif
		internal sealed unsafe class Wrapper
		{
			readonly ILogger instance;
			readonly ULLogger _NativeStruct;
			public ULLogger NativeStruct
			{
				get
				{
					if (IsDisposed) throw new ObjectDisposedException(nameof(Wrapper));
					return _NativeStruct;
				}
				private init => _NativeStruct = value;
			}
			readonly GCHandle handle;
			public bool IsDisposed { get; private set; }

			public Wrapper(ILogger instance)
			{
				this.instance = instance;
				var nativeStruct = instance.GetNativeStruct();
				if (nativeStruct is not null)
				{
					NativeStruct = nativeStruct.Value;
					return;
				}

				NativeStruct = new() { LogMessage = (delegate* unmanaged[Cdecl]<ULLogLevel, ULString*, void>)Helper.AllocateDelegate((ULLogLevel logLevel, ULString* message) => instance.LogMessage(logLevel, message->ToString()), out handle) };
			}

			public void Dispose()
			{
				if (IsDisposed) return;
				if (handle.IsAllocated) handle.Free();

				GC.SuppressFinalize(this);
				IsDisposed = true;
			}
			~Wrapper() => Dispose();
		}
	}
}
