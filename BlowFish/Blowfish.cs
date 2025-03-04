using System;
using System.Text;

public class Blowfish
{
    private readonly uint[] P;
    private readonly uint[,] S;
    private uint[] roundkeys;

    public Blowfish(uint[] pArray, uint[,] sBoxes, byte[] key)
    {
        P = (uint[])pArray.Clone();
        S = (uint[,])sBoxes.Clone();
        roundkeys = GenerateKeys(key);
    }

    private uint F(uint x)
    {
        return ((S[0, (x >> 24) & 0xFF] + S[1, (x >> 16) & 0xFF]) ^ S[2, (x >> 8) & 0xFF]) + S[3, x & 0xFF];
    }

    public uint[] GenerateKeys(byte[] key)
    {
        uint[] roundKeys = new uint[18];
        int keyLen = key.Length;
        int j = 0;

        for (int i = 0; i < 18; i++)
        {
            uint data = 0;
            for (int k = 0; k < 4; k++)
            {
                data = (data << 8) | key[j];
                j = (j + 1) % keyLen;
            }
            roundKeys[i] = P[i] ^ data;
        }

        return roundKeys;
    }

    // Прямое шифрование сообщения в виде блоков ulong
    public ulong[] ShifrMessage(ulong[] msg)
    {
        ulong[] encrypted = new ulong[msg.Length];
        for (int i = 0; i < msg.Length; i++)
        {
            encrypted[i] = ShifrPart(msg[i]);
        }
        return encrypted;
    }

    private ulong ShifrPart(ulong msg)
    {
        uint left = (uint)(msg >> 32);
        uint right = (uint)msg;

        for (int i = 0; i < 16; i++)
        {
            left ^= roundkeys[i];
            right ^= F(left);

            // Обмен
            (left, right) = (right, left);
        }

        // Последний обмен не нужен, поэтому меняем обратно
        (left, right) = (right, left);

        right ^= roundkeys[16];
        left ^= roundkeys[17];

        return ((ulong)left << 32) | right;
    }

    // Прямое дешифрование сообщения в виде блоков ulong
    public ulong[] DecryptMessage(ulong[] msg)
    {
        ulong[] decrypted = new ulong[msg.Length];
        for (int i = 0; i < msg.Length; i++)
        {
            decrypted[i] = DecryptPart(msg[i]);
        }
        return decrypted;
    }

    private ulong DecryptPart(ulong msg)
    {
        uint left = (uint)(msg >> 32);
        uint right = (uint)msg;

        for (int i = 17; i > 1; i--)
        {
            left ^= roundkeys[i];
            right ^= F(left);

            // Обмен
            (left, right) = (right, left);
        }

        // Последний обмен не нужен, меняем обратно
        (left, right) = (right, left);

        right ^= roundkeys[1];
        left ^= roundkeys[0];

        return ((ulong)left << 32) | right;
    }
}
