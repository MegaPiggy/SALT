using UnityEngine;
using System;

namespace SAL.Extensions
{
    public static class MathExtensions
    {
        #region Time
        public static bool HasReached(this double currentWorldTime, double targetWorldTime) => currentWorldTime >= targetWorldTime;
        #endregion

        public static float Abs(this float value) => (double)value < 0.0 ? -value : value;

        public static bool Approx(this float v1, float v2)
        {
            float num = v1 - v2;
            return (double)num >= -1.0000000116861E-07 && (double)num <= 1.0000000116861E-07;
        }

        public static bool Approx(this Vector2 v1, Vector2 v2) => v1.x.Approx(v2.x) && v1.y.Approx(v2.y);

        public static bool IsNotZero(this float value) => (double)value < -1.0000000116861E-07 || (double)value > 1.0000000116861E-07;

        public static bool IsZero(this float value) => (double)value >= -1.0000000116861E-07 && (double)value <= 1.0000000116861E-07;

        public static bool AbsoluteIsOverThreshold(this float value, float threshold) => (double)value < -(double)threshold || (double)value > (double)threshold;

        public static float NormalizeAngle(this float angle)
        {
            while ((double)angle < 0.0)
                angle += 360f;
            while ((double)angle > 360.0)
                angle -= 360f;
            return angle;
        }

        public static float ToAngle(this Vector2 vector) => vector.x.IsZero() && vector.y.IsZero() ? 0.0f : NormalizeAngle(Mathf.Atan2(vector.x, vector.y) * 57.29578f);

        public static float Min(this float v0, float v1) => (double)v0 >= (double)v1 ? v1 : v0;

        public static float Max(this float v0, float v1) => (double)v0 <= (double)v1 ? v1 : v0;

        public static float Min(this float v0, float v1, float v2, float v3)
        {
            float num1 = (double)v0 < (double)v1 ? v0 : v1;
            float num2 = (double)v2 < (double)v3 ? v2 : v3;
            return (double)num1 >= (double)num2 ? num2 : num1;
        }

        public static float Max(this float v0, float v1, float v2, float v3)
        {
            float num1 = (double)v0 > (double)v1 ? v0 : v1;
            float num2 = (double)v2 > (double)v3 ? v2 : v3;
            return (double)num1 <= (double)num2 ? num2 : num1;
        }

        internal static float ValueFromSides(this float negativeSide, float positiveSide)
        {
            float v1 = negativeSide.Abs();
            float v2 = positiveSide.Abs();
            if (v1.Approx(v2))
                return 0.0f;
            return (double)v1 > (double)v2 ? -v1 : v2;
        }

        internal static float ValueFromSides(this float negativeSide, float positiveSide, bool invertSides) => invertSides ? positiveSide.ValueFromSides(negativeSide) : negativeSide.ValueFromSides(positiveSide);

        public static int NextPowerOfTwo(this int value)
        {
            if (value <= 0)
                return 0;
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            ++value;
            return value;
        }

        public static float Pow(this float f, float p) => Mathf.Pow(f, p);

        /// <summary>
        /// Stop value from going above max or below min values.
        /// </summary>
        public static float Clamp(this float val, float min, float max)
        {
            return Mathf.Clamp(val, min, max);
        }

        /// <summary>
        /// Swap two reference values
        /// </summary>
        public static void Swap<T>(ref T a, ref T b)
        {
            T x = a;
            a = b;
            b = x;
        }

        /// <summary>
        /// Snap to grid of "round" size
        /// </summary>
        public static float Snap(this float val, float round)
        {
            return round * Mathf.Round(val / round);
        }

        /// <summary>
        /// Returns the sign 1/-1 evaluated at the given value.
        /// </summary>
        public static int Sign(IComparable x) => x.CompareTo(0);

        /// <summary>
        /// Shortcut for Mathf.Approximately
        /// </summary>
        public static bool Approximately(this float value, float compare)
        {
            return Mathf.Approximately(value, compare);
        }

        /// <summary>
        /// Value is in [0, 1) range.
        /// </summary>
        public static bool InRange01(this float value)
        {
            return InRange(value, 0, 1);
        }

        /// <summary>
        /// Value is in [closedLeft, openRight) range.
        /// </summary>
        public static bool InRange<T>(this T value, T closedLeft, T openRight)
            where T : IComparable =>
            value.CompareTo(closedLeft) >= 0 && value.CompareTo(openRight) < 0;

        ///// <summary>
        ///// Value is in a RangedFloat.
        ///// </summary>
        //public static bool InRange(this float value, RangedFloat range) =>
        //    value.InRange(range.Min, range.Max);

        ///// <summary>
        ///// Value is in a RangedInt.
        ///// </summary>
        //public static bool InRange(this int value, RangedInt range) =>
        //    value.InRange(range.Min, range.Max);

        /// <summary>
        /// Value is in [closedLeft, closedRight] range, max-inclusive.
        /// </summary>
        public static bool InRangeInclusive<T>(this T value, T closedLeft, T closedRight)
            where T : IComparable =>
            value.CompareTo(closedLeft) >= 0 && value.CompareTo(closedRight) <= 0;

        ///// <summary>
        ///// Value is in a RangedFloat, max-inclusive.
        ///// </summary>
        //public static bool InRangeInclusive(this float value, RangedFloat range) =>
        //    value.InRangeInclusive(range.Min, range.Max);

        ///// <summary>
        ///// Value is in a RangedInt, max-inclusive.
        ///// </summary>
        //public static bool InRangeInclusive(this int value, RangedInt range) =>
        //    value.InRangeInclusive(range.Min, range.Max);

        /// <summary>
        /// Clamp value to less than min or more than max
        /// </summary>
        public static float NotInRange(this float num, float min, float max)
        {
            if (min > max)
            {
                var x = min;
                min = max;
                max = x;
            }

            if (num < min || num > max) return num;

            float mid = (max - min) / 2;

            if (num > min) return num + mid < max ? min : max;
            return num - mid > min ? max : min;
        }

        /// <summary>
        /// Clamp value to less than min or more than max
        /// </summary>
        public static int NotInRange(this int num, int min, int max)
        {
            return (int)((float)num).NotInRange(min, max);
        }

        /// <summary>
        /// Return point A or B, closest to num
        /// </summary>
        public static float ClosestPoint(this float num, float pointA, float pointB)
        {
            if (pointA > pointB)
            {
                var x = pointA;
                pointA = pointB;
                pointB = x;
            }

            float middle = (pointB - pointA) / 2;
            float withOffset = num.NotInRange(pointA, pointB) + middle;
            return (withOffset >= pointB) ? pointB : pointA;
        }

        /// <summary>
        /// Check if pointA closer to num than pointB
        /// </summary>
        public static bool ClosestPointIsA(this float num, float pointA, float pointB)
        {
            var closestPoint = num.ClosestPoint(pointA, pointB);
            return Mathf.Approximately(closestPoint, pointA);
        }
    }
}
