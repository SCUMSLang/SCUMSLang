using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SCUMSLang
{
    public static class SCUMSLangTools
    {
        public static void ForEachEnum<T>(Action<T> callback)
            where T : Enum
        {
            var enumType = typeof(T);
            var tokenTypeValues = Enum.GetValues(enumType);
            var index = tokenTypeValues.Length;

            while (--index >= 0) {
                var enumValue = (T)tokenTypeValues.GetValue(index)!;
                callback(enumValue);
            }
        }

        public static MemberInfo GetEnumField<T>(T enumValue)
            where T : Enum
        {
            var enumType = typeof(T);
            var tokenTypeName = Enum.GetName(enumType, enumValue)!;
            var memberInfo = enumType.GetField(tokenTypeName)!;
            return memberInfo;
        }

        public static bool TryGetAttribute<T>(MemberInfo memberInfo, [MaybeNullWhen(false)] out T attribute)
            where T : Attribute
        {
            var attributeType = typeof(T);
            attribute = Attribute.GetCustomAttribute(memberInfo, attributeType) as T;

            if (!(attribute is null)) {
                return true;
            }

            attribute = null!;
            return false;
        }
    }
}
