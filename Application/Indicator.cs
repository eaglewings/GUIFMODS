using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;
using System.Collections.Generic;

namespace MainApp
{
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

        public static double HypervolumeConstrained(SolutionSet set, List<Constraint> constraints)
        {
            double hypervolume = 0;
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

                    if(constraint != null)
                    {
                        if((constraint.Min != null) && (objective < constraint.Min))
                        {
                            solution = null;
                            break;
                        }

                        if ((constraint.Max != null) && (objective > constraint.Max))
                        {
                            solution = null;
                            break;
                        }
                    }
                    solution[j] = objective;
                }
                if (solution != null)
                {
                    constrainedSolutions.Add(solution);
                }
            }


            hypervolume = (new HyperVolume()).CalculateHypervolume(constrainedSolutions.ToArray(), constrainedSolutions.ToArray().Length, nrOfObjectives);


            return hypervolume;
        }
    }
}
