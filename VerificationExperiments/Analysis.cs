using MainApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationExperiments
{
    public class Analysis
    {
        static readonly int[] ITEMS = { 20, 200, 500 };
        static readonly int[] NR_OBJECTIVES = { 3, 5, 7 };

        static readonly UserConstraintsMethod[] METHODS = { UserConstraintsMethod.NotConsidered, UserConstraintsMethod.DefaultConstraint, UserConstraintsMethod.SoftConstraint };

        static public void Write()
        {
            List<string> paths = new List<string>();
            List<string> concatlines = new List<string>();
            List<FileStream> files = new List<FileStream>();
            List<string[]> lines = new List<string[]>();

            string dir = @"C:\Users\Benjamin\Dropbox\FHNW\Master Thesis\Experiments\P 0100 - Max 020K - Intervall 10002016-07-14T08-57-22\";
            int objectives = 7;

            foreach (var items in ITEMS)
            {
                foreach (var met in METHODS)
                {
                    paths.Add(dir + GetFileName(objectives, items, met, 1));
                }
            }
            

            foreach (var path in paths)
            {
                lines.Add(File.ReadAllLines(path));
            }

            foreach (var filelines in lines)
            {
                for (int i = 0; i < filelines.Length; i++)
                {
                    string line ="";
                    if(concatlines.Count < i + 1)
                    {
                        concatlines.Add("");
                    }
                    concatlines[i] += filelines[i] + "\t";
                    if(i == 0)
                    {
                        concatlines[i] += "\t\t\t\t";
                    }
                }
            }

            File.WriteAllLines(dir + "Analysis " + objectives + " objectives.txt", concatlines.ToArray());
        }

        static private string GetFileName(int objectives, int items, UserConstraintsMethod method, int constraints)
        {
            string fileName = "";

            fileName += objectives.ToString("'O'00") + items.ToString("'I'000");

            switch (method)
            {
                case UserConstraintsMethod.NotConsidered:
                    fileName += "N";
                    break;
                case UserConstraintsMethod.DefaultConstraint:
                    fileName += "D";
                    break;
                case UserConstraintsMethod.SoftConstraint:
                    fileName += "S";
                    break;
                case UserConstraintsMethod.ObjectiveFunction:
                    fileName += "O";
                    break;
                default:
                    break;
            }

            fileName += constraints.ToString("'C'00");

            fileName += ".txt";
            return fileName;
        }
    }
}
