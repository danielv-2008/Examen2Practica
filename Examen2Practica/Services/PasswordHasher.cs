using System.Security.Cryptography;

namespace Examen2Practica.Services
{
    // Hash de contraseñas con PBKDF2 + salt aleatorio.
    // Nunca se guarda la contraseña en texto plano.
    public static class PasswordHasher
    {
        private const int Iteraciones = 100_000;
        private const int TamanioSalt = 16;
        private const int TamanioHash = 64;

        public static string Hashear(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(TamanioSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanioHash);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                return false;
            }

            var partes = passwordHash.Split('.');
            if (partes.Length != 2)
            {
                return false;
            }

            byte[] salt;
            byte[] hashEsperado;
            try
            {
                salt = Convert.FromBase64String(partes[0]);
                hashEsperado = Convert.FromBase64String(partes[1]);
            }
            catch (FormatException)
            {
                return false;
            }

            var hashObtenido = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanioHash);
            return CryptographicOperations.FixedTimeEquals(hashObtenido, hashEsperado);
        }
    }
}
