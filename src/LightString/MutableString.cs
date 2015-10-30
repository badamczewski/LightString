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
    /// Represents a string that's mutable.
    /// </summary>
    public class MutableString
    {
        private string str;
        public const char PointerEnd = '\0';

        public MutableString(string str)
        {
            this.str = str;
        }

        public string Value 
        { 
            get { return str; } 
        }  

        public static implicit operator string(MutableString @this)
        {
            return @this.str;
        }

        public static implicit operator MutableString(string @this)
        {
            return @this.Mutate();
        }

        /// <summary>
        /// Returns a reference of this System.String converted to uppercase, using the casing
        //  rules of the current culture.
        /// </summary>
        /// <returns></returns>
        public MutableString ToUpperInPlace()
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

            return this;
        }

        /// <summary>
        /// Returns a reference of this System.String converted to lowercase, using the casing
        //  rules of the current culture.
        /// </summary>
        /// <returns></returns>
        public MutableString ToLowerInPlace()
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    for (char* p = c; *p != 0; p++)
                    {
                        *p = char.ToLower(*p);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the current System.String object,
        /// using pointer arthmetic and returns a copy of the string.
        /// </summary>
        /// <returns></returns>
        public MutableString TrimInPlaceWithCopy()
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* start = c;
                    char* end = c;
                    char cpy = PointerEnd;

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
                            if (char.IsWhiteSpace(*end) == false) break;
                        }

                        *(end + 1) = PointerEnd;
                    }

                    str = new string(start);
                    *(end + 1) = cpy;
                    return this; 
                }
            }
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the current System.String object,
        /// using pointer arthmetic and returns a reference to that string.
        /// </summary>
        /// <returns></returns>
        public MutableString TrimInPlace()
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* start = c;
                    char* curr = c;

                    char* tailingSpace = c;
                    char* leadingSpace = c;

                    bool wasSpace = false;
                    bool startsWithSpace = false;

                    if (char.IsWhiteSpace(*start) == true)
                        startsWithSpace = true;

                    for (; *start != 0; start++)
                    {
                        if (char.IsWhiteSpace(*start) == true)
                        {
                            if (wasSpace)
                            {
                                *(curr) = *start;
                                curr++;
                            }
                            else
                            {
                                leadingSpace++;
                            }
                        }
                        else
                        {
                            wasSpace = true;
                            tailingSpace = curr;

                            if (startsWithSpace == true)
                                *(curr) = *start;

                            curr++;
                        }
                    }

                    int len = str.Length - (int)(curr - tailingSpace - 1) - (int)(leadingSpace - c);
                    ResizeStringInPlace(str, len);

                    return this;
                }
            }
        }

        /// <summary>
        // Returns a string array that contains the substrings in this string that are
        // delimited by element of a specified Unicode character.
        /// </summary>
        /// <param name="sep">character separator.</param>
        /// <returns></returns>
        public Split SplitInPlace(char sep)
        {
            unsafe
            {
                var split = new Split(str);
                split.Add(-1);

                fixed (char* c = str)
                {
                    char* start = c;
                    int cnt = 0;

                    for (; *start != 0; start++)
                    {
                        if (*start == sep)
                        {
                            split.Add(cnt);
                        }

                        cnt++;
                    }
                }

                split.Add(str.Length);
                
                return split;
            }
        }

        /// <summary>
        /// Replaces all occurrences of a specified Unicode character in this instance
        /// with another specified Unicode character.
        /// </summary>
        /// <param name="newChar"></param>
        /// <param name="oldChar"></param>
        /// <returns></returns>
        public MutableString ReplaceInPlace(char oldChar, char newChar)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    for (char* p = c; *p != 0; p++)
                    {
                        if (*p == oldChar)
                            *p = newChar;
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts at a specified
        /// character position and returns a copy of the string.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public MutableString SubstringInPlaceWithCopy(int startIndex)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* p = c + startIndex;
                    str = new string(p);
                }
            }

            return this;
        }

        /// <summary>
        /// Returns a reference of this System.String reversed.
        /// </summary>
        /// <returns></returns>
        public MutableString ReverseInPlace()
        {
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

            return this;
        }

        /// <summary>
        /// Replaces two or more whitespaces with single whitespace.
        /// </summary>
        /// <returns>Trimmed string</returns>
        public MutableString TrimMiddleAllInPlace()
        {
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

                    ResizeStringInPlace(str, (int)newLength);
                }
            }

            return this;

        }

        /// <summary>
        /// Sorts characters in a string in O(N) time, where N is length of the string.
        /// </summary>
        /// <returns>String with characters ordered</returns>
        public MutableString RadixSortInPlace()
        {
            int[] counts = new int[char.MaxValue];

            unsafe
            {
                fixed (char* c = str)
                {
                    char* p = c;
                    for (; p < c + str.Length; ++p)
                        counts[(int)*p]++;

                    p = c;
                    for (int i = 0; i < counts.Length; ++i)
                    {
                        int k = counts[i];
                        while (k-- > 0)
                            *(p++) = (char)i;
                    }
                }
            }

            return this;
        }

        unsafe private string ResizeStringInPlace(string str, int newLength)
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

    }
}
