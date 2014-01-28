#region Licence
/*
Copyright (c) 2011-2014 Contributors as noted in the AUTHORS file

This file is part of LightString.

LightString is free software; you can redistribute it and/or modify it under
the terms of the GNU Lesser General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

SCPM is distributed WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

You should have received a copy of the GNU Lesser General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

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

        /// <summary>
        /// Returns a reference of this System.String reversed.
        /// </summary>
        /// <param name="str">this string.</param>
        /// <returns></returns>
        public static string ReverseInPlace(this string str)
        {
            if (str == null)
                return null;

            unsafe
            {
                fixed (char* c = str)
                {
                    char* p = c;
                    char* q = c + str.Length - 1;
                    for (; p < q; ++p, --q)
                    {
                        char t = *p;
                        *p = *q;
                        *q = t;
                    }
                }
            }

            return str;
        }
    }
}
