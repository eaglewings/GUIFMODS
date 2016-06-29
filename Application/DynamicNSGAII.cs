using JMetalCSharp.Core;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    public class NSGAIIEventArgs : EventArgs
    {
        public NSGAIIEventArgs(SolutionSet p)
        {
            Population = p;
        }
        private SolutionSet population;

        public SolutionSet Population
        {
            get { return population; }
            set { population = value; }
        }

        public int Evaluations { get; set; }
    }

    public delegate void GenerationCalculatedEventHandler(object sender, NSGAIIEventArgs e);
    public delegate void SolutionEvaluatedEventHandler(object sender, NSGAIIEventArgs e);


    enum DynamicNSGAIIState
    {
        Initialization,
        SelectAndMate,
        Evaluate1,
        Evaluate2,
        MakeNextParentPopulation
    }

    public class DynamicNSGAII : Algorithm
    {

        public event GenerationCalculatedEventHandler GenerationCalculated;
        public event SolutionEvaluatedEventHandler SolutionEvaluated;

        int populationSize = -1;
        int evaluations;

        int evaluationsEventCounter;

        volatile bool isRunning;

        SolutionSet population;
        SolutionSet offspringPopulation;
        Solution[] parents = new Solution[2];
        Solution[] offspring = new Solution[2];
        
        Operator mutationOperator;
        Operator crossoverOperator;
        Operator selectionOperator;

        DynamicNSGAIIState state = DynamicNSGAIIState.Initialization;

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problem">Problem to solve</param>
        public DynamicNSGAII(Problem problem)
			: base(problem)
		{
            EvaluationsBetweenEvents = 50;
            RequiredEvaluations = 0;

            populationSize = 100;

            generation = 0;

            // Mutation and Crossover for Real codification 
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("probability", 0.9);
            Operator crossover = CrossoverFactory.GetCrossoverOperator("SinglePointCrossover", parameters);

            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 1.0 / problem.NumberOfVariables);
            Operator mutation = MutationFactory.GetMutationOperator("BitFlipMutation", parameters);

            // Selection Operator 
            parameters = null;
            Operator selection = SelectionFactory.GetSelectionOperator("BinaryTournament2", parameters);

            // Add the operators to the algorithm
            AddOperator("crossover", crossover);
            AddOperator("mutation", mutation);
            AddOperator("selection", selection);
            
        }

        #endregion

        public int EvaluationsBetweenEvents { get; set; }

        public int RequiredEvaluations { get; set; }

        private int generation;
        public int Generation
        {
            get
            {
                return generation;
            }
        }


        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }


        protected virtual void OnGenerationCalculated(NSGAIIEventArgs e)
        {
            e.Evaluations = evaluations;
            if (GenerationCalculated != null)
            {
                GenerationCalculated(this, e);
            }
        }

        protected virtual void OnSolutionEvaluated()
        {
            NSGAIIEventArgs e;

            evaluationsEventCounter++;
            if ((EvaluationsBetweenEvents) > 0 && (evaluationsEventCounter >= EvaluationsBetweenEvents))
            {
                evaluationsEventCounter = 0;
                e = new NSGAIIEventArgs(GetCurrentNondominatedFront());
                e.Evaluations = evaluations;
                if (SolutionEvaluated != null)
                {
                    SolutionEvaluated(this, e);
                }
            }
        }

        #region Public Override

        /// <summary>
        /// Runs the NSGA-II algorithm.
        /// </summary>
        /// <returns>a <code>SolutionSet</code> that is a set of non dominated solutions as a result of the algorithm execution</returns>
        public override SolutionSet Execute()
        {
            while (KeepRunning())
            {
                switch (state)
                {
                    case DynamicNSGAIIState.Initialization:
                        Initialize();
                        state = DynamicNSGAIIState.SelectAndMate;
                        break;
                    case DynamicNSGAIIState.SelectAndMate:
                        SelectAndMate();
                        state = DynamicNSGAIIState.Evaluate1;
                        break;
                    case DynamicNSGAIIState.Evaluate1:
                        Evaluate(offspring[0]);
                        state = DynamicNSGAIIState.Evaluate2;
                        break;
                    case DynamicNSGAIIState.Evaluate2:
                        Evaluate(offspring[1]);
                        if (offspringPopulation.Size() < offspringPopulation.Capacity)
                        {
                            state = DynamicNSGAIIState.SelectAndMate;
                        }
                        else
                        {
                            state = DynamicNSGAIIState.MakeNextParentPopulation;
                        }
                        break;
                    case DynamicNSGAIIState.MakeNextParentPopulation:
                        MakeNextParentPopulation();
                        state = DynamicNSGAIIState.SelectAndMate;
                        break;
                }
            }
            return GetCurrentNondominatedFront();
        }
        #endregion

        public void Initialize()
        {
            
            //Initialize the variables
            population = new SolutionSet(populationSize);
            evaluations = 0;
            

            //Read the operators
            mutationOperator = Operators["mutation"];
            crossoverOperator = Operators["crossover"];
            selectionOperator = Operators["selection"];

            Random random = new Random(2);
            JMetalRandom.SetRandom(random);

            // Create the initial solutionSet
            population = CreateInitialPopulation();
            offspringPopulation = new SolutionSet(populationSize);
        }

        private SolutionSet CreateInitialPopulation()
        {
            SolutionSet initialPopulation = new SolutionSet(populationSize);
            Solution newSolution;
            for (int i = 0; i < populationSize; i++)
            {
                newSolution = new Solution(Problem);
                Problem.Evaluate(newSolution);
                Problem.EvaluateConstraints(newSolution);
                evaluations++;
                initialPopulation.Add(newSolution);
            }
            return initialPopulation;
        }

        private bool KeepRunning()
        {
            bool keepRunning = isRunning;

            if (RequiredEvaluations != 0)
            {
                keepRunning = (evaluations < RequiredEvaluations);
            }

            return keepRunning;
        }

        private void SelectAndMate()
        {
            parents[0] = (Solution)selectionOperator.Execute(population);
            parents[1] = (Solution)selectionOperator.Execute(population);
            offspring = (Solution[])crossoverOperator.Execute(parents);
            mutationOperator.Execute(offspring[0]);
            mutationOperator.Execute(offspring[1]);
        }

        private void Evaluate(Solution solution)
        {
            Problem.Evaluate(solution);
            Problem.EvaluateConstraints(solution);
            offspringPopulation.Add(solution);
            evaluations++;
            OnSolutionEvaluated();
        }

        private void MakeNextParentPopulation()
        {
            Distance distance = new Distance();

            // Create the solutionSet union of solutionSet and offSpring
            SolutionSet union = ((SolutionSet)population).Union(offspringPopulation);

            // Ranking the union
            Ranking ranking = new Ranking(union);

            int remain = populationSize;
            int index = 0;
            SolutionSet front = null;
            SolutionSet newParentPopulation = new SolutionSet(populationSize);

            // Obtain the next front
            front = ranking.GetSubfront(index);

            while ((remain > 0) && (remain >= front.Size()))
            {
                //Assign crowding distance to individuals
                distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
                //Add the individuals of this front
                for (int k = 0; k < front.Size(); k++)
                {
                    newParentPopulation.Add(front.Get(k));
                }

                //Decrement remain
                remain = remain - front.Size();

                //Obtain the next front
                index++;
                if (remain > 0)
                {
                    front = ranking.GetSubfront(index);
                }
            }

            // Remain is less than front(index).size, insert only the best one
            if (remain > 0)
            {  // front contains individuals to insert                        
                distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
                front.Sort(new CrowdingComparator());
                for (int k = 0; k < remain; k++)
                {
                    newParentPopulation.Add(front.Get(k));
                }

                remain = 0;
            }

            population = newParentPopulation;
            generation++;
            offspringPopulation.Clear();
        }

        private SolutionSet GetCurrentNondominatedFront()
        {
            SolutionSet union;

            // Create the solutionSet union of solutionSet and offSpring
            union = ((SolutionSet)population).Union(offspringPopulation);

            // Ranking the union
            Ranking ranking = new Ranking(union);

            return ranking.GetSubfront(0);
        }
    }
}
