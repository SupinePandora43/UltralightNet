using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public ref struct ULCommand
	{
		private byte _CommandType;
		public ULCommandType CommandType { get => Unsafe.As<byte, ULCommandType>(ref _CommandType); set => _CommandType = Unsafe.As<ULCommandType, byte>(ref value); }
		public ULGPUState GPUState;

		/// <remarks>Only used when <see cref="command_type"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
		public uint GeometryId;
		/// <remarks>Only used when <see cref="command_type"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
		public uint IndicesCount;
		/// <remarks>Only used when <see cref="command_type"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
		public uint IndicesOffset;
	}
}
