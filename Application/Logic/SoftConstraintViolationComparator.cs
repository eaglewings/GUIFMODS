using JMetalCSharp.Core;
using JMetalCSharp.Utils.Comparators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Logic
{
    public class SoftConstraintViolationComparator : IConstraintViolationComparator
    {
        enum Violation { None = 0, Soft = 1, Hard = 2}
        #region Implement Interface

        /// <summary>
        /// Compares two solutions.
        /// </summary>
        /// <param name="s1">The first <code>Solution</code></param>
        /// <param name="s2">The second <code>Solution</code></param>
        /// <returns>-1, or 0, or 1 if o1 is less than, equal, or greater than o2, respectively.</returns>
        public int Compare(Solution s1, Solution s2)
        {
            int result;
            double hard1, hard2, soft1, soft2;
            Violation v1, v2;
            hard1 = s1.OverallConstraintViolation;
            hard2 = s2.OverallConstraintViolation;
            soft1 = s1.OverallSoftConstraintViolation;
            soft2 = s2.OverallSoftConstraintViolation;

            v1 = GetViolation(s1);
            v2 = GetViolation(s2);

            if(v1 < v2)
            {
                result = -1;
            }
            else if(v1 > v2)
            {
                result = 1;
            }
            else // v1 == v2
            {
                switch (v1)
                {
                    case Violation.None:
                        result = 0;
                        break;
                    case Violation.Soft:
                        result = CompareSingleTypeViolation(soft1, soft2);
                        break;
                    case Violation.Hard:
                        result = CompareSingleTypeViolation(hard1, hard2);
                        break;
                    default:
                        result = 0;
                        break;
                }
            }

            return result;
        }

        private Violation GetViolation(Solution s)
        {
            if(s.OverallConstraintViolation < 0)
            {
                return Violation.Hard;
            }
            else if (s.OverallSoftConstraintViolation < 0)
            {
                return Violation.Soft;
            }

            return Violation.None;
        }

        private int CompareSingleTypeViolation(double overall1, double overall2)
        {
            int result;
            if ((overall1 < 0) && (overall2 < 0))
            {
                if (overall1 > overall2)
                {
                    result = -1;
                }
                else if (overall2 > overall1)
                {
                    result = 1;
                }
                else
                {
                    result = 0;
                }
            }
            else if ((overall1 == 0) && (overall2 < 0))
            {
                result = -1;
            }
            else if ((overall1 < 0) && (overall2 == 0))
            {
                result = 1;
            }
            else
            {
                result = 0;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns>Returns true if solutions s1 and/or s2 have an overall constraint violation < 0</returns>
        public bool NeedToCompare(Solution s1, Solution s2)
        {
            bool needToCompare;
            needToCompare = (s1.OverallConstraintViolation < 0) || (s2.OverallConstraintViolation < 0);
            needToCompare |= (s1.OverallSoftConstraintViolation < 0) || (s2.OverallSoftConstraintViolation < 0);

            return needToCompare;
        }

        #endregion
    }
}
