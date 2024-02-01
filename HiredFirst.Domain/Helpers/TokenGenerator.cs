using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HiredFirst.Domain.Helpers
{
    public static class TokenGenerator
    {
        public static string GenerateUniqueToken(int length = 32)
        {
            // Define allowed characters for the token
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Create a cryptographic random number generator
            using (var rng = new RNGCryptoServiceProvider())
            {
                // Create a byte array to store the random values
                byte[] randomBytes = new byte[length];

                // Fill the byte array with random values
                rng.GetBytes(randomBytes);

                // Create a char array to store the token characters
                char[] chars = new char[length];

                // Convert the random bytes to characters from the allowedChars array
                for (int i = 0; i < length; i++)
                {
                    int index = randomBytes[i] % allowedChars.Length;
                    chars[i] = allowedChars[index];
                }

                // Convert the char array to a string and return the token
                return new string(chars);
            }
        }
    }
}
