using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace QAHelper.WPF
{
    public class EnumToItemsSourceExtension : MarkupExtension
    {
        private Type Type { get; }

        public EnumToItemsSourceExtension(Type type)
        {
            Type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if ((Type != null) && Type.IsEnum)
            {
                return Enum.GetValues(Type)
                    .OfType<object>()
                    .Select(e => new { Value = e, DisplayName = GetEnumValueDescription(e) });
            }
            return new object[0];
        }

        private string GetEnumValueDescription(object enumValue)
        {
            Type enumType = enumValue.GetType();
            string enumFieldName = enumValue.ToString();

            string description = enumType
                .GetField(enumFieldName)
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault()
                ?.Description;

            return string.IsNullOrWhiteSpace(description) ? enumFieldName : description;
        }
    }
}