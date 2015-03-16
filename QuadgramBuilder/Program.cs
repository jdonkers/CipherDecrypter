namespace QuadgramBuilder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Utility program to create a dictionary of Quadgrams from a specified piece of text.
    /// </summary>
    public class Program
    {
        public static Dictionary<string, int> quadgrams;

        public static void Main(string[] args)
        {
            string fileContents;
            string quadgram;
            Regex regex;

            if (args.Count() < 2)
            {
                Console.WriteLine("File name and output file name required.");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Could not find file.");
                return;
            }

            fileContents = File.ReadAllText(args[0]);
            regex = new Regex("[^A-Z]");
            fileContents = regex.Replace(fileContents.ToUpper(), string.Empty);

            quadgrams = new Dictionary<string, int>();

            for (int x = 0; x < fileContents.Length - 4; x++)
            {
                quadgram = fileContents.Substring(x, 4);

                if (quadgrams.ContainsKey(quadgram))
                {
                    quadgrams[quadgram]++;
                }
                else
                {
                    quadgrams.Add(quadgram, 1);
                }
            }

            var sortedQuadgrams =
                from entry in quadgrams
                orderby entry.Value descending
                select entry;

            using (StreamWriter file = new StreamWriter(args[1]))
            {
                foreach (KeyValuePair<string, int> item in sortedQuadgrams)
                {
                    file.WriteLine(item.Key + "\t" + Math.Log(((double)item.Value / quadgrams.Count())));
                }
            }
        }
    }
}