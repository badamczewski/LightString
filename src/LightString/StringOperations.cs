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
using LightString.Common;

namespace LightString
{
    /// <summary>
    /// Represents a set of string operations (extensions) that 
    /// operate on a reference of a string thus teating it like an 
    /// mutable data structure.
    /// </summary>
    public static class StringOperations
    {
        public const char PointerEnd = '\0';

        /// <summary>
        /// Returns a reference of this System.String converted to uppercase, using the casing
        //  rules of the current culture.
        /// </summary>
        /// <param name="str">this string.</param>
        /// <returns></returns>
        public static string UnsafeToUpperInPlace(this string str)
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
        public static string UnsafeTrimInPlaceWithCopy(this string str)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* start = c;
                    char* end;

                    for (; *start != 0; start++)
                    {
                        if (char.IsWhiteSpace(*start) == false) break;
                    }

                    //End
                    if (*start != 0)
                    {
                        end = c + (str.Length - 1);
                        for (; end > c; end--)
                        {
                            if (char.IsWhiteSpace(*end)) break;                           
                        }

                        *(end + 1) = PointerEnd;
                    }

                    return new string(start);
                }
            }
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the current System.String object,
        /// using pointer arthmetic and returns a reference to that string.
        /// </summary>
        /// <param name="str">this string.</param>
        /// <returns></returns>
        public static string UnsafeTrimInPlace(this string str)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* start = c;
                    char* curr = c;
                    bool wasSpace = false;

                    for (; *start != 0; start++)
                    {
                        if (char.IsWhiteSpace(*start) == true)
                        {
                            if (wasSpace)
                            {
                                *(curr) = *start;
                                curr++;
                            }
                        }
                        else
                        {
                            wasSpace = true;
                            *(curr) = *start;
                            curr++;
                        }
                    }

                    UnsafeResizeStringInPlace(str, (int)(curr - c - -1));

                    return str;
                }
            }
        }

        /// <summary>
        // Returns a string array that contains the substrings in this string that are
        // delimited by element of a specified Unicode character.
        /// </summary>
        /// <param name="str">this string.</param>
        /// <param name="sep">character separator.</param>
        /// <returns></returns>
        public static Split UnsafeSplitInPlaceWithCopy(this string str, char sep)
        {
            unsafe
            {
                List<int> indexes = new List<int>() { 0 };

                fixed (char* c = str)
                {
                    char* start = c;
                    int cnt = 0;

                    for (; *start != 0; start++)
                    {
                        if (*start == sep)
                        {
                            indexes.Add(cnt);
                        }

                        cnt++;
                    }
                }

                indexes.Add(str.Length - 1);

                return new Split(indexes, str);
            }
        }

        /// Returns a reference of this System.String reversed.
        /// </summary>
        /// <param name="str">this string.</param>
        /// <returns></returns>
        public static string UnsafeReverseInPlace(this string str)
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

        /// <summary>
        /// Replaces two or more whitespaces with single whitespace.
        /// </summary>
        /// <param name="str">String to trim</param>
        /// <returns>Trimmed string</returns>
        public static string UnsafeTrimMiddleAllInPlace(this string str)
        {
            if (str == null)
                return null;

            unsafe
            {
                fixed (char* c = str)
                {
                    char* p = c;
                    char* curr = c;
                    int spaceCount = 1; //trim all on the begining

                    for (; p < c + str.Length; ++p)
                    {
                        spaceCount = char.IsWhiteSpace(*p) ? spaceCount + 1 : 0;

                        if (spaceCount <= 1)
                            *(curr++) = *p;
                    }

                    long newLength = curr - c;

                    if (newLength > 0 && char.IsWhiteSpace(curr[-1]))
                        newLength--; //last char is space - trim them all!

                    UnsafeResizeStringInPlace(str, (int)newLength);
                }
            }

            return str;

        }

        unsafe private static string UnsafeResizeStringInPlace(string str, int newLength)
        {
            if (str == null)
                throw new ArgumentNullException();

            fixed (char* c = str)
            {
                int* ptr = (int*)c;

                if (newLength > ptr[-1])
                    throw new ArgumentException(string.Format("Argument cannot exceed actual lenght of string: {0}", ptr[-1]), "newLength");

                ptr[-1] = newLength;
                c[newLength] = PointerEnd;
            }

            return str;
        }

        /// <summary>
        /// Sorts characters in a string in O(N) time, where N is length of the string.
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>String with characters ordered</returns>
        public static string UnsafeRadixSortInPlace(this string str)
        {
            if (str == null)
                return null;

            int[] counts = new int[char.MaxValue];

            unsafe
            {
                fixed (char* c = str)
                {
                    char* p = c;
                    for(; p < c + str.Length; ++p)
                        counts[(int)*p]++;

                    p = c;
                    for(int i = 0; i < counts.Length; ++i)
                    {
                        int k = counts[i];
                        while(k-- > 0)
                            *(p++) = (char)i;
                    }
                }
            }

            return str;

        }

    }
}
