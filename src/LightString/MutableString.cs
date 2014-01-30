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

        /// <summary>
        /// Returns a reference of this System.String converted to uppercase, using the casing
        //  rules of the current culture.
        /// </summary>
        /// <returns></returns>
        public MutableString UnsafeToUpperInPlace()
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
        /// Removes all leading and trailing white-space characters from the current System.String object,
        /// using pointer arthmetic and returns a copy of the string.
        /// </summary>
        /// <returns></returns>
        public MutableString UnsafeTrimInPlaceWithCopy()
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
        public MutableString UnsafeTrimInPlace()
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
                    UnsafeResizeStringInPlace(str, len);

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
        public Split UnsafeSplitInPlaceWithCopy(char sep)
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

        /// <summary>
        /// Returns a reference of this System.String reversed.
        /// </summary>
        /// <returns></returns>
        public MutableString UnsafeReverseInPlace()
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

            return this;
        }

        /// <summary>
        /// Replaces two or more whitespaces with single whitespace.
        /// </summary>
        /// <returns>Trimmed string</returns>
        public MutableString UnsafeTrimMiddleAllInPlace()
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

            return this;

        }

        unsafe private string UnsafeResizeStringInPlace(string str, int newLength)
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
        /// <returns>String with characters ordered</returns>
        public MutableString UnsafeRadixSortInPlace()
        {
            if (str == null)
                return null;

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
    }
}
