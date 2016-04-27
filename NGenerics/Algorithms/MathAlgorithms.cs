/*  
  Copyright 2007-2013 The NGenerics Team
 (https://github.com/ngenerics/ngenerics/wiki/Team)

 This program is licensed under the GNU Lesser General Public License (LGPL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://www.gnu.org/copyleft/lesser.html.
*/


using System;
using System.Diagnostics.CodeAnalysis;

namespace NGenerics.Algorithms
{
    /// <summary>
    /// General math functions.
    /// </summary>
    public static class MathAlgorithms
    {
        #region Fibonacci Sequence

      
        public static double Hypotenuse(double a, double b)
        {
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);

            return Math.Sqrt((absB * absB) + (absA * absA));
        }

        #endregion
    }
}
