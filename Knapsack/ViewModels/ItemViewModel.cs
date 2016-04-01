using Knapsack.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Knapsack.ViewModels
{
    internal class ItemViewModel :INotifyPropertyChanged
    {
        private Item item;
        public ItemViewModel(Item item)
        {
            this.item = item;
            List<double> values = new List<double>();
            values.AddRange(item.Characteristics);
            values.AddRange(item.Profits);
            this.values = values;
        }

        private IList<double> values;

        public IList<double> Values
        {
            get { return values; }
            private set { values = value; }
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