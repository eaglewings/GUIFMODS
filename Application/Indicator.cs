using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;
using System.Collections.Generic;

namespace MainApp
{

    public class Metric
    {
        public double HypervolumeConstrained { get; set; }
        public int SolutionsWithinConstraints { get; set; }
    }

    public class Indicator
    {
        public static double Hypervolume(SolutionSet set)
        {
            double hypervolume = 0;
            double[][] front = set.WriteObjectivesToMatrix();
            for (int i = 0; i < front.Length; i++)
            {
                for (int j = 0; j < front[i].Length; j++)
                {
                    front[i][j] *= -1;
                }
            }
            hypervolume = (new HyperVolume()).CalculateHypervolume(front, front.Length, front[0].Length);

            return hypervolume;
        }

        private static List<double[]> GetConstrainedSet(SolutionSet set, List<Constraint> constraints)
        {
            List<double[]> constrainedSolutions = new List<double[]>();
            double[][] solutions = set.WriteObjectivesToMatrix();
            int nrOfSolutions = solutions.Length;
            int nrOfObjectives = solutions[0].Length;
            Constraint[] objectiveConstraints = new Constraint[nrOfObjectives];
            foreach (Constraint constraint in constraints)
            {
                objectiveConstraints[constraint.ObjectiveIndex] = constraint;
            }
            for (int i = 0; i < nrOfSolutions; i++)
            {
                double[] solution = new double[nrOfObjectives];
                for (int j = 0; j < nrOfObjectives; j++)
                {
                    double objective = solutions[i][j] * (-1);
                    Constraint constraint = objectiveConstraints[j];

                    if (constraint != null)
                    {
                        if ((constraint.Max != null) && (objective > constraint.Max))
                        {
                            solution = null;
                            break;
                        }

                        if (constraint.Min != null)
                        {
                            if (objective < constraint.Min)
                            {
                                solution = null;
                                break;
                            }
                            else
                            {
                                objective -= (double)constraint.Min; //moves the reference point to constraints minimum
                            }
                        }
                    }
                    solution[j] = objective;
                }
                if (solution != null)
                {
                    constrainedSolutions.Add(solution);
                }
            }
            return constrainedSolutions;
        }

        public static double HypervolumeConstrained(SolutionSet set, List<Constraint> constraints)
        {
            double hypervolume = 0;

            List<double[]> constrainedSolutions = GetConstrainedSet(set, constraints);

            hypervolume = (new HyperVolume()).CalculateHypervolume(constrainedSolutions.ToArray(), constrainedSolutions.ToArray().Length, set.Get(0).NumberOfObjectives);

            return hypervolume;
        }

        public static Metric Metrics(SolutionSet set, List<Constraint> constraints)
        {
            Metric m = new Metric();
            List<double[]> constrainedSolutions = GetConstrainedSet(set, constraints);

            m.HypervolumeConstrained = (new HyperVolume()).CalculateHypervolume(constrainedSolutions.ToArray(), constrainedSolutions.ToArray().Length, set.Get(0).NumberOfObjectives);
            m.SolutionsWithinConstraints = constrainedSolutions.Count;

            return m;
        }
    }
}
