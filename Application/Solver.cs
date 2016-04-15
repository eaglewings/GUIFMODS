using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using System.Collections.Generic;

namespace MainApp
{
    class Solver
    {
        public static SolutionSet Solve(KnapsackProblem problem)
        {
            Algorithm algorithm; // The algorithm to use
            Operator crossover; // Crossover operator
            Operator mutation; // Mutation operator
            Operator selection; // Selection operator

            Dictionary<string, object> parameters;
            
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
            
            SolutionSet population = algorithm.Execute();

            return population;
        }

        
    }
}
