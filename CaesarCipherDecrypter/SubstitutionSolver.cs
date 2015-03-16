namespace CipherDecrypter
{
    /// <summary>
    /// Base class for substitution substitution algorithms.
    /// </summary>
    public abstract class SubstitutionSolver
    {
        /// <summary>
        /// Returns a best guess at the cipher key for the specified text.
        /// </summary>
        /// <param name="text">The text this method is acting upon.</param>
        /// <returns>A <see cref="CipherKey"/> representing the key to the cipher.</returns>
        public abstract CipherKey Solve(string text);
    }
}