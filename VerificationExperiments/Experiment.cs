using JMetalCSharp.Core;
using MainApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationExperiments
{
    public class Experiment
    {
        KnapsackProblem problem;
        DynamicNSGAII algorithm;
        List<SolutionSet> nondominatedFronts = new List<SolutionSet>();

        string filepath;
        string separator = "\t";

        public Experiment(KnapsackProblem problem, int populationSize, string id)
        {
            this.problem = problem;
            algorithm = new DynamicNSGAII(problem, populationSize);
            algorithm.EvaluationsBetweenEvents = 500;
            algorithm.SolutionEvaluated += Algorithm_SolutionEvaluated;
            string directory = @"C:\Users\Benjamin\Dropbox\FHNW\Master Thesis\Experiments\" + id;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            filepath =Path.Combine( directory, GetFileName(problem));
            File.AppendAllText(filepath, "%" + PrintConstraints() + Environment.NewLine);
            File.AppendAllText(filepath, "Evaluations" + separator + "Generations" + separator + "NonDominatedFrontSize" + separator +
                "SolutionsWithinConstraints" + separator + "UniqueSolutionsWithinConstraints" + separator + "ConstrainedHypervolume" + Environment.NewLine);
        }

        public DynamicNSGAII Algorithm{
            get{return algorithm;}
            }


        public void Execute(int evaluations)
        {
            algorithm.RequiredEvaluations += evaluations;
            algorithm.Execute();
            PrintFronts();
        }


        private string GetFileName(KnapsackProblem problem)
        {
            string fileName="";
            
            fileName += problem.NumberOfObjectives.ToString("'O'00") + problem.Problem.Items.Count.ToString("'I'000");

            switch (problem.UserConstraintHandling)
            {
                case UserConstraintsMethod.NotConsidered:
                    fileName += "N";
                    break;
                case UserConstraintsMethod.DefaultConstraint:
                    fileName += "D";
                    break;
                case UserConstraintsMethod.SoftConstraint:
                    fileName += "S";
                    break;
                case UserConstraintsMethod.ObjectiveFunction:
                    fileName += "O";
                    break;
                default:
                    break;
            }
            
            fileName += problem.UserConstraints.Count.ToString("'C'00");
            
            fileName += ".txt";
            return fileName;
        }

        private void Algorithm_SolutionEvaluated(object sender, NSGAIIEventArgs e)
        {
            DynamicNSGAII algorithm = (DynamicNSGAII)sender;
            Metric m = Indicator.Metrics(e.Population, problem.UserConstraints);
            nondominatedFronts.Add(m.DistinctSet);
            string line = e.Evaluations + separator + algorithm.Generation + separator + e.Population.Size() + separator + m.SolutionsWithinConstraints;
            line += separator +m.DistinctSolutionsWithinConstraints + separator + m.HypervolumeConstrained;
            File.AppendAllText(filepath, line + Environment.NewLine);
            Console.WriteLine(e.Evaluations + "/" + algorithm.RequiredEvaluations +" = " +
                (100 * (double) e.Evaluations/algorithm.RequiredEvaluations).ToString("0.00") + " % : Evaluation " + 
                e.ElapsedtimeMs + " ms, HV " +m.HyperVolumeElapsedMiliseconds + " ms");
        }

        private string PrintConstraints()
        {
            string s = problem.UserConstraintHandling + separator;

            foreach (var constraint in problem.UserConstraints)
            {
                s += "O" + constraint.ObjectiveIndex + " Min=" + constraint.Min + " Max=" + constraint.Max + ", ";
            }

            return s;
        }

        private void PrintFronts()
        {
            string path = filepath.Replace(".txt", "_fronts.txt");
            
            int frontnr = 0;

            foreach (var front in nondominatedFronts)
            {
                
                frontnr++;
                if(front.SolutionsList.Count == 0)
                {
                    continue;
                }
                var nrObjectives = front.SolutionsList[0].NumberOfObjectives;
                List <string> lines = new List<string>(nrObjectives);

                double[] min = new double[nrObjectives];
                double[] max = new double[nrObjectives];
                for (int i = 0; i < nrObjectives; i++)
                {
                    lines.Add("");
                    min[i] = double.MaxValue;
                    max[i] = double.MinValue;
                }
                
                foreach (var s in front.SolutionsList)
                {
                    
                    for (int i = 0; i < nrObjectives; i++)
                    {
                        lines[i] += separator + s.Objective[i];
                        min[i] = s.Objective[i] < min[i] ? s.Objective[i] : min[i];
                        max[i] = s.Objective[i] > max[i] ? s.Objective[i] : max[i];
                    }
                }
                for (int i = 0; i < nrObjectives; i++)
                {
                    lines[i] = "Front " + frontnr + " O" + (i + 1) + separator + max[i] + separator + min[i] + lines[i];
                }
                File.AppendAllLines(path, lines);
            }
        }

    }
}
