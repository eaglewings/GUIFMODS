using System.ComponentModel;

namespace Controls
{
    public class Axis : INotifyPropertyChanged
    {
        public Axis()
        {
            Min = 0;
            BoundaryMin = Min;
            Max = 1;
            BoundaryMax = Max;
        }

        public Axis(double min, double max)
        {
            Min = min;
            BoundaryMin = min;
            Max = max;
            BoundaryMax = max;
        }

        private double max;

        public double Max
        {
            get
            {
                return max;
            }
            set
            {
                if (max != value && value > Min)
                {
                    if (BoundaryMax > value)
                    {
                        BoundaryMax = value;
                    }
                    if (BoundaryMin > value)
                    {
                        BoundaryMin = value;
                    }
                    max = value;
                    OnPropertyChanged("Max");
                }
            }

        }

        private double min;

        public double Min
        {
            get
            {
                return min;
            }
            set
            {
                if (min != value && value < Max)
                {
                    if(BoundaryMax < value)
                    {
                        BoundaryMax = value;
                    }
                    if(BoundaryMin < value)
                    {
                        BoundaryMin = value;
                    }
                    min = value;
                    OnPropertyChanged("Min");
                }
            }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        private double boundaryMin;

        public double BoundaryMin
        {
            get { return boundaryMin; }
            set
            {
                if (boundaryMin != value)
                {
                    boundaryMin = value;
                    OnPropertyChanged("BoundaryMin");
                }
            }
        }

        private double boundaryMax;

        public double BoundaryMax
        {
            get { return boundaryMax; }
            set
            {
                if (boundaryMax != value)
                {
                    boundaryMax = value;
                    OnPropertyChanged("BoundaryMax");
                }
            }
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
