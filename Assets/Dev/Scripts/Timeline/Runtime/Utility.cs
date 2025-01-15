using System;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Serialization.TypeResolvers;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    [Serializable]
    public static class Utility
    {
        public static double GetSecondPerFrame(Sequence sequence) => GetSecondPerFrame(sequence?.frameRateType ?? EFrameRate.Game);
        public static double GetSecondPerFrame(EFrameRate eFrameRate)
        {
            if (eFrameRate == EFrameRate.Film)
                return SPF_Film;
            else if (eFrameRate == EFrameRate.HD)
                return SPF_HD;
            else if (eFrameRate == EFrameRate.Game)
                return SPF_Gane;
            return SPF_Default;
        }

        public static class Time
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

        //
        // 摘要:
        //     Determines whether a type inherits or implements another type. Also include support
        //     for open generic base types such as List<>.
        //
        // 参数:
        //   type:
        public static bool InheritsFrom<TBase>(this Type type)
        {
            return type.InheritsFrom(typeof(TBase));
        }

        //
        // 摘要:
        //     Determines whether a type inherits or implements another type. Also include support
        //     for open generic base types such as List<>.
        //
        // 参数:
        //   type:
        //
        //   baseType:
        public static bool InheritsFrom(this Type type, Type baseType)
        {
            if (baseType.IsAssignableFrom(type))
            {
                return true;
            }

            if (type.IsInterface && !baseType.IsInterface)
            {
                return false;
            }

            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            Type type2 = type;
            while (type2 != null)
            {
                if (type2 == baseType)
                {
                    return true;
                }

                if (baseType.IsGenericTypeDefinition && type2.IsGenericType && type2.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }

                type2 = type2.BaseType;
            }

            return false;
        }

        public static Type GetGenericType(this Type type, Type baseType)
        {
            Type type2 = type.BaseType;
            while (type2 != null)
            {
                if (baseType.IsGenericTypeDefinition && type2.IsGenericType && type2.GetGenericTypeDefinition() == baseType)
                {
                    var args = type2.GetGenericArguments(); 
                    if (args == null || args.Length <= 0)
                        return null;
                    return type2.GetGenericArguments()[0];
                }

                type2 = type2.BaseType;
            }
            return null;
        }
    }
}