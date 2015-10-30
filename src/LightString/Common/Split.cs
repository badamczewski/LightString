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

namespace LightString.Common
{
    /// <summary>
    /// The split object that contains indexes to splits.
    /// </summary>
    public class Split
    {
        private string str;

        private int capacity;
        private int[] indexes;
        private int idx;

        internal void Add(int index)
        {
            bool doExpand = idx == capacity;

            if (doExpand)
                Expand();

            indexes[idx++] = index;
        }



        private void Expand()
        {
            capacity = capacity * 2;
            int[] newArray = new int[capacity];

            Buffer.BlockCopy(indexes, 0, newArray, 0, indexes.Length);

            indexes = newArray;
        }

        public Split(string str)
        {  
            this.capacity = 16;
            this.indexes = new int[capacity];
            
            this.idx = 0;
            this.str = str;

        }

        public int Count { get { return idx == 2 ? 0 : idx - 1; } }

        public string UnsafeGetStringWithCopy(int position)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* start = c;
                    start += indexes[position] + 1;

                    if (*start == MutableString.PointerEnd)
                        start++;

                    char* end = c + indexes[position + 1] - 1;

                    *(end + 1) = MutableString.PointerEnd;

                    return new string(start);
                }
            }
        }
    }
}
