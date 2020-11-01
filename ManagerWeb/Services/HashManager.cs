using System;
using System.Security.Cryptography;
using System.Text;

namespace ManagerWeb.Services
{
    internal class HashManager : IHashManager
    {
        public string HashPasswordToSha512(string password)
        {
            using SHA512 shaM = new SHA512Managed();
            byte[] hashedBytes = shaM.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
        }
    }
}
