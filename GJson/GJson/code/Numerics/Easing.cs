using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gal.Core
{
	public static class Easing
	{
		private const float HALF_PI = (float)(Math.PI * 0.5f);
		private const float TWO_PI = (float)(Math.PI * 2f);

		public enum Types
		{
			Linear,
			InSine,
			OutSine,
			InOutSine,
			InQuad,
			OutQuad,
			InOutQuad,
			InCubic,
			OutCubic,
			InOutCubic,
			InQuart,
			OutQuart,
			InOutQuart,
			InQuint,
			OutQuint,
			InOutQuint,
			InExpo,
			OutExpo,
			InOutExpo,
			InCirc,
			OutCirc,
			InOutCirc,
			InElastic,
			OutElastic,
			InOutElastic,
			InBack,
			OutBack,
			InOutBack,
			InBounce,
			OutBounce,
			InOutBounce,
		}

		public static readonly IReadOnlyDictionary<Types, Func<float, float>> FUNCTIONS = new Dictionary<Types, Func<float, float>> {
			[Types.Linear] = Linear,
			[Types.InSine] = InSine,
			[Types.OutSine] = OutSine,
			[Types.InOutSine] = InOutSine,
			[Types.InQuad] = InQuad,
			[Types.OutQuad] = OutQuad,
			[Types.InOutQuad] = InOutQuad,
			[Types.InCubic] = InCubic,
			[Types.OutCubic] = OutCubic,
			[Types.InOutCubic] = InOutCubic,
			[Types.InQuart] = InQuart,
			[Types.OutQuart] = OutQuart,
			[Types.InOutQuart] = InOutQuart,
			[Types.InQuint] = InQuint,
			[Types.OutQuint] = OutQuint,
			[Types.InOutQuint] = InOutQuint,
			[Types.InExpo] = InExpo,
			[Types.OutExpo] = OutExpo,
			[Types.InOutExpo] = InOutExpo,
			[Types.InCirc] = InCirc,
			[Types.OutCirc] = OutCirc,
			[Types.InOutCirc] = InOutCirc,
			[Types.InElastic] = InElastic,
			[Types.OutElastic] = OutElastic,
			[Types.InOutElastic] = InOutElastic,
			[Types.InBack] = InBack,
			[Types.OutBack] = OutBack,
			[Types.InOutBack] = InOutBack,
			[Types.InBounce] = InBounce,
			[Types.OutBounce] = OutBounce,
			[Types.InOutBounce] = InOutBounce,
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Linear(float n) => n;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InSine(float n) => -(float)Math.Cos(n * HALF_PI) + 1f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutSine(float n) => (float)Math.Sin(n * TWO_PI);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutSine(float n) => -0.5f * ((float)Math.Cos(Math.PI * n) - 1f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InQuad(float n) => n * n;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutQuad(float n) => -n * (n - 2f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutQuad(float n) => (n *= 2f) < 1f ? 0.5f * n * n : -0.5f * (--n * (n - 2f) - 1f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InCubic(float n) => n * n * n;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutCubic(float n) => --n * n * n + 1f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutCubic(float n) => (n *= 2f) < 1f ? 0.5f * n * n * n : 0.5f * ((n -= 2f) * n * n + 2f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InQuart(float n) => n * n * n * n;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutQuart(float n) => --n * n * n * n + 1f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutQuart(float n) => (n *= 2f) < 1f ? 0.5f * n * n * n * n : 0.5f * ((n -= 2f) * n * n * n + 2f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InQuint(float n) => n * n * n * n * n;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutQuint(float n) => --n * n * n * n * n + 1f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutQuint(float n) => (n *= 2f) < 1f ? 0.5f * n * n * n * n * n : 0.5f * ((n -= 2f) * n * n * n * n + 2f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InExpo(float n) => 0f == n ? 0f : (float)Math.Pow(1024f, n - 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutExpo(float n) => Number.Equals(n, 1f) ? n : (float)(1f - Math.Pow(1024f, -n));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutExpo(float n) => n switch {
			0f => 0f,
			1f => 1f,
			_ => (n *= 2f) < 1f ? (float)(0.5f * Math.Pow(1024f, n - 1f)) : (float)(0.5f * (-Math.Pow(2f, -10f * (n - 1f)) + 2f))
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InCirc(float n) => (float)(1 - Math.Sqrt(1f - n * n));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutCirc(float n) => (float)Math.Sqrt(1f - --n * n);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutCirc(float n) => (n *= 2f) < 1f
			? (float)(-0.5f * (Math.Sqrt(1f - n * n) - 1f))
			: (float)(0.5f * (Math.Sqrt(1f - (n -= 2f) * n) + 1f));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InBack(float n) => n * n * ((1.70158f + 1f) * n - 1.70158f);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutBack(float n) => --n * n * ((1.70158f + 1f) * n + 1.70158f) + 1f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutBack(float n) {
			const float s = 1.70158f * 1.525f;
			return (n *= 2f) < 1f ? 0.5f * (n * n * ((s + 1f) * n - s)) : 0.5f * ((n -= 2f) * n * ((s + 1f) * n + s) + 2f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InBounce(float n) => 1f - OutBounce(1f - n);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutBounce(float n) {
			return n switch {
				< 1 / 2.75f => 7.5625f * n * n,
				< 2f / 2.75f => 7.5625f * (n -= 1.5f / 2.75f) * n + 0.75f,
				< 2.5f / 2.75f => 7.5625f * (n -= 2.25f / 2.75f) * n + 0.9375f,
				_ => 7.5625f * (n -= 2.625f / 2.75f) * n + 0.984375f
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutBounce(float n) => n < 0.5f ? InBounce(n * 2f) * 0.5f : OutBounce(n * 2f - 1f) * 0.5f + 0.5f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InElastic(float n) {
			if (n == 0f) return 0f;
			if (Number.Equals(n, 1f)) return 1f;
			const float a = 1f, p = 0.4f;
			const float s = p / 4f;
			return (float)-(a * Math.Pow(2f, 10f * (n -= 1f)) * Math.Sin((n - s) * (2f * Math.PI) / p));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float OutElastic(float n) {
			if (n == 0f) return 0;
			if (Number.Equals(n, 1f)) return 1f;
			const float a = 1f, p = 0.4f;
			const float s = p / 4f;
			return (float)(a * Math.Pow(2f, -10f * n) * Math.Sin((n - s) * (2f * Math.PI) / p) + 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InOutElastic(float n) {
			if (n == 0) return 0f;
			if (Number.Equals(n, 1f)) return 1f;
			const float a = 1f, p = 0.4f;
			const float s = p / 4f;
			return (n *= 2f) < 1f
				? (float)(-0.5f * (a * Math.Pow(2f, 10f * (n -= 1f)) * Math.Sin((n - s) * (2f * Math.PI) / p)))
				: (float)(a * Math.Pow(2f, -10f * (n -= 1f)) * Math.Sin((n - s) * (2f * Math.PI) / p) * 0.5f + 1f);
		}
	}
}