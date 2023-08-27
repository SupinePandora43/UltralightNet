#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

namespace UltralightNet;

public struct ULRect : IEquatable<ULRect>
{
	public float Left;
	public float Top;
	public float Right;
	public float Bottom;

	public readonly bool IsEmpty => (Left == Right) || (Top == Bottom);

	public readonly bool Equals(ULRect other) =>
#if NETCOREAPP3_0_OR_GREATER
		Vector128.Create(Left, Top, Right, Bottom).Equals(Vector128.Create(other.Left, other.Top, other.Right, other.Bottom));
#else
		Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
#endif
	public readonly override bool Equals(object? other) => other is ULRect rect && Equals(rect);
	public static bool operator ==(ULRect? left, ULRect? right) => left is not null ? (right is not null && left.Equals(right)) : right is null;
	public static bool operator !=(ULRect? left, ULRect? right) => !(left == right);

#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
	public readonly override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
#endif

	public static explicit operator ULRect(ULIntRect rect)
#if NET7_0_OR_GREATER
	{
		Vector128<int> int4 = Vector128.Create(rect.Left, rect.Top, rect.Right, rect.Bottom);
		Vector128<float> float4 = Vector128.ConvertToSingle(int4); // thx Tanner Gooding and TrumpMcDonaldz
		return System.Runtime.CompilerServices.Unsafe.As<Vector128<float>, ULRect>(ref float4); // thx rickbrew
	}
#else
	=> new() { Left = (float)rect.Left, Top = (float)rect.Top, Right = (float)rect.Right, Bottom = (float)rect.Bottom };
#endif
}
