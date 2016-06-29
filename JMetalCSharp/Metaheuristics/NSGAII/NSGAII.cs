using JMetalCSharp.Core;
using JMetalCSharp.Utils;
using JMetalCSharp.Utils.Comparators;
using System;

namespace JMetalCSharp.Metaheuristics.NSGAII
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


    /// <summary>
    /// Implementation of NSGA-II. This implementation of NSGA-II makes use of a
    /// QualityIndicator object to obtained the convergence speed of the algorithm.
    /// This version is used in the paper: A.J. Nebro, J.J. Durillo, C.A. Coello
    /// Coello, F. Luna, E. Alba "A Study of Convergence Speed in Multi-Objective
    /// Metaheuristics." To be presented in: PPSN'08. Dortmund. September 2008.
    /// </summary>
    public class NSGAII : Algorithm
	{
        public event GenerationCalculatedEventHandler GenerationCalculated;
        public event SolutionEvaluatedEventHandler SolutionEvaluated;

        int populationSize = -1;
        int maxEvaluations = -1;
        int evaluations;
        int totalEvaluations;

        int evaluationsEventCounter;

        QualityIndicator.QualityIndicator indicators = null; // QualityIndicator object
        int requiredEvaluations; // Use in the example of use of the
                                 // indicators object (see below)

        SolutionSet population;
        SolutionSet offspringPopulation;

        Operator mutationOperator;
        Operator crossoverOperator;
        Operator selectionOperator;



        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problem">Problem to solve</param>
        public NSGAII(Problem problem)
			: base(problem)
		{
            EvaluationsBetweenEvents = 50;
		}

        #endregion

        public int EvaluationsBetweenEvents { get; set; }

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
            if ((EvaluationsBetweenEvents) > 0 &&  (evaluationsEventCounter >= EvaluationsBetweenEvents))
            {
                evaluationsEventCounter = 0;
                e = new NSGAIIEventArgs(GetCurrentFront());
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
            SolutionSet nondominatedFront;
			// Generations 
			while (evaluations < maxEvaluations)
			{
                nondominatedFront = CalculateNextGeneration();
                OnGenerationCalculated(new NSGAIIEventArgs(population));
			}

			// Return as output parameter the required evaluations
			SetOutputParameter("evaluations", requiredEvaluations);

			// Return the first non-dominated front
			Ranking rank = new Ranking(population);

			Result = rank.GetSubfront(0);

			return Result;
		}
        #endregion

        public void Initialize()
        {
            //Read the parameters
            JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "maxEvaluations", ref maxEvaluations);
            JMetalCSharp.Utils.Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);
            JMetalCSharp.Utils.Utils.GetIndicatorsFromParameters(this.InputParameters, "indicators", ref indicators);

            //Initialize the variables
            population = new SolutionSet(populationSize);
            evaluations = 0;
            totalEvaluations = 0;

            requiredEvaluations = 0;

            //Read the operators
            mutationOperator = Operators["mutation"];
            crossoverOperator = Operators["crossover"];
            selectionOperator = Operators["selection"];

            Random random = new Random(2);
            JMetalRandom.SetRandom(random);

            // Create the initial solutionSet
            population = CreateInitialPopulation(populationSize);
        }

        private SolutionSet CreateInitialPopulation(int populationSize)
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
            totalEvaluations = evaluations;
            return initialPopulation;
        }

        private SolutionSet CalculateNextGeneration()
        {
            SolutionSet union;

            Distance distance = new Distance();

            // Create the offSpring solutionSet      
            offspringPopulation = new SolutionSet(populationSize);
            Solution[] parents = new Solution[2];
            for (int i = 0; i < (populationSize / 2); i++)
            {
                if (evaluations < maxEvaluations)
                {
                    //obtain parents
                    parents[0] = (Solution)selectionOperator.Execute(population);
                    parents[1] = (Solution)selectionOperator.Execute(population);
                    Solution[] offSpring = (Solution[])crossoverOperator.Execute(parents);
                    mutationOperator.Execute(offSpring[0]);
                    mutationOperator.Execute(offSpring[1]);
                    Evaluate(offSpring[0]);
                    Evaluate(offSpring[1]);
                }
            }
            
            // Create the solutionSet union of solutionSet and offSpring
            union = ((SolutionSet)population).Union(offspringPopulation);

            // Ranking the union
            Ranking ranking = new Ranking(union);

            int remain = populationSize;
            int index = 0;
            SolutionSet front = null;
            population.Clear();

            // Obtain the next front
            front = ranking.GetSubfront(index);

            while ((remain > 0) && (remain >= front.Size()))
            {
                //Assign crowding distance to individuals
                distance.CrowdingDistanceAssignment(front, Problem.NumberOfObjectives);
                //Add the individuals of this front
                for (int k = 0; k < front.Size(); k++)
                {
                    population.Add(front.Get(k));
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
                    population.Add(front.Get(k));
                }

                remain = 0;
            }

            // This piece of code shows how to use the indicator object into the code
            // of NSGA-II. In particular, it finds the number of evaluations required
            // by the algorithm to obtain a Pareto front with a hypervolume higher
            // than the hypervolume of the true Pareto front.
            if ((indicators != null) && (requiredEvaluations == 0))
            {
                double HV = indicators.GetHypervolume(population);
                if (HV >= (0.98 * indicators.TrueParetoFrontHypervolume))
                {
                    requiredEvaluations = evaluations;
                }
            }
            return ranking.GetSubfront(0);
        }

        private void Evaluate(Solution solution)
        {
            Problem.Evaluate(solution);
            Problem.EvaluateConstraints(solution);
            offspringPopulation.Add(solution);
            evaluations++;
            totalEvaluations++;
            OnSolutionEvaluated();
        }

        private SolutionSet GetCurrentFront()
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
