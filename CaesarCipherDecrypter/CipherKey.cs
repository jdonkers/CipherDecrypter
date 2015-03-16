namespace CipherDecrypter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a key to a simple substitution cipher.
    /// </summary>
    public class CipherKey : ICloneable
    {
        /// <summary>
        /// The cipher alphabet, in the order of the natural alphabet.
        /// </summary>
        private char[] cipherAlphabet;

        /// <summary>
        /// A reverse mapping of the cipher alphabet.
        /// </summary>
        private char[] reverseAlphabet;

        /// <summary>
        /// Initializes a new instance of the <see cref="CipherKey"/> class.
        /// </summary>
        public CipherKey()
        {
            this.cipherAlphabet = new char[26];
            this.reverseAlphabet = new char[26];
        }

        /// <summary>
        ///  Creates a copy of the <see cref="BigramDistribution"/>
        /// </summary>
        /// <returns>A copy of the <see cref="BigramDistribution"/>.</returns>
        public object Clone()
        {
            CipherKey cipherKey;

            cipherKey = new CipherKey();
            cipherKey.cipherAlphabet = this.cipherAlphabet.Clone() as char[];
            cipherKey.reverseAlphabet = this.reverseAlphabet.Clone() as char[];

            return cipherKey;
        }

        /// <summary>
        /// Uses the key to decrypt the specified letter.
        /// </summary>
        /// <param name="letter">The letter to decrypt.</param>
        /// <returns>A <see cref="char"/> containing the decrypted char.</returns>
        public char DecryptLetter(char letter)
        {
            return reverseAlphabet[letter - 'A'];

            /*// Naive implementation. Possible performance bottleneck.
            for (int i = 0; i < 26; i++)
            {
                if (this.cipherAlphabet[i] == letter)
                {
                    return (char)(i + 'A');
                }
            }*/

            //return '\0';
        }

        /// <summary>
        /// Uses the key to decrypt the specified text.
        /// </summary>
        /// <param name="text">The text to decrypt.</param>
        /// <returns>A <see cref="string"/> containing the decrypted text.</returns>
        public string DecryptText(string text)
        {
            string translatedText;

            text = text.ToUpper();
            translatedText = string.Empty;

            for (int x = 0; x < text.Length; x++)
            {
                if (char.IsLetter(text[x]))
                {
                    translatedText += reverseAlphabet[text[x] - 'A'];
                }
                else
                {
                    translatedText += text[x];
                }
            }

            return translatedText;
        }

        /// <summary>
        /// Creates a cipher alphabet using a best-guess approach using the letter frequencies in the specified text. Each letters is matched with the letter of corresponding frequency in English.
        /// </summary>
        /// <param name="text">The text from which to determine the letter frequencies.</param>
        public void SetAlphabetFrequency(string text)
        {
            Dictionary<int, int> letterFrequencies;
            char[] naturalAlphabet;

            // The english alphabet ordered by letter frequency.
            naturalAlphabet = "ETAOINSHRDLCUMWFGYPBVKJXQZ".ToCharArray();

            letterFrequencies = new Dictionary<int, int>();

            for (int x = 0; x < 26; x++)
            {
                letterFrequencies.Add(x, text.Count(c => c == (char)('A' + x)));
            }

            int i = 0;
            foreach (KeyValuePair<int, int> item in letterFrequencies.OrderByDescending(key => key.Value))
            {
                this.cipherAlphabet[naturalAlphabet[i] - 'A'] = (char)(item.Key + 'A');
                this.reverseAlphabet[item.Key] = naturalAlphabet[i];
                i++;
            }
        }

        /// <summary>
        /// Randomly swaps two letters in the key.
        /// </summary>
        /// <param name="random">A <see cref="Random"/> used to randomly select the letters.</param>
        public void Shuffle(Random random)
        {
            int firstIndex = random.Next(0, 26);
            int secondIndex = random.Next(0, 26);
            this.SwapLetters(firstIndex, secondIndex);
        }

        /// <summary>
        /// Swaps two letters in the key.
        /// </summary>
        /// <param name="first">The index of the first letter.</param>
        /// <param name="second">The index of the second letter.</param>
        public void SwapLetters(int first, int second)
        {
            char firstLetter = this.cipherAlphabet[first];
            char secondLetter = this.cipherAlphabet[second];

            this.cipherAlphabet[first] = secondLetter;
            this.cipherAlphabet[second] = firstLetter;

            // Find and swap the same letters in the reverse alphabet.
            int firstIndex = 0;
            for (int x = 0; x < 26; x++)
            {
                if (this.reverseAlphabet[x] == firstLetter)
                {
                    firstIndex = x;
                    break;
                }
            }

            int secondIndex = 0;
            for (int x = 0; x < 26; x++)
            {
                if (this.reverseAlphabet[x] == secondLetter)
                {
                    secondIndex = x;
                    break;
                }
            }

            firstLetter = this.reverseAlphabet[firstIndex];
            secondLetter = this.reverseAlphabet[secondIndex];

            this.reverseAlphabet[firstIndex] = secondLetter;
            this.reverseAlphabet[secondIndex] = firstLetter;
        }
    }
}