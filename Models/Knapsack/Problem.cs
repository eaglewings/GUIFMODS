using System;
using System.Collections.Generic;

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

        private readonly List<Item> items;
        private double[] capacities;
        private int numberOfProfits;

        public Problem()
        {

        }

        public Problem(ItemConfig itemConfig)
        {
            this.items = CreateItems(itemConfig);
            capacities = new double[itemConfig.Characteristics];
            numberOfProfits = itemConfig.Profits;
        }

        private List<Item> CreateItems(ItemConfig itemConfig)
        {
            var items = new List<Item>();
            var random = new Random();

            for (int i = 0; i < itemConfig.Items; i++)
            {
                items.Add(new Item(itemConfig.Profits, itemConfig.Characteristics, random));
            }
            return items;
        }

        public List<Item> Items {
            get
            {
                return items;
            }
        }

        public double[] Capacities
        {
            get
            {
                return capacities;
            }
        }

        public int NumberOfProfits
        {
            get
            {
                return numberOfProfits;
            }
        }

    }



}
