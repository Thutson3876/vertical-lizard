using System;
using System.Diagnostics;
using UnityEngine;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SaintsArrowAttribute: OneDirectionBaseAttribute
    {
        public readonly float HeadLength;
        public readonly float HeadAngle;

        public SaintsArrowAttribute(
            string start = null, int startIndex = 0, Space startSpace = Space.World,
            string end = null, int endIndex = 0, Space endSpace = Space.World,
            EColor color = EColor.White, float colorAlpha = 1f,
            float headLength = 0.5f,
            float headAngle = 20.0f
        ): base(start, startIndex, startSpace, end, endIndex, endSpace, color, colorAlpha)
        {
            HeadLength = headLength;
            HeadAngle = headAngle;
        }
    }
}
