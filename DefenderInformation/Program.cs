using System.Numerics;
using System.Security.Cryptography;

namespace DefenderInformation
{
    public class Program
    {
        static void Main(string[] args)
        {
            RSA rsa = new RSA(61, 53);
            string message = "hello";
            BigInteger[] encrypt = rsa.Encrypt(message);
            foreach (BigInteger b in encrypt)
            {
                Console.Write(b+" ");
            }
            Console.WriteLine();
            string message2=rsa.Decrypt(encrypt);
            Console.WriteLine(message2);
        }
       
    }
}
