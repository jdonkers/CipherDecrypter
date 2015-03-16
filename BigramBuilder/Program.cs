namespace BigramBuilder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Utility program to create a dictionary of Bigrams from a specified piece of text.
    /// </summary>
    public class Program
    {
        public static Dictionary<string, int> Bigrams;

        public static void Main(string[] args)
        {
            string fileContents;
            string Bigram;
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
            Bigrams = new Dictionary<string, int>();

            for (int x = 0; x < fileContents.Length - 2; x++)
            {
                Bigram = fileContents.Substring(x, 2);

                if (Bigrams.ContainsKey(Bigram))
                {
                    Bigrams[Bigram]++;
                }
                else
                {
                    Bigrams.Add(Bigram, 1);
                }
            }

            var sortedBigrams =
                from entry in Bigrams
                orderby entry.Value descending
                select entry;

            using (StreamWriter file = new StreamWriter(args[1]))
            {
                foreach (KeyValuePair<string, int> item in sortedBigrams)
                {
                    file.WriteLine(item.Key + "\t" + (((double)item.Value / (fileContents.Length - 2))).ToString("0.000##########"));
                }
            }

        }
    }
}
