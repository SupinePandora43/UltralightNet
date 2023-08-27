using System.Runtime.CompilerServices;

namespace UltralightNet;

public struct ULCommand : IEquatable<ULCommand>
{
	private byte _CommandType;
	public ULCommandType CommandType { readonly get => Unsafe.As<byte, ULCommandType>(ref Unsafe.AsRef(_CommandType)); set => _CommandType = Unsafe.As<ULCommandType, byte>(ref value); }
	public ULGPUState GPUState;

	/// <remarks>Only used when <see cref="CommandType"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
	public uint GeometryId;
	/// <remarks>Only used when <see cref="CommandType"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
	public uint IndicesCount;
	/// <remarks>Only used when <see cref="CommandType"/> is <see cref="ULCommandType.DrawGeometry"/></remarks>
	public uint IndicesOffset;

	public readonly bool Equals(ULCommand other) => CommandType == other.CommandType && GPUState.Equals(other.GPUState) && GeometryId == other.GeometryId && IndicesCount == other.IndicesCount && IndicesOffset == other.IndicesOffset;
}
