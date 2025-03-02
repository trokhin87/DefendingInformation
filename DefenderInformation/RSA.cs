using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DefenderInformation
{
    public class RSA
    {
        private BigInteger p;
        private BigInteger q;
        private BigInteger n;
        private BigInteger phi;
        private BigInteger e;
        private BigInteger d;

        public RSA(BigInteger p, BigInteger q)
        {
            this.p = p;
            this.q = q;
            n = p * q;
            phi = (p - 1) * (q - 1);

            e = 65537; // Стандартный выбор e
            if (BigInteger.GreatestCommonDivisor(e, phi) != 1)
            {
                throw new Exception("Выбранное e не взаимно просто с phi!");
            }

            d = ModInverse(e, phi);
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m, t, q;
            BigInteger x0 = 0, x1 = 1;

            while (a > 1)
            {
                q = a / m;
                t = m;
                m = a % m;
                a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            return x1 < 0 ? x1 + m0 : x1;
        }

        public BigInteger[] Encrypt(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            BigInteger[] encrypted = new BigInteger[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                encrypted[i] = BigInteger.ModPow(bytes[i], e, n);
            }

            return encrypted;
        }

        public string Decrypt(BigInteger[] encrypted)
        {
            byte[] bytes = new byte[encrypted.Length];

            for (int i = 0; i < encrypted.Length; i++)
            {
                BigInteger decrypted = BigInteger.ModPow(encrypted[i], d, n);

                if (decrypted < 0 || decrypted > 255)
                {
                    throw new Exception("Ошибка: декодированный символ выходит за пределы ASCII!");
                }

                bytes[i] = (byte)decrypted;
            }

            return Encoding.ASCII.GetString(bytes);
        }
    }
}
