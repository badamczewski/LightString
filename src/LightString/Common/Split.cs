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
        private List<int> indexes;
        private string str;

        public Split(List<int> indexes, string str)
        {
            this.indexes = indexes;
            this.str = str;
        }

        public int Count { get { return indexes.Count - 1; } }

        public string UnsafeGetStringWithCopy(int position)
        {
            unsafe
            {
                fixed (char* c = str)
                {
                    char* start = c;
                    start += indexes[position];

                    if (*start == StringOperations.PointerEnd)
                        start++;

                    char* end = c + indexes[position + 1] - 1;

                    *(end + 1) = StringOperations.PointerEnd;

                    return new string(start);
                }
            }
        }
    }
}
