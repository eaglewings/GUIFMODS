using JMetalCSharp.Core;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Logic
{
    public class BinaryTournament3 : BinaryTournament2
    {
        /// <summary>
		/// Store the <code>Comparator</code> for check dominance
		/// </summary>
		private IComparer<Solution> dominance;

        /// <summary>
        /// Stores a permutation of the solutions in the solutionSet used
        /// </summary>
        private int[] a;

        /// <summary>
        /// Stores the actual index for selection
        /// </summary>
        private int index = 0;

        /// <summary>
        /// Constructor Creates a new instance of the Binary tournament operator
        /// (Deb's NSGA-II implementation version)
        /// </summary>
        /// <param name="parameters"></param>
        public BinaryTournament3(Dictionary<string, object> parameters)
			: base(parameters)
		{
            dominance = new DominanceComparator(new SoftConstraintViolationComparator());
        }

        /// <summary>
        /// Performs the operation
        /// </summary>
        /// <param name="obj">Object representing a SolutionSet</param>
        /// <returns>The selected solution</returns>
        public override object Execute(object obj)
        {
            SolutionSet population = (SolutionSet)obj;
            if (index == 0) //Create the permutation
            {
                a = (new PermutationUtility()).IntPermutation(population.Size());
            }

            Solution solution1, solution2;
            solution1 = population.Get(a[index]);
            solution2 = population.Get(a[index + 1]);

            index = (index + 2) % population.Size();

            int flag = dominance.Compare(solution1, solution2);
            if (flag == -1)
            {
                return solution1;
            }
            else if (flag == 1)
            {
                return solution2;
            }
            else if (solution1.CrowdingDistance > solution2.CrowdingDistance)
            {
                return solution1;
            }
            else if (solution2.CrowdingDistance > solution1.CrowdingDistance)
            {
                return solution2;
            }
            else if (JMetalRandom.NextDouble() < 0.5)
            {
                return solution1;
            }
            else
            {
                return solution2;
            }
        }

    }
}
