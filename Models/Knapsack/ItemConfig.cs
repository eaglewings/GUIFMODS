using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack.Models
{
    public class ItemConfig
    {
        private int characteristics;

        public int Characteristics
        {
            get { return characteristics; }
            set { characteristics = value; }
        }

        private int profits;

        public int Profits
        {
            get { return profits; }
            set { profits = value; }
        }

        private int items;

        public int Items
        {
            get { return items; }
            set { items = value; }
        }

    }
}
