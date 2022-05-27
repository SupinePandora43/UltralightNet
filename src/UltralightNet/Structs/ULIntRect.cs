using System;
#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

namespace UltralightNet;

public struct ULIntRect : IEquatable<ULIntRect>
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;

	public readonly bool IsEmpty => (Left == Right) || (Top == Bottom);

#if NETCOREAPP3_0_OR_GREATER
	public readonly bool Equals(ULIntRect other) => Vector128.Create(Left, Top, Right, Bottom).Equals(Vector128.Create(other.Left, other.Top, other.Right, other.Bottom));
#else
		public readonly bool Equals(ULIntRect other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
#endif
	public readonly override bool Equals(object? other) => other is ULIntRect rect ? Equals(rect) : false;
	public static bool operator ==(ULIntRect? left, ULIntRect? right) => left is not null ? (right is not null ? left.Equals(right) : false) : right is null;
	public static bool operator !=(ULIntRect? left, ULIntRect? right) => !(left == right);

#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
	public readonly override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
#else
		public readonly override int GetHashCode() => base.GetHashCode();
#endif

	public static explicit operator ULIntRect(ULRect rect)
#if NET7_0_OR_GREATER
	{
		Vector128<float> float4 = Vector128.Create(rect.Left, rect.Top, rect.Right, rect.Bottom);
		Vector128<int> int4 = Vector128.ConvertToInt32(float4); // thx Tanner Gooding and TrumpMcDonaldz
		return System.Runtime.CompilerServices.Unsafe.As<Vector128<int>, ULIntRect>(ref int4); // thx rickbrew
	}
#else
	=> new() { Left = (int)rect.Left, Top = (int)rect.Top, Right = (int)rect.Right, Bottom = (int)rect.Bottom };
#endif
}
