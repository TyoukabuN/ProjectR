using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    public static class TimeUtil
    {
        public const double kTimeEpsilon = 1e-14;
        public const double kFrameRateEpsilon = 1e-6;
        public const double k_MaxTimelineDurationInSeconds = 9e6; //104 days of running time
        public const double kFrameRateRounding = 1e-2;
        static void ValidateFrameRate(double frameRate)
        {
            if (frameRate <= kTimeEpsilon)
                throw new ArgumentException("frame rate cannot be 0 or negative");
        }


        /// <summary>
        /// RoundToFrame
        /// </summary>
        /// <param name="time">Seconds</param>
        /// <param name="frameRate">FPS</param>
        /// <returns></returns>
        public static double RoundToFrame(double time, double frameRate)
        {
            ValidateFrameRate(frameRate);

            var frameBefore = (int)Math.Floor(time * frameRate) / frameRate;
            var frameAfter = (int)Math.Ceiling(time * frameRate) / frameRate;

            return Math.Abs(time - frameBefore) < Math.Abs(time - frameAfter) ? frameBefore : frameAfter;
        }
        public static double RoundToFrame(double time) => RoundToFrame(time, Define.SPF_Default);

        public static double GetEpsilon(double time, double frameRate)
        {
            return Math.Max(Math.Abs(time), 1) * frameRate * kTimeEpsilon;
        }
        public static int ToFrames(double time, double frameRate)
        {
            ValidateFrameRate(frameRate);
            time = Math.Min(Math.Max(time, -k_MaxTimelineDurationInSeconds), k_MaxTimelineDurationInSeconds);
            // this matches OnFrameBoundary
            double tolerance = GetEpsilon(time, frameRate);
            if (time < 0)
            {
                return (int)Math.Ceiling(time * frameRate - tolerance);
            }
            return (int)Math.Floor(time * frameRate + tolerance);
        }

        public static double ToExactFrames(double time, float frameRate)
        {
            ValidateFrameRate(frameRate);
            return time * frameRate;
        }
    }
}