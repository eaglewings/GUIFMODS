using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;

namespace jMetalTest
{
    public class KnapsackProblem : Problem
    {
        private Knapsack.Models.Problem problem;

        public KnapsackProblem()
        {
            ProblemName = "Knapsack";
            SolutionType = new BinarySolutionType(this);
        }

        public KnapsackProblem(Knapsack.Models.Problem problem) : this()
        {
            this.problem = problem;
            NumberOfConstraints = 0;
            NumberOfVariables = 1;
            NumberOfObjectives = problem.NumberOfProfits;
            Length = new int[] { problem.Items.Count };
        }

        public override void Evaluate(Solution solution)
        {
            Binary b = (Binary) solution.Variable[0];
            double[] profit = new double[problem.NumberOfProfits];

            for (int i = 0; i < problem.Items.Count; i++)
            {
                if (b.Bits[i]) {
                    for (int j = 0; j < profit.Length; j++)
                    {
                        profit[j] += problem.Items[i].Profits[j];
                    }
                }
            }

            for (int j = 0; j < profit.Length; j++)
            {
                solution.Objective[j] = -1*profit[j];
            }
        }

        public override void EvaluateConstraints(Solution solution)
        {
            Binary b = (Binary)solution.Variable[0];
            double[] capacity = new double[problem.Capacities.Length];

            for (int i = 0; i < problem.Items.Count; i++)
            {
                if (b.Bits[i])
                {
                    for (int j = 0; j < capacity.Length; j++)
                    {
                        capacity[j] += problem.Items[i].Characteristics[j];
                    }
                }
            }
            
            int numberOfViolatedConstraints = 0;
            double overallConstraintViolation = 0;
            for (int j = 0; j < capacity.Length; j++)
            {
                if (capacity[j] > problem.Capacities[j])
                {
                    numberOfViolatedConstraints++;
                    overallConstraintViolation += problem.Capacities[j] - capacity[j]; //must be negative
                }
            }
            solution.NumberOfViolatedConstraints = numberOfViolatedConstraints;
            solution.OverallConstraintViolation = overallConstraintViolation;
        }
    }
}
