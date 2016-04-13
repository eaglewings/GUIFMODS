using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.Utils;
using jMetalTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace JMetalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem problem; // The problem to solve
            Algorithm algorithm; // The algorithm to use
            Operator crossover; // Crossover operator
            Operator mutation; // Mutation operator
            Operator selection; // Selection operator

            Dictionary<string, object> parameters;

            Knapsack.Models.Problem ksproblem;

            var logger = Logger.Log;

            var appenders = logger.Logger.Repository.GetAppenders();
            var fileAppender = appenders[0] as log4net.Appender.FileAppender;
            fileAppender.File = "NSGAII.log";
            fileAppender.ActivateOptions();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"C:\Users\Benjamin\Desktop\ks_3_1_20", FileMode.Open, FileAccess.Read, FileShare.Read);
            var loadedModel = formatter.Deserialize(stream);
            stream.Close();
            ksproblem = (Knapsack.Models.Problem)loadedModel;

            ksproblem.Capacities[0] = 5;
            //ksproblem.Capacities[1] = 10;
            problem = new KnapsackProblem(ksproblem);

            algorithm = new JMetalCSharp.Metaheuristics.NSGAII.NSGAII(problem);
            //algorithm = new ssNSGAII(problem);

            // Algorithm parameters
            algorithm.SetInputParameter("populationSize", 100);
            algorithm.SetInputParameter("maxEvaluations", 25000);

            // Mutation and Crossover for Real codification 
            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 0.9);
            crossover = CrossoverFactory.GetCrossoverOperator("SinglePointCrossover", parameters);

            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 1.0 / problem.NumberOfVariables);
            mutation = MutationFactory.GetMutationOperator("BitFlipMutation", parameters);

            // Selection Operator 
            parameters = null;
            selection = SelectionFactory.GetSelectionOperator("BinaryTournament2", parameters);

            // Add the operators to the algorithm
            algorithm.AddOperator("crossover", crossover);
            algorithm.AddOperator("mutation", mutation);
            algorithm.AddOperator("selection", selection);

            long initTime = Environment.TickCount;
            SolutionSet population = algorithm.Execute();
            long estimatedTime = Environment.TickCount - initTime;



            // Result messages 
            logger.Info("Total execution time: " + estimatedTime + "ms");
            logger.Info("Variables values have been writen to file VAR");
            population.PrintVariablesToFile("VAR");
            logger.Info("Objectives values have been writen to file FUN");
            population.PrintObjectivesToFile("FUN");
            Console.WriteLine("Time: " + estimatedTime);
            Console.ReadLine();
        }
    }
}
