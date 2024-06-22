namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Interface for a service that provides hashing functionality.
    /// </summary>
    public interface IHashManager
    {
        /// <summary>
        /// Hashes a password using the SHA-512 algorithm.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>A string representing the hashed password in SHA-512 format.</returns>
        string HashPasswordToSha512(string password);
    }

}
