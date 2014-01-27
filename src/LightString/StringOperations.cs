using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightString
{
    /// <summary>
    /// Represents a set of string operations (extensions) that 
    /// operate on a reference of a string thus teating it like an 
    /// mutable data structure.
    /// </summary>
    public static class StringOperations
    {
        public const int WhiteSpace = 32;
        public const char PointerEnd = '\0';

        /// <summary>
        /// Returns a reference of this System.String converted to uppercase, using the casing
        //  rules of the current culture.
        /// </summary>
        /// <param name="str">this string.</param>
        /// <returns></returns>
        public static string ToUpperInPlace(this string str)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    for (char* p = c; *p != 0; p++)
                    {
                        *p = char.ToUpper(*p);
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the current System.String object,
        /// using pointer arthmetic and returns a copy of the string.
        /// </summary>
        /// <param name="str">this string.</param>
        /// <returns></returns>
        public static string FastTrim(this string str)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* start;
                    char* end;
                    uint cnt = 0;

                    start = c;

                    for (; *start != 0; start++)
                    {
                        if (*start != WhiteSpace) break;
                        cnt++;
                    }

                    //End
                    if (*start != 0)
                    {
                        end = start + (str.Length - 1 - cnt);
                        for (; end > c; end--)
                        {
                            if (*end != WhiteSpace) break;                           
                        }

                        *(end + 1) = PointerEnd;
                    }

                    return new string(start);
                }
            }
        }
    }
}
