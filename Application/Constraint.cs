using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp
{
    public class Constraint
    {
        private double? min;

        public double? Min
        {
            get { return min; }
            set { min = value; }
        }

        private double? max;

        public double? Max
        {
            get { return max; }
            set { max = value; }
        }

        private int objectiveIndex;

        public int ObjectiveIndex
        {
            get { return objectiveIndex; }
            set { objectiveIndex = value; }
        }
    }
}
