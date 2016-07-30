using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;
using System.Collections.Generic;

namespace MainApp
{

    public class Metric
    {
        public double HypervolumeConstrained { get; set; }
        public int SolutionsWithinConstraints { get; set; }
        public int DistinctSolutionsWithinConstraints { get; set; }
        public SolutionSet DistinctSet { get; set; }
        public long HyperVolumeElapsedMiliseconds { get; set; }
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
        
        private static SolutionSet GetDistinctSolutions(SolutionSet set)
        {
            SolutionSet uniqueSolutions = new SolutionSet(set.Capacity);
            for (int i = 0; i < set.Size(); i++)
            {
                Solution s = set.Get(i);
                int count = 0;
                for (int j = i + 1; j < set.Size(); j++)
                {
                    if (set.Get(j).Variable[0].ToString() == s.Variable[0].ToString())
                    {
                        count++;
                    }
                }
                if(count == 0)
                {
                    uniqueSolutions.Add(s);
                }
            }
            return uniqueSolutions;
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
            int objectives = 0;

            if(constrainedSolutions.Count > 0)
            {
                objectives = constrainedSolutions[0].Length;
            }

            m.SolutionsWithinConstraints = constrainedSolutions.Count;


            SolutionSet distinctSet = GetDistinctSolutions(set);
            constrainedSolutions = GetConstrainedSet(distinctSet, constraints);
            if ((constrainedSolutions.Count > 500 && objectives >=5) || (constrainedSolutions.Count > 300 && objectives >= 7))
            {
                m.HypervolumeConstrained = 0;
            }
            else
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                m.HypervolumeConstrained = (new HyperVolume()).CalculateHypervolume(constrainedSolutions.ToArray(), constrainedSolutions.ToArray().Length, set.Get(0).NumberOfObjectives);
                watch.Stop();
                m.HyperVolumeElapsedMiliseconds = watch.ElapsedMilliseconds;
            }
            m.DistinctSolutionsWithinConstraints = constrainedSolutions.Count;
            m.DistinctSet = distinctSet;
            return m;
        }
    }
}
