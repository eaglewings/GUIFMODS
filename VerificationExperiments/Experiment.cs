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

        string filepath;

        public Experiment(KnapsackProblem problem)
        {
            this.problem = problem;
            algorithm = new DynamicNSGAII(problem);
            algorithm.SolutionEvaluated += Algorithm_SolutionEvaluated;
            filepath = @"C:\Users\Benjamin\Dropbox\FHNW\Master Thesis\Experiments\UserConstraintsExperiments\" + GetFileName(problem);
            File.AppendAllText(filepath, PrintConstraints() + Environment.NewLine);
            File.AppendAllText(filepath, "Evaluations;Generations;NonDominatedFrontSize;SolutionsWithinConstraints;ConstrainedHypervolume" + Environment.NewLine);
        }


        public void Execute(int evaluations)
        {
            algorithm.RequiredEvaluations += evaluations;
            algorithm.Execute();
        }


        private string GetFileName(KnapsackProblem problem)
        {
            string fileName;
            fileName = problem.NumberOfObjectives.ToString("'O'00") + problem.Problem.Items.Count.ToString("'I'000");
            
            if (problem.UserConstraintHandling == UserConstraintsMethod.DefaultConstraint)
            {
                fileName += "D";
            }
            else
            {
                fileName += "N";
            }

            fileName += problem.UserConstraints.Count.ToString("'C'00");
            fileName += DateTime.Now.ToString("s").Replace(":","");
            fileName += ".txt";
            return fileName;
        }

        private void Algorithm_SolutionEvaluated(object sender, NSGAIIEventArgs e)
        {
            DynamicNSGAII algorithm = (DynamicNSGAII)sender;
            Metric m = Indicator.Metrics(e.Population, problem.UserConstraints);

            string line = e.Evaluations + ";" + algorithm.Generation + ";" + e.Population.Size() + ";" + m.SolutionsWithinConstraints + ";" + m.HypervolumeConstrained;

            File.AppendAllText(filepath, line + Environment.NewLine);
            Console.WriteLine(e.Evaluations + "/" + algorithm.RequiredEvaluations +" = " + (100 * (double) e.Evaluations/algorithm.RequiredEvaluations).ToString("0.00") + " %");
        }

        private string PrintConstraints()
        {
            string s = problem.UserConstraintHandling + "; ";

            foreach (var constraint in problem.UserConstraints)
            {
                s += "O" + constraint.ObjectiveIndex + " Min=" + constraint.Min + " Max=" + constraint.Max + "; ";
            }

            return s;
        }

    }
}
