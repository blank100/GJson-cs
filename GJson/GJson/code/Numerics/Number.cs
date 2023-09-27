using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gal.Core
{
	/// <summary>
	/// 
	/// </summary>
	/// <para>author gouanlin</para>
	[StructLayout(LayoutKind.Explicit)]
	public struct Number
	{
		public const float FLOAT_TOLERANCE = 1e-6f;
		public const double DOUBLE_TOLERANCE = 1E-015D;

		[FieldOffset(0)]
		private bool m_IsDouble;

		[FieldOffset(1)]
		private long m_Long;

		[FieldOffset(1)]
		private double m_Double;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => m_IsDouble ? m_Double.ToString(CultureInfo.InvariantCulture) : m_Long.ToString();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Number(long value) => new() { m_IsDouble = false, m_Long = value };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Number(double value) => new() { m_IsDouble = true, m_Double = value };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator long(Number value) => value.m_IsDouble ? (long)value.m_Double : value.m_Long;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double(Number value) => value.m_IsDouble ? value.m_Double : value.m_Long;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equals(float a, float b) => (a > b ? a - b : b - a) < FLOAT_TOLERANCE;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equals(double a, double b) => (a > b ? a - b : b - a) < DOUBLE_TOLERANCE;
	}
}