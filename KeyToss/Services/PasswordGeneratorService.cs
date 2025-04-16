using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeyToss.Services
{
    internal class PasswordGeneratorService
    {
        //Generated password is hardcoded at 16 characters and used every charater in the string chars
        //RandomNumberGenerator is considered a safe way to generate random numbers
        public string GeneratePassword()
        {
            const int passwordLength = 16;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            char[] password = new char[passwordLength];

            for (int i = 0; i < passwordLength; i++)
            {
                int index = RandomNumberGenerator.GetInt32(chars.Length);
                password[i] = chars[index];
            }

            return new string(password);
        }
    }
}
