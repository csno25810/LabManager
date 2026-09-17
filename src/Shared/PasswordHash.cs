using System;
using System.Security.Cryptography;

namespace LabCommon
{
    public static class PasswordHash
    {
        public const string DefaultPassword = "lab2026";

        public static string Hash(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return "pbkdf2$10000$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored))
                return false;
            string[] parts = stored.Split('$');
            if (parts.Length != 4 || parts[0] != "pbkdf2")
                return false;

            int iterations;
            if (!int.TryParse(parts[1], out iterations) || iterations < 1)
                return false;

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch
            {
                return false;
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] actual = pbkdf2.GetBytes(expected.Length);
                return SlowEquals(expected, actual);
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            int n = Math.Min(a.Length, b.Length);
            for (int i = 0; i < n; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
