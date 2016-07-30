using MainApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Knapsack.Models;


namespace VerificationExperiments
{
    class Program
    {
        static Knapsack.Models.Problem knapsackmodel;
        static List<Constraint> constraints;
        static string experimentID;
        static int evaluationsIntervall;
        static int maxEvaluations;
        static int populationSize;

        static readonly int[] ITEMS = { 500 };
        static readonly int[] NR_OBJECTIVES = { 5, 7 };

        static void Main(string[] args)
        {
            //Analysis.Write();
            ConductExperiments();
          // NadirUtopia.Write();
        }
        static void ConductExperiments()
        {
            experimentID = DateTime.Now.ToString("s").Replace(":", "-");

            evaluationsIntervall = 2000;
            maxEvaluations = 100000;
            populationSize = 500;

            experimentID = "P " + populationSize.ToString("0000") + " - Max " + (maxEvaluations / 1000).ToString("000")
                + "K - Intervall " + evaluationsIntervall.ToString("0000") + DateTime.Now.ToString("s").Replace(":", "-");

            foreach (var items in ITEMS)
            {
                foreach (var nrObjectives in NR_OBJECTIVES)
                {
                    string s = "ks_" + items + "_" + nrObjectives + "_1";
                    ExecuteExperiments(@"C:\Users\Benjamin\Dropbox\FHNW\Master Thesis\Experiments\" + s);
                }
            }
            Console.Read();
        }

        static void MakeConstraints()
        {
            constraints = new List<Constraint>();
            double[] profitSums = ProfitSums(knapsackmodel);
            for (int i = 0; i < knapsackmodel.NumberOfProfits; i++)
            {
                //constraints.Add(new Constraint { Min = 0, Max = 0, ObjectiveIndex = i });
            }
            
            //constraints.Add(new Constraint { Min = 0.33 * knapsackmodel.Items.Count, Max = null, ObjectiveIndex = 0 });
            /*constraints.Add(new Constraint { Min = 0.33 * knapsackmodel.Items.Count, Max = null, ObjectiveIndex = 1 });
            constraints.Add(new Constraint { Min = 0.33 * knapsackmodel.Items.Count, Max = null, ObjectiveIndex = 2 });
            //constraints.Add(new Constraint { Min = null, Max = 0.3 * knapsackmodel.Items.Count, ObjectiveIndex = 1 });
            */
        }

        static private void ExecuteExperiments(string problemPath)
        {
            LoadProblem(problemPath);
            MakeConstraints();
            MakeExperiment(constraints, UserConstraintsMethod.NotConsidered).Execute(maxEvaluations);

            //MakeExperiment(constraints, UserConstraintsMethod.DefaultConstraint).Execute(maxEvaluations);
            //MakeExperiment(constraints, UserConstraintsMethod.SoftConstraint).Execute(maxEvaluations);
        }

        static private Experiment MakeExperiment( List<Constraint> constraints, UserConstraintsMethod method)
        {
            KnapsackProblem ksp = new KnapsackProblem(knapsackmodel);
            ksp.UserConstraints = constraints;
            ksp.UserConstraintHandling = method;
            Experiment e = new Experiment(ksp, populationSize, experimentID);
            e.Algorithm.EvaluationsBetweenEvents = evaluationsIntervall;
            return e;
        }

        
        static private void LoadProblem(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            knapsackmodel = (Knapsack.Models.Problem) formatter.Deserialize(stream);
            stream.Close();
            for (int i = 0; i < knapsackmodel.Capacities.Length; i++)
            {
                double sum = 0;
                foreach (var item in knapsackmodel.Items)
                {
                    sum += item.Characteristics[i];
                }
                knapsackmodel.Capacities[i] = sum / 2;
            }
        }

        static double[] ProfitSums(Knapsack.Models.Problem problem)
        {
            double[] sums = new double[problem.NumberOfProfits];

            foreach (var item in problem.Items)
            {
                for (int i = 0; i < item.Profits.Length; i++)
                {
                    sums[i] += item.Profits[i];
                }
            }

            return sums;
        }
    }
}
