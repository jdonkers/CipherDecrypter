namespace CipherDecrypter
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Hill-climbing approach to solving substitution ciphers. Quadgrams are used to test fitness.
    /// </summary>
    public class QuadgramHillClimbSolver : SubstitutionSolver
    {
        /// <summary>
        /// The number of random key shuffles to make searching for a key.
        /// </summary>
        private static int keyShuffles = 5000;

        /// <summary>
        /// The number of shuffles to give up
        /// </summary>
        private static int giveUpAfter = 500;

        /// <summary>
        /// The number of attempts to make to find a key. 
        /// </summary>
        private static int attempts = 3;
        
        /// <summary>
        /// Returns a best guess at the cipher key for the specified text.
        /// </summary>
        /// <param name="text">The text this method is acting upon.</param>
        /// <returns>A <see cref="CipherKey"/> representing the key to the cipher.</returns>
        public override CipherKey Solve(string text)
        {
            CipherKey bestKey;
            Dictionary<string, double> quadgrams;
            QuadgramDistribution quadgramFitness;
            Random random;
            double score, bestScore;

            quadgrams = CipherTextTools.LoadQuadgrams(Properties.Resources.quadgrams);
            quadgramFitness = new QuadgramDistribution(quadgrams);

            // Any text over a certain length has a cost on performance with little effect on the results.
            if (text.Length > 1500)
            {
                text = text.Substring(0, 1500);
            }

            bestKey = null;
            random = new Random();

            bestScore = double.MaxValue;

            // A search is executed several times, with the best attempt being kept.
            Parallel.For(1, QuadgramHillClimbSolver.attempts, (x) =>
            {
                string newText = (string)text.Clone();
                CipherKey workingKey = new CipherKey();
                workingKey.SetAlphabetFrequency(newText);

                // The key is increasingly shuffled each attempt to avoid local maximums in the case where the alphabet frequency is a poor guess.
                for (int y = 0; y < (x * 2) - 1; y++)
                {
                    workingKey.Shuffle(random);
                }

                workingKey = this.FindKey(workingKey, quadgramFitness, newText);
                score = quadgramFitness.CalculateFitness(workingKey.DecryptText(newText));

                lock (this)
                {
                    if (score < bestScore || bestKey == null)
                    {
                        bestKey = workingKey;
                        bestScore = score;

                        // Anything below 5.7 is very close to the english language, and can be assumed to be correct.
                        if (bestScore < 5.7)
                        {
                            return;
                        }
                    }
                }
            });

            return bestKey;
        }

        /// <summary>
        /// Performs a search for the best CipherKey for the specified text.
        /// </summary>
        /// <param name="key">A <see cref="CipherKey"/> representing the starting point for the search, and will contain the best key found when this method returns.</param>
        /// <param name="fitness">A <see cref="QuadgramDistribution"/> representing the ideal quadgram distribution.</param>
        /// <param name="text">The text this method is acting upon.</param>
        /// <returns>A <see cref="double"/> representing the fitness of the key.</returns>
        private CipherKey FindKey(CipherKey key, QuadgramDistribution fitness, string text)
        {
            double score, bestScore, sinceLastChange;
            CipherKey workingKey;
            string translatedText;
            Random random;

            bestScore = double.MaxValue;
            sinceLastChange = 0;
            random = new Random();

            int x = 0;
            while (x < QuadgramHillClimbSolver.keyShuffles && sinceLastChange < QuadgramHillClimbSolver.giveUpAfter)
            {
                x++;
                workingKey = (CipherKey)key.Clone();

                workingKey.Shuffle(random);

                translatedText = workingKey.DecryptText(text);
                score = fitness.CalculateFitness(translatedText);

                if (score < bestScore)
                {
                    bestScore = score;
                    key = workingKey;
                    sinceLastChange = 0;
                }

                sinceLastChange++;
            }

            return key;
        }
    }
}
