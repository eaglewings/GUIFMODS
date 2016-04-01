using System.ComponentModel;

namespace Controls
{
    public class Axis : INotifyPropertyChanged
    {
        public Axis()
        {
            Min = 0;
            Max = 1;
        }

        private int max;

        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                if (max != value && value > Min)
                {
                    max = value;
                    OnPropertyChanged("Max");
                }
            }

        }

        private int min;

        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                if (min != value && value < Max)
                {
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
