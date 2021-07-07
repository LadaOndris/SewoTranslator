using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SewoTranslator.Model
{
    public static class ExtensionMethods
    {
        public static string RemoveDiacritics(this string text)
        {
            byte[] tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(text);
            return Encoding.UTF8.GetString(tempBytes);
        }
        
    }
}
