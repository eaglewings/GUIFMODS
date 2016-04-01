using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack.Models
{
    [Serializable]
    public class Item
    {
        double[] profits;
        double[] characteristics;

        public Item() { }

        public Item(int profits, int characteristics, Random random)
        {

            this.profits = new double[profits];
            this.characteristics = new double[characteristics];

            for (int i = 0; i < profits; i++)
            {
                this.profits[i] = random.NextDouble();
            }
            for (int i = 0; i < characteristics; i++)
            {
                this.characteristics[i] = random.NextDouble();
            }

        }

        public double[] Profits {
            get
            {
                return profits;
            }
        }
        public double[] Characteristics {
            get { return characteristics; }  }
    }
}
