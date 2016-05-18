using JMetalCSharp.Core;
using JMetalCSharp.QualityIndicator;

namespace MainApp
{
    public class Indicator
    {
        public static double Hypervolume(SolutionSet set)
        {
            double hypervolume = 0;
            double[][] front = set.WriteObjectivesToMatrix();
            hypervolume = (new HyperVolume()).CalculateHypervolume(front, front.Length, front[0].Length);

            return hypervolume;
        }
    }
}
