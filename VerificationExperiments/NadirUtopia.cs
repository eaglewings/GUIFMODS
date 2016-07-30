using MainApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationExperiments
{
    public class NadirUtopia
    {
        static readonly int[] ITEMS = { 20, 200, 500 };
        static readonly int[] NR_OBJECTIVES = { 3, 5, 7 };
        static readonly string[] METHOD_CODE = {"None", "DC", "SC" };

        static readonly UserConstraintsMethod[] METHODS = { UserConstraintsMethod.NotConsidered, UserConstraintsMethod.DefaultConstraint, UserConstraintsMethod.SoftConstraint };

        static public void Write()
        {
            List<string> paths = new List<string>();
            string[] header = new string[3];
            List<string> concatLines = new List<string>();
            List<FileStream> files = new List<FileStream>();
            List<string[]> lines = new List<string[]>();

            string dir = @"C:\Users\Benjamin\Dropbox\FHNW\Master Thesis\Experiments\P 0100 - Max 020K - Intervall 10002016-07-16T15-48-59\";
            int objectives = 3;

            header[0] = "%Items";
            header[1] = "%population size 100, 20000 Evaluations, O0 Min=0.33n O1 Min=0.33n";
            header[2] = "c1";

            int colNr = 1;
            foreach (var items in ITEMS)
            {
                foreach (var met in METHODS)
                {
                    header[0] += "\t" + items + "\t" + items;
                    header[1] += "\tNadir " + METHOD_CODE[(int)met] + "\tUtopia" + METHOD_CODE[(int)met];
                    header[2] += "\tc" + ++colNr + "\tc" + ++colNr;
                    paths.Add(dir + GetFileName(objectives, items, met, 2));
                }
            }
            

            foreach (var path in paths)
            {
                string[] allLines = File.ReadAllLines(path);
                int concatLineNr = 0;
                for (int i = allLines.Length-objectives; i < allLines.Length; i++)
                {
                    string[] splitline = allLines[i].Split(new char[] { '\t' });
                    string nadir = splitline[1];
                    string utopia = splitline[2];
                    if(concatLineNr >= concatLines.Count)
                    {
                        concatLines.Add(@"{Objective " + concatLineNr + @"}");
                    }
                    concatLines[concatLineNr] = concatLines[concatLineNr] + "\t" + nadir.Remove(0,1) + "\t" + utopia.Remove(0, 1);
                    concatLineNr++;
                }
            }
            File.WriteAllLines(dir + "O" + objectives + "UtopiaNadir.txt", header);
            File.AppendAllLines(dir + "O"+objectives + "UtopiaNadir.txt", concatLines.ToArray());
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

            fileName += "_fronts.txt";
            return fileName;
        }
    }
}
