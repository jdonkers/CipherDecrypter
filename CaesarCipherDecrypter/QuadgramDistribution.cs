namespace CipherDecrypter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a quadgram distribution, or in other words; the frequency of four-letter combinations found in a piece of text.
    /// </summary>
    public class QuadgramDistribution
    {
        /// <summary>
        /// The fitness value for 4 characters of text that do not exist in the quadgram dictionary.
        /// </summary>
        private double quadgramNotFoundFitness;

        /// <summary>
        /// A dictionary of quadgram-frequency pairs.
        /// </summary>
        private Dictionary<string, double> quadgrams;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadgramDistribution"/> class.
        /// </summary>
        /// <param name="quadgrams">A dictionary of quadgram-frequency pairs.</param>
        public QuadgramDistribution(Dictionary<string, double> quadgrams)
        {
            this.quadgrams = quadgrams;
            this.quadgramNotFoundFitness = Math.Log(0.7 / quadgrams.Count);
        }

        /// <summary>
        /// Returns a value indicating how close the distribution of quadgrams in the specified text matches this quadgrams distribution.
        /// </summary>
        /// <param name="text">The text from which to determine the fitness.</param>
        /// <returns>A <see cref="double"/> representing the text fitness. 5.5 is a very high fitness, anything over 10 is a low fitness.</returns>
        public double CalculateFitness(string text)
        {
            double fitness;
            string quadgram;

            text = CipherTextTools.RemoveNonAlphaCharacters(text);

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(text);
            }

            fitness = 0.0;

            for (int x = 0; x < text.Length - 4; x++)
            {
                quadgram = text.Substring(x, 4);

                if (!this.quadgrams.ContainsKey(quadgram))
                {
                    fitness += this.quadgramNotFoundFitness;
                }
                else
                {
                    fitness += this.quadgrams[quadgram];
                }
            }

            return -fitness / text.Length;
        }
    }
}