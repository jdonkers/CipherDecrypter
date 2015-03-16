namespace CipherDecrypter
{
    using System;

    /// <summary>
    /// Represents a bigram distribution matrix, or in other words; the frequency of two-letter combinations found in a piece of text.
    /// </summary>
    public class BigramDistribution : ICloneable
    {
        /// <summary>
        /// The distribution matrix.
        /// </summary>
        private double[,] distribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="BigramDistribution"/> class.
        /// </summary>
        /// <param name="text">The english text from which to calculate the distribution.</param>
        public BigramDistribution(string text)
        {
            this.distribution = new double[26, 26];
            this.SetDistributionFromText(text);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigramDistribution"/> class.
        /// </summary>
        /// <param name="distribution">A bigram distribution matrix.</param>
        public BigramDistribution(double[,] distribution)
        {
            this.distribution = distribution;
        }

        /// <summary>
        ///  Creates a copy of the <see cref="BigramDistribution"/>
        /// </summary>
        /// <returns>A copy of the <see cref="BigramDistribution"/>.</returns>
        public object Clone()
        {
            return new BigramDistribution((double[,])this.distribution.Clone());
        }

        /// <summary>
        /// Returns a value indicating how close the distribution of bigrams is to the specified bigram distribution.
        /// </summary>
        /// <param name="bigramDistribution">The <see cref="BigramDistribution"/> this being compared to.</param>
        /// <returns>A <see cref="double"/> representing the similarity. Lower is better, with 0 being a perfect match.</returns>
        public double DistributionSimilarity(BigramDistribution bigramDistribution)
        {
            double[,] compareDistribution;
            double score;

            compareDistribution = bigramDistribution.distribution;

            score = 0;

            for (int x = 0; x < 26; x++)
            {
                for (int y = 0; y < 26; y++)
                {
                    score += Math.Abs(this.distribution[x, y] - compareDistribution[x, y]);
                }
            }

            return score;
        }

        /// <summary>
        /// Swaps the specified elements in the bigram distribution matrix.
        /// </summary>
        /// <param name="first">The index of the first element.</param>
        /// <param name="second">The index of the second element.</param>
        public void SwapElements(int first, int second)
        {
            // Swap horiztonally.
            for (int x = 0; x < this.distribution.GetLength(0); x++)
            {
                double swap1 = this.distribution[x, first];
                double swap2 = this.distribution[x, second];

                this.distribution[x, first] = swap2;
                this.distribution[x, second] = swap1;
            }

            // Swap vertically.
            for (int y = 0; y < this.distribution.GetLength(1); y++)
            {
                double swap1 = this.distribution[first, y];
                double swap2 = this.distribution[second, y];

                this.distribution[first, y] = swap2;
                this.distribution[second, y] = swap1;
            }
        }

        /// <summary>
        /// Returns a value indicating how close the distribution of bigrams in the specified text matches this bigram distribution.
        /// </summary>
        /// <param name="text">The text from which to determine the fitness.</param>
        /// <returns>A <see cref="double"/> representing the text fitness. Lower is better, with 0 being a perfect match.</returns>
        public double CalculateFitness(string text)
        {
            BigramDistribution textDistribution;
            textDistribution = new BigramDistribution(text);

            return this.DistributionSimilarity(textDistribution);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string text;
            char[] alphabet;

            text = string.Empty;
            alphabet = "ABCDEFGHHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            for (int i = 0; i < 26; i++)
            {
                text += (char)alphabet[i] + "\t";
            }

            text += "\n";

            for (int i = 0; i < 26; i++)
            {
                text += (char)alphabet[i] + "\t";

                for (int j = 0; j < 26; j++)
                {
                    text += this.distribution[i, j].ToString("0.###") + '\t';
                }

                text += '\n';
            }

            return text;
        }

        /// <summary>
        /// Calculates the bigram distribution matrix from the specified text.
        /// </summary>
        /// <param name="text">The text this method is acting upon.</param>
        private void SetDistributionFromText(string text)
        {
            for (int x = 0; x < text.Length - 1; x++)
            {
                this.distribution[text[x] - 'A', text[x + 1] - 'A']++;
            }

            // Normalize the frequencies.
            for (int x = 0; x < 26; x++)
            {
                for (int y = 0; y < 26; y++)
                {
                    this.distribution[x, y] = this.distribution[x, y] / (text.Length - 1);
                }
            }
        }
    }
}