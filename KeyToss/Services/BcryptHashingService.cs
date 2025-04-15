using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyToss.Services
{
    //
    //Need to install the BCrypt.Net NuGet package to use this class.
    //BCrypt.Net-Next
    //This class provides methods for hashing and verifying passwords using the BCrypt algorithm.
    //
    public class BcryptHashingService
    {
        /// Hash a plain text string using BCrypt.
        public string HashPassword(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        /// Verify a plain text string against a hash.
        public bool VerifyPassword(string plainText, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, hashedPassword);
        }
    }
}
