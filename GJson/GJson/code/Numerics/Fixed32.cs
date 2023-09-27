using System;
using System.Runtime.CompilerServices;

namespace Gal.Core
{
	/// <summary>
	/// 64位定点数
	/// </summary>
	/// <para>author gouanlin</para>
	public readonly struct Fixed32 : IEquatable<Fixed32>, IFormattable
	{
		private static int m_FractionBits = 8;

		public static int fractionBits {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => m_FractionBits;
			set {
				m_FractionBits = value;
				m_One = 1 << fractionBits;
				m_Fraction = 1.0f / m_One;
				m_MaxInteger = int.MaxValue >> m_FractionBits;
				m_MinInteger = int.MinValue >> m_FractionBits;
			}
		}

		private static int m_One = 1 << fractionBits;

		private static float m_Fraction = 1.0f / m_One;

		public static float precision {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => m_Fraction;
		}

		private static int m_MaxInteger = int.MaxValue >> m_FractionBits;
		private static int m_MinInteger = int.MinValue >> m_FractionBits;
		public static int maxInteger => m_MaxInteger;
		public static int minInteger => m_MinInteger;

		private readonly int m_RawValue;

		private Fixed32(int v) => m_RawValue = v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed32(float v) => new((int)(v * m_One));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float(Fixed32 v) => (float)v.m_RawValue / m_One;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed32(int v) => new(v << m_FractionBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int(Fixed32 v) => v.m_RawValue >> m_FractionBits;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed32(long v) => new((int)v << m_FractionBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator long(Fixed32 v) => v.m_RawValue >> m_FractionBits;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed32(double v) => new((int)(v * m_One));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double(Fixed32 v) => (double)v.m_RawValue / m_One;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed32(decimal v) => new((int)(v * m_One));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator decimal(Fixed32 v) => (decimal)v.m_RawValue / m_One;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator +(Fixed32 a) => new(a.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator -(Fixed32 a) => new(-a.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator +(Fixed32 a, Fixed32 b) => new(a.m_RawValue + b.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator -(Fixed32 a, Fixed32 b) => new(a.m_RawValue - b.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator *(Fixed32 a, Fixed32 b) => new((int)((long)a.m_RawValue * b.m_RawValue >> m_FractionBits));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator /(Fixed32 a, Fixed32 b) => new((int)((double)((long)a.m_RawValue << m_FractionBits) / b.m_RawValue));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator %(Fixed32 a, Fixed32 b) => new(a.m_RawValue % b.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator <<(Fixed32 a, int c) => new(a.m_RawValue << c);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed32 operator >>(Fixed32 a, int c) => new(a.m_RawValue >> c);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Fixed32 a, Fixed32 b) => a.m_RawValue == b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Fixed32 a, Fixed32 b) => !(a == b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(Fixed32 a, Fixed32 b) => a.m_RawValue > b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(Fixed32 a, Fixed32 b) => a.m_RawValue < b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(Fixed32 a, Fixed32 b) => a.m_RawValue >= b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(Fixed32 a, Fixed32 b) => a.m_RawValue <= b.m_RawValue;

		public bool Equals(Fixed32 other) => m_RawValue == other.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => m_RawValue.GetHashCode();

		public override bool Equals(object obj) {
			return obj is Fixed32 other && Equals(other);
		}

		public string ToString(string format, IFormatProvider formatProvider) => ((decimal)this).ToString(format, formatProvider);
	}
}