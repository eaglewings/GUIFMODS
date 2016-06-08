using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Encoding.Variable;
using System.Collections.Generic;

namespace MainApp
{
    public enum UserConstraintsMethod
    {
        NotConsidered,
        DefaultConstraint,
        ObjectiveFunction
    }

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

        private UserConstraintsMethod userConstraintHandling = UserConstraintsMethod.NotConsidered;

        public UserConstraintsMethod UserConstraintHandling
        {
            get { return userConstraintHandling; }
            set { userConstraintHandling = value; }
        }

        private List<Constraint> userConstraints = new List<Constraint>();

        public List<Constraint> UserConstraints
        {
            get { return userConstraints; }
            set { userConstraints = value; }
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

            if(UserConstraintHandling == UserConstraintsMethod.ObjectiveFunction)
            {
                EvaluateUser(solution);
            }
        }

        private void EvaluateUser(Solution solution)
        {
            double distanceToBound = 0;
            foreach (var constraint in UserConstraints)
            {
                if (constraint.ObjectiveIndex >= problem.NumberOfProfits)
                {
                    continue;
                }
                var objective = solution.Objective[constraint.ObjectiveIndex] * -1;
                if (constraint.Min != null)
                {
                    if (objective < constraint.Min)
                    {
                        distanceToBound = objective - (double)constraint.Min;
                    }
                }
                if (constraint.Max != null)
                {
                    if (objective > constraint.Max)
                    {
                        distanceToBound = (double)constraint.Max - objective;
                    }
                }
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
            if(UserConstraintHandling == UserConstraintsMethod.DefaultConstraint)
            {
                EvaluateUserConstraints(capacity, solution);
            }
        }

        private void EvaluateUserConstraints(double[] capacities, Solution solution)
        {
            int numberOfViolatedUserConstraints = 0;
            double overallUserConstraintViolation = 0;

            foreach (var constraint in UserConstraints)
            {
                if(constraint.ObjectiveIndex >= problem.NumberOfProfits)
                {
                    continue;
                }
                var objective = solution.Objective[constraint.ObjectiveIndex] * -1;
                if(constraint.Min != null)
                {
                    if(objective < constraint.Min)
                    {
                        numberOfViolatedUserConstraints++;
                        overallUserConstraintViolation += objective - (double)constraint.Min;
                    }
                }
                if(constraint.Max != null)
                {
                    if(objective > constraint.Max)
                    {
                        numberOfViolatedUserConstraints++;
                        overallUserConstraintViolation += (double)constraint.Max - objective;
                    }
                }
            }
            solution.NumberOfViolatedConstraints += numberOfViolatedUserConstraints;
            solution.OverallConstraintViolation += overallUserConstraintViolation;
        }
    }
}
