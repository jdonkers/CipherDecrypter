namespace CipherDecrypter
{
    using System;

    /// <summary>
    /// Implementation of Thomas Jakobsen's fast method for the Cryptanalysis of Substitution Ciphers
    /// </summary>
    public class JakobsensSolver : SubstitutionSolver
    {
        /// <summary>
        /// Represents the alphabet with letters ordered by frequency of use in english.
        /// </summary>
        private static char[] frequencyAlphabet = "ETAOINSHRDLCUMWFGYPBVKJXQZ".ToCharArray();

        /// <summary>
        /// After performing the primary attack, the number of random permutations to try.
        /// </summary>
        private static int randomPermutations = 1000;

        /// <summary>
        /// Returns a best guess at the cipher key for the specified text.
        /// </summary>
        /// <param name="text">The text this method is acting upon.</param>
        /// <returns>A <see cref="CipherKey"/> representing the key to the cipher.</returns>
        public override CipherKey Solve(string text)
        {
            // The bigram distribution of the natural english language.
            BigramDistribution naturalDistribution;

            // The bigram distribution of the current working guess.
            BigramDistribution cipherDistribution;

            // The closest bigram distribution found so far.
            BigramDistribution closestDistribution;

            CipherKey key;
            Random random;
            double[,] naturalFrequency;
            double bestScore, score;

            // The initial key is ordered to match letter frequencies with the english language.
            key = new CipherKey();
            key.SetAlphabetFrequency(text);

            naturalFrequency = CipherTextTools.LoadBigrams(Properties.Resources.bigrams);

            naturalDistribution = new BigramDistribution(naturalFrequency);
            closestDistribution = new BigramDistribution(key.DecryptText(text));

            score = closestDistribution.DistributionSimilarity(naturalDistribution);
            bestScore = score;

            // Attempt to improve the key by swapping letters. Closer letters are swapped first.
            for (int i = 1; i <= 26; i++)
            {
                for (int j = 0; j < 26 - i; j++)
                {
                    cipherDistribution = (BigramDistribution)closestDistribution.Clone();

                    int first = j;
                    int second = j + i;

                    int firstIndex = this.ConvertToLetterIndex(first);
                    int secondIndex = this.ConvertToLetterIndex(second);

                    cipherDistribution.SwapElements(firstIndex, secondIndex);
                    score = cipherDistribution.DistributionSimilarity(naturalDistribution);

                    if (score < bestScore)
                    {
                        bestScore = score;
                        closestDistribution = (BigramDistribution)cipherDistribution.Clone();
                        key.SwapLetters(firstIndex, secondIndex);
                    }
                }
            }

            random = new Random();

            // At this point the solutions is sometimes very close, but a few letters are still off.
            // A number of random swaps are attempted to see if the cipher key can be improved.
            for (int i = 1; i <= JakobsensSolver.randomPermutations; i++)
            {
                cipherDistribution = (BigramDistribution)closestDistribution.Clone();

                int firstIndex = random.Next(0, 26);
                int secondIndex = random.Next(0, 26);

                cipherDistribution.SwapElements(firstIndex, secondIndex);
                score = cipherDistribution.DistributionSimilarity(naturalDistribution);

                if (score < bestScore)
                {
                    bestScore = score;
                    closestDistribution = (BigramDistribution)cipherDistribution.Clone();
                    key.SwapLetters(firstIndex, secondIndex);
                }
            }

            return key;
        }

        /// <summary>
        /// Converts a value representing the index of a letter in an frequency-ordered alphabet, to the index of the same letter in a regular alphabet.
        /// </summary>
        /// <param name="i">A <see cref="int"/> representing the index of the letter in the frequency-ordered alphabet.</param>
        /// <returns>A <see cref="int"/> representing the index of the specified letter, but in the natural alphabet.</returns>
        private int ConvertToLetterIndex(int i)
        {
            return frequencyAlphabet[i] - 'A';
        }
    }
}