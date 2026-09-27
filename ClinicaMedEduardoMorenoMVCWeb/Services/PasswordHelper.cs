using System;
using System.Security.Cryptography;

namespace ClinicaMedEduardoMorenoMVCWeb.Services
{
    public static class PasswordHelper
    {
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            // Soporte si está en texto plano
            if (enteredPassword == storedHash)
                return true;

            // Formato estándar: PBKDF2$iteraciones$salt$hash
            var parts = storedHash.Split('$');
            if (parts.Length == 4 && parts[0].Equals("PBKDF2", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(parts[1], out int iterations))
                {
                    try
                    {
                        byte[] salt = Convert.FromBase64String(parts[2]);
                        byte[] expectedHash = Convert.FromBase64String(parts[3]);

                        // Probar primero con SHA256 (el más común para 32 bytes)
                        byte[] actualHash256 = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
                        if (CryptographicOperations.FixedTimeEquals(actualHash256, expectedHash))
                            return true;

                        // Probar también con SHA1 por compatibilidad si es un hash antiguo de .NET Framework
                        byte[] actualHash1 = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, salt, iterations, HashAlgorithmName.SHA1, expectedHash.Length);
                        if (CryptographicOperations.FixedTimeEquals(actualHash1, expectedHash))
                            return true;

                        // Probar también con SHA512
                        byte[] actualHash512 = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, salt, iterations, HashAlgorithmName.SHA512, expectedHash.Length);
                        if (CryptographicOperations.FixedTimeEquals(actualHash512, expectedHash))
                            return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
