using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace SewoTranslator.Converter
{
    public sealed class TranslationsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> translations = value as List<string>;

            if (translations == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder(translations[0]);
            for (int i = 1; i < translations.Count; i++)
            {
                sb.AppendFormat(", {0}", translations[i]);
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This action is not supported.");
        }
    }
}
