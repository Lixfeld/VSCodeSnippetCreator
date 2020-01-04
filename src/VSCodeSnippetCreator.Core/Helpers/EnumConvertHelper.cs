using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace VSCodeSnippetCreator.Core.Helpers
{
    public static class EnumConvertHelper
    {
        public static string ConvertEnumDescriptionToString<T>(T enumValue) where T : Enum
        {
            IEnumerable<Enum> languageCollection = Enum.GetValues(typeof(T)).Cast<Enum>();
            return languageCollection.FirstOrDefault(e => e.ToString() == enumValue.ToString())?.GetDescription();
        }

        public static T ConvertStringToEnumValue<T>(object stringObj) where T : struct, IConvertible
        {
            string enumString = stringObj?.ToString();
            if (Enum.TryParse(enumString, out T enumValue))
            {
                return enumValue;
            }

            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute == null)
                {
                    continue;
                }
                else if (attribute.Description == enumString)
                {
                    Enum.TryParse(field.GetValue(null).ToString(), out enumValue);
                    return enumValue;
                }
            }
            return enumValue;
        }

        public static IEnumerable<string> GetEnumDescriptions<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<Enum>().Select(e => e.GetDescription());
        }

        private static string GetDescription(this Enum value)
        {
            string description = value
                .GetType()
                .GetField(value.ToString())
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;

            return description ?? value.ToString();
        }
    }
}

