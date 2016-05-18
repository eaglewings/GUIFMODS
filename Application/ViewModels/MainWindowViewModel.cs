using Controls;
using JMetalCSharp.Core;
using Knapsack.Helpers;
using Knapsack.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MainApp.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private AlgorithmController algorithmController;

        public MainWindowViewModel()
        {
            Axes = new List<Axis> {
                    new Axis {Max = 1,Min = 0,Title = "O1" }, new Axis {Max = 5,Min = 3,Title = "O2" }, new Axis {Max = 5,Min = 0,Title = "O3" }, new Axis {Max = 2,Min = 1,Title = "O4" }
            };

            Lines = new ObservableCollection<ChartLine>();
        }
        #region Properties
        private DataTable solutions = new DataTable();

        public DataTable Solutions
        {
            get { return solutions; }
            set
            {
                if (solutions != value)
                {
                    solutions = value;
                    OnPropertyChanged("Solutions");
                }
            }
        }

        private DataRowView selectedSolution;

        public DataRowView SelectedSolution
        {
            get { return selectedSolution; }
            set
            {
                if (selectedSolution != value)
                {
                    selectedSolution = value;
                    if (selectedSolution != null)
                    {
                        double[] objectives = new double[problem.NumberOfProfits];
                        for (int i = 0; i < objectives.Length; i++)
                        {
                            objectives[i] = (double) selectedSolution.Row.ItemArray[i];
                        }
                        SelectedLine = GetChartLine(objectives);
                    }
                    
                    OnPropertyChanged("SelectedSolution");
                }
            }
        }


        private Knapsack.Models.Problem problem;

        private Knapsack.Models.Problem Problem
        {
            get { return problem; }
            set
            {
                problem = value;
                DataTable items = new DataTable();
                for (int i = 0; i < problem.Capacities.Length; i++)
                {
                    items.Columns.Add(string.Format("c{0}", i + 1), typeof(double));
                }
                for (int i = 0; i < problem.NumberOfProfits; i++)
                {
                    items.Columns.Add(string.Format("p{0}", i + 1), typeof(double));
                }
                foreach (Item item in problem.Items)
                {
                    var row = item.Characteristics.Concat(item.Profits).ToArray().Cast<object>().ToArray();
                    items.Rows.Add(row);
                }

                Items = items;

                List<Axis> axes = new List<Axis>();
                for (int i = 0; i < problem.NumberOfProfits; i++)
                {
                    Axis axis = new Axis { Max = 1, Min = 0};
                    axis.Title = string.Format("O{0}", i + 1);
                    axes.Add(axis);
                }
                Axes = axes;
            }
        }

        private UserConstraintsMethod selectedUserConstraintHandlingMethod;

        public UserConstraintsMethod SelectedConstraintHandlingMethod
        {
            get { return selectedUserConstraintHandlingMethod; }
            set
            {
                selectedUserConstraintHandlingMethod = value;
                OnPropertyChanged("SelectedConstraintHandlingMethod");
            }
        }

        public IEnumerable<UserConstraintsMethod> ConstraintHandlingMethods
        {
            get
            {
                return Enum.GetValues(typeof(UserConstraintsMethod))
                    .Cast<UserConstraintsMethod>();
            }
        }

        private SolutionSet currentSolutionSet;

        public SolutionSet CurrentSolutionSet
        {
            get { return currentSolutionSet; }
            set {
                currentSolutionSet = value;

                DataTable solutions = new DataTable();
                ObservableCollection<ChartLine> lines = new ObservableCollection<ChartLine>();

                var s = currentSolutionSet.Get(0);
                double[] max = new double[s.NumberOfObjectives];

                for (int i = 0; i < max.Length; i++)
                {
                    max[i] = double.MinValue;
                }
                double[] min = new double[s.NumberOfObjectives];
                for (int i = 0; i < min.Length; i++)
                {
                    min[i] = double.MaxValue;
                }

                for (int i = 0; i < s.NumberOfObjectives; i++)
                {
                    solutions.Columns.Add(string.Format("O{0}", i + 1), typeof(double));
                }
                solutions.Columns.Add("Items", typeof(string));
                foreach (var solution in currentSolutionSet.SolutionsList)
                {
                    double[] objectives = new double[solution.NumberOfObjectives];
                    object[] row = new object[solutions.Columns.Count];
                    
                    for (int i = 0; i < solution.NumberOfObjectives; i++)
                    {
                        objectives[i] = solution.Objective[i] * -1;
                        max[i] = Math.Max(max[i], (double)objectives[i]);
                        min[i] = Math.Min(min[i], (double)objectives[i]);
                    }
                    Array.Copy(objectives, row, objectives.Length);
                    row[row.Length - 1] = solution.Variable[0].ToString();
                    solutions.Rows.Add(row);
                    lines.Add(GetChartLine(objectives));
                }
                Solutions = solutions;

                List<Axis> axes = new List<Axis>();

                for (int i = 0; i < problem.NumberOfProfits; i++)
                {
                    Axis axis = new Axis { Max = (int)Math.Ceiling(max[i]), Min = (int)Math.Floor(min[i])};
                    axis.BoundaryMax = axis.Max;
                    axis.BoundaryMin = axis.Min;
                    axis.Title = string.Format("O{0}", i + 1);
                    axes.Add(axis);
                }
                Axes = axes;
                Lines = lines;

                Hypervolume = Indicator.Hypervolume(value);

            }
        }

        private ChartLine GetChartLine(double[] objectives)
        {
            ChartLine line =
            new ChartLine
            {
                LineColor = Colors.Red,
                LineThickness = 1,
            };
            

            line.PointDataSource = new List<double>(objectives);
            return line;
        }

        private List<Axis> axes;

        public List<Axis> Axes
        {
            get { return axes; }
            set
            {
                if (axes != value)
                {
                    axes = value;
                    OnPropertyChanged("Axes");
                }
            }
        }
        private ObservableCollection<ChartLine> lines;

        public ObservableCollection<ChartLine> Lines
        {
            get { return lines; }
            set
            {
                if (lines != value)
                {
                    lines = value;
                    OnPropertyChanged("Lines");
                }
            }
        }

        private ChartLine selectedLine;

        public ChartLine SelectedLine
        {
            get { return selectedLine; }
            set
            {
                if (selectedLine != value)
                {
                    selectedLine = value;
                    OnPropertyChanged("SelectedLine");
                }
            }
        }



        private DataTable items;

        public DataTable Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        private int evaluations;

        public int Evaluations
        {
            get { return evaluations; }
            set
            {
                if (evaluations != value)
                {
                    evaluations = value;
                    OnPropertyChanged("Evaluations");
                }
            }
        }

        private double hypervolume = 0;

        public double Hypervolume
        {
            get { return hypervolume; }
            set
            {
                if (hypervolume != value)
                {
                    hypervolume = value;
                    OnPropertyChanged("Hypervolume");
                }
            }
        }


        #endregion Properties


        private void LoadProblem(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var loadedModel = formatter.Deserialize(stream);
            stream.Close();
            Problem = (Knapsack.Models.Problem)loadedModel;
        }

        public void GenerateData()
        {
            LoadProblem(@"C:\Users\Benjamin\Dropbox\FHNW\Master Thesis\Experiments\ks_500_5_1");
            SolveProblem();
        }

        #region Commands
        private void Load()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadProblem(dialog.FileName);
            }
        }

        private RelayCommand loadCmd;

        public RelayCommand LoadCmd
        {
            get
            {
                if (loadCmd == null)
                {
                    loadCmd = new RelayCommand(p => Load());
                }
                return loadCmd;
            }
            set { loadCmd = value; }
        }

        private void SolveProblem()
        {
            for (int i = 0; i < Problem.Capacities.Length; i++)
            {
                double sum = 0;
                foreach (var item in Problem.Items)
                {
                    sum += item.Characteristics[i];
                }
                Problem.Capacities[i] = sum/2;
            }
            KnapsackProblem ksproblem = new KnapsackProblem(Problem);
            for (int i = 0; i < Axes.Count; i++)
            {
                Axis axis = Axes[i];
                if(axis.BoundaryMax != axis.Max || axis.BoundaryMin != axis.Min) {
                    Constraint c = new Constraint();
                    c.ObjectiveIndex = i;
                    if(axis.BoundaryMax != axis.Max)
                    {
                        c.Max = axis.BoundaryMax;
                    }

                    if(axis.BoundaryMin != axis.Min)
                    {
                        c.Min = axis.BoundaryMin;
                    }
                    ksproblem.UserConstraints.Add(c);
                    }
            }

            ksproblem.UserConstraintHandling = SelectedConstraintHandlingMethod;
            algorithmController = new AlgorithmController(ksproblem);
            algorithmController.GenerationCalculated += AlgorithmController_GenerationCalculated;
            Task.Factory.StartNew(() =>
            {
                CurrentSolutionSet = algorithmController.Solve();
            }
            );
            
        }

        private void AlgorithmController_GenerationCalculated(object sender, JMetalCSharp.Metaheuristics.NSGAII.GenerateionCalculatedEventArgs e)
        {
            CurrentSolutionSet = e.Population;
            Evaluations = e.Evaluations;
        }

        public void UpdateCurrentSolutionSet(SolutionSet solutionSet)
        {
            DispatchService.Invoke(() =>
            {
                //this.CurrentSolutionSet.Add(null);
            });
        }

        private RelayCommand solveCmd;

        public RelayCommand SolveCmd
        {
            get
            {
                if (solveCmd == null)
                {
                    solveCmd = new RelayCommand(p => SolveProblem(), p => { return Problem != null; });
                }
                return solveCmd;
            }
            set { solveCmd = value; }
        }
        #endregion Commands
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public static class DispatchService
    {
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }
    }
}
