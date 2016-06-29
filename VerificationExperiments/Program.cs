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
        static KnapsackProblem problem1;
        static KnapsackProblem problem2;
        static List<Constraint> constraints;

        static void Main(string[] args)
        {
            LoadProblem(@"C:\Users\Benjamin\Desktop\ks_3_1_20");
            constraints = new List<Constraint>();
            /*
            for (int i = 0; i < problem1.NumberOfObjectives; i++)
            {
                constraints.Add(new Constraint { Min = null, Max = null, ObjectiveIndex = i });
            }
            */
            constraints.Add(new Constraint { Min = 2, Max = 6, ObjectiveIndex = 0 });
            problem1.UserConstraints = constraints;
            problem2.UserConstraints = constraints;

            problem1.UserConstraintHandling = UserConstraintsMethod.DefaultConstraint;
            problem2.UserConstraintHandling = UserConstraintsMethod.NotConsidered;

            var e1 = new Experiment(problem1);
            var e2 = new Experiment(problem2);

            e1.Execute(20000);
            e2.Execute(20000);
        }

        
        static private void LoadProblem(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Knapsack.Models.Problem loadedModel = (Knapsack.Models.Problem) formatter.Deserialize(stream);
            stream.Close();
            for (int i = 0; i < loadedModel.Capacities.Length; i++)
            {
                double sum = 0;
                foreach (var item in loadedModel.Items)
                {
                    sum += item.Characteristics[i];
                }
                loadedModel.Capacities[i] = sum / 2;
            }
            problem1 = new KnapsackProblem(loadedModel);
            problem2 = new KnapsackProblem(loadedModel);
            
        }
    }
}
