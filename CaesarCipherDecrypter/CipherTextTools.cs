namespace CipherDecrypter
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Contains static methods for working ciphers.
    /// </summary>
    public static class CipherTextTools
    {
        /// <summary>
        /// Gives a rough percentage estimate of how similar the specified text is to english. Inaccurate for strings under 200 characters.
        /// </summary>
        /// <param name="text">The text this method is acting upon.</param>
        /// <returns>A <see cref="double"/> representing the fitness as a percent.</returns>
        public static double DetermineConfidence(string text)
        {
            Dictionary<string, double> quadgrams;
            QuadgramDistribution quadgramFitness;
            double fitness;

            text = CipherTextTools.RemoveNonAlphaCharacters(text);
            quadgrams = CipherTextTools.LoadQuadgrams(Properties.Resources.quadgrams);
            quadgramFitness = new QuadgramDistribution(quadgrams);

            fitness = quadgramFitness.CalculateFitness(text);

            if (text.Length < 150)
            {
                fitness += (220 - text.Length) / 12;
            }

            fitness = (Math.Pow(fitness, 3) + 850) / Math.Pow(fitness, 4);

            if (fitness > 1)
            {
                fitness = 1;
            }

            return fitness;
        }

        /// <summary>
        /// Loads bigrams frequencies from the specified text and returns the values as a matrix.
        /// </summary>
        /// <param name="text">A <see cref="string"/> containing the bigrams, where each bigram is on a newline.</param>
        /// <returns>A bigram distribution matrix.</returns>
        public static double[,] LoadBigrams(string text)
        {
            double[,] bigrams;

            bigrams = new double[26, 26];

            foreach (string line in text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = line.Split('\t');

                bigrams[parts[0][0] - 'A', parts[0][1] - 'A'] = Convert.ToDouble(parts[1]);
            }

            return bigrams;
        }

        /// <summary>
        /// Loads a dictionary quadgrams from the specified text.
        /// </summary>
        /// <param name="text">A <see cref="string"/> containing the quadgrams, where each quadgram is on a newline.</param>
        /// <returns>A dictionary of quadgram to quadgram frequency pairs.</returns>
        public static Dictionary<string, double> LoadQuadgrams(string text)
        {
            Dictionary<string, double> quadgrams;
            string[] parts;

            quadgrams = new Dictionary<string, double>();

            foreach (string line in text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                parts = line.Split('\t');
                quadgrams.Add(parts[0], Convert.ToDouble(parts[1]));
            }

            return quadgrams;
        }

        /// <summary>
        /// Strips all non-alphanumeric characters from the specified text.
        /// </summary>
        /// <param name="text">The text this method is acting upon.</param>
        /// <returns>The specified text converted to uppercase, and non-alphanumeric characters remove.</returns>
        public static string RemoveNonAlphaCharacters(string text)
        {
            Regex regex = new Regex("[^A-Z]");

            return regex.Replace(text.ToUpper(), string.Empty);
        }
    }
}