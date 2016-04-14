using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ArgumentNotNull
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        public static void ThrowIfNull<T>(this T obj, string parameterName)
                where T : class
        {
            if (obj == null) throw new ArgumentNullException(parameterName);
        }

        public static void ThrowIfNull<T>(this T obj, string message, string parameterName)
               where T : class
        {
            if (obj == null) throw new ArgumentNullException(message, parameterName);
        }
        public static bool IsNull<T>(this T obj)
              where T : class
        {
            return (obj == null);
        }
    }
}
