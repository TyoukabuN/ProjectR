using System;
using System.Linq;
using static PJR.Timeline.Define;

namespace PJR.Timeline
{
    [Serializable]
    public static class Utility
    {
        public static double GetSecondPerFrame(Sequence sequence) => GetSecondPerFrame(sequence?.FrameRateType ?? EFrameRate.Game);
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

        public static bool IsValid(this Sequence sequence)
        {
            if(sequence == null)
                return false;
            var tracks = sequence.Tracks;
            if (tracks == null || tracks.Length <= 0)
                return false;
            return true;
        }

        public static bool IsValid(this Track track)
        {
            return true;
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