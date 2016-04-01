using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack.Models
{
    /// <summary>
    /// Knapsack problem specification
    /// Items
    /// Nr of characteristics
    /// Characteristics limits (capacities)
    /// </summary>
    [Serializable]
    public class Problem
    {

        readonly List<Item> items;
        private double[] capacities;

        public Problem()
        {

        }

        public Problem(ItemConfig itemConfig)
        {
            this.items = CreateItems(itemConfig);
            capacities = new double[itemConfig.Characteristics];
        }

        private List<Item> CreateItems(ItemConfig itemConfig)
        {
            var items = new List<Item>();
            var random = new Random();

            for (int i = 0; i < itemConfig.Items; i++)
            {
                items.Add(new Item(itemConfig.Characteristics, itemConfig.Profits, random));
            }
            return items;
        }

        public List<Item> Items {
            get
            {
                return items;
            }
        }

        public double[] Capacities {
            get
            {
                return capacities;
            }
            }

    }



}
