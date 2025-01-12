using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace BibliotecaAPI.Services {
    public class HashService {
        public ResultadoHash Hash ( string texto_plano ) {
            var sal = new byte [ 16 ];

            using ( var random = RandomNumberGenerator.Create() ) {
                random.GetBytes ( sal );
            }

            return Hash ( texto_plano, sal );
        }

        public ResultadoHash Hash ( string texto_plano, byte [] sal ) {
            var llave_derivada = KeyDerivation.Pbkdf2 (
                password: texto_plano,
                salt: sal,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32
            );

            var hash = Convert.ToBase64String ( llave_derivada );

            return new ResultadoHash() {
                Hash = hash,
                Sal = sal
            };
        }
    }
}
