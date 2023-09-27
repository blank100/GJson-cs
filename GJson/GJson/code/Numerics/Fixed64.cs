using System;
using System.Runtime.CompilerServices;

namespace Gal.Core
{
	/// <summary>
	/// 64位定点数
	/// </summary>
	/// <para>author gouanlin</para>
	public readonly struct Fixed64 : IEquatable<Fixed64>, IFormattable
	{
		private static int m_FractionBits = 16;

		public static int fractionBits {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => m_FractionBits;
			set {
				m_FractionBits = value;
				m_One = 1L << fractionBits;
				m_Fraction = 1.0f / m_One;
				m_MulFastOutMask = long.MaxValue << 32;
				m_DivFastOutMask = long.MaxValue << (64 - m_FractionBits);
				m_MaxInteger = long.MaxValue >> m_FractionBits;
				m_MaxFastInteger = long.MaxValue >> (m_FractionBits + m_FractionBits);
				m_MinInteger = long.MinValue >> m_FractionBits;
				m_MinFastInteger = long.MinValue >> (m_FractionBits + m_FractionBits);
			}
		}

		private static long m_One = 1L << fractionBits;

		private static float m_Fraction = 1.0f / m_One;

		public static float precision {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => m_Fraction;
		}

		private static long m_MulFastOutMask = long.MaxValue << 32;
		private static long m_DivFastOutMask = long.MaxValue << (64 - m_FractionBits);

		private static long m_MaxInteger = long.MaxValue >> m_FractionBits;
		private static long m_MaxFastInteger = long.MaxValue >> (m_FractionBits + m_FractionBits);
		private static long m_MinInteger = long.MinValue >> m_FractionBits;
		private static long m_MinFastInteger = long.MinValue >> (m_FractionBits + m_FractionBits);
		public static long maxInteger => m_MaxInteger;
		public static long MaxFastInteger => m_MaxFastInteger;
		public static long minInteger => m_MinInteger;
		public static long MinFastInteger => m_MinFastInteger;

		private readonly long m_RawValue;

		private Fixed64(long v) => m_RawValue = v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed64(float v) => new((long)((double)v * m_One));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float(Fixed64 v) => (float)((double)v.m_RawValue / m_One);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed64(int v) => new((long)v << m_FractionBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int(Fixed64 v) => (int)(v.m_RawValue >> m_FractionBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed64(long v) => new(v << m_FractionBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator long(Fixed64 v) => v.m_RawValue >> m_FractionBits;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed64(double v) => new((long)(v * m_One));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator double(Fixed64 v) => (double)v.m_RawValue / m_One;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Fixed64(decimal v) => new((long)(v * m_One));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator decimal(Fixed64 v) => (decimal)v.m_RawValue / m_One;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator +(Fixed64 a) => new(a.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator -(Fixed64 a) => new(-a.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator +(Fixed64 a, Fixed64 b) => new(a.m_RawValue + b.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator -(Fixed64 a, Fixed64 b) => new(a.m_RawValue - b.m_RawValue);

		/// <summary>
		/// 此方法时不安全的,当 a 和 b 的 raw 值相乘大于 long.MaxValue 时,会出现数据丢失,可用 SafeMul 进行计算
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator *(Fixed64 a, Fixed64 b) => new((a.m_RawValue * b.m_RawValue) >> m_FractionBits);

		/// <summary>
		/// 此方法时不安全的,当 a 的 raw 大于 long.MaxValue >> fractionBits 时,会出现数据丢失,可改用 SafeDiv 进行计算
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator /(Fixed64 a, Fixed64 b) => new((long)((double)(a.m_RawValue << m_FractionBits) / b.m_RawValue));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator %(Fixed64 a, Fixed64 b) => new(a.m_RawValue % b.m_RawValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator <<(Fixed64 a, int c) => new(a.m_RawValue << c);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 operator >>(Fixed64 a, int c) => new(a.m_RawValue >> c);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Fixed64 a, Fixed64 b) => a.m_RawValue == b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Fixed64 a, Fixed64 b) => !(a == b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(Fixed64 a, Fixed64 b) => a.m_RawValue > b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(Fixed64 a, Fixed64 b) => a.m_RawValue < b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(Fixed64 a, Fixed64 b) => a.m_RawValue >= b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(Fixed64 a, Fixed64 b) => a.m_RawValue <= b.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 SafeMul(Fixed64 a, Fixed64 b) => (a.m_RawValue & m_MulFastOutMask & b.m_RawValue) > 0
			? (Fixed64)new((long)((decimal)a.m_RawValue * b.m_RawValue / m_One))
			: new((a.m_RawValue * b.m_RawValue) >> m_FractionBits);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64 SafeDiv(Fixed64 a, Fixed64 b) => (a.m_RawValue & m_DivFastOutMask) > 0
			? (Fixed64)new((long)((decimal)a.m_RawValue * m_One / b.m_RawValue))
			: new((long)((double)(a.m_RawValue << m_FractionBits) / b.m_RawValue));

		public bool Equals(Fixed64 other) => m_RawValue == other.m_RawValue;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => m_RawValue.GetHashCode();

		public override bool Equals(object obj) {
			return obj is Fixed64 other && Equals(other);
		}

		public string ToString(string format, IFormatProvider formatProvider) => ((decimal)this).ToString(format, formatProvider);
	}
}