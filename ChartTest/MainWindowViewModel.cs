using Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace ChartTest
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Random random = new Random(1234);

        public MainWindowViewModel()
        {
            Axes = new List<Axis> {
                    new Axis {Max = 1,Min = 0,Title = "O1" }, new Axis {Max = 5,Min = 3,Title = "O2" }, new Axis {Max = 5,Min = 0,Title = "O3" }, new Axis {Max = 2,Min = 1,Title = "O4" }
            };

            Lines = new ObservableCollection<ChartLine> {
                                                            new ChartLine {
                                                                              LineColor = Colors.Red,
                                                                              FillColor = Color.FromArgb(128, 255, 0, 0),
                                                                              LineThickness = 2,
                                                                              PointDataSource = GenerateRandomDataSet(),
                                                                              Name = "Chart 1"
                                                                          },
                                                            new ChartLine {
                                                                              LineColor = Colors.Blue,
                                                                              FillColor = Color.FromArgb(128, 0, 0, 255),
                                                                              LineThickness = 2,
                                                                              PointDataSource = GenerateRandomDataSet(),
                                                                              Name = "Chart 2"
                                                                          }
                                                        };
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

        public List<Axis> Axes { get; set; }

        public List<double> GenerateRandomDataSet()
        {
            var pts = new List<double>();
            foreach (Axis axis in Axes)
            {
                pts.Add(random.NextDouble() * (axis.Max - axis.Min) + axis.Min);
            }            
            return pts;
        }

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
}
