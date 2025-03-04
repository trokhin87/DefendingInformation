using System;
using System.IO;

class Steganography
{
    const int HEADER_SIZE = 54; // Размер заголовка BMP

    static void HideMessage(string bmpPath, string messagePath, string outputBmpPath)
    {
        byte[] bmpBytes = File.ReadAllBytes(bmpPath);
        byte[] messageBytes = File.ReadAllBytes(messagePath);
        byte[] hiddenData = new byte[messageBytes.Length + 1];
        Array.Copy(messageBytes, hiddenData, messageBytes.Length);
        hiddenData[hiddenData.Length - 1] = 0xFF; // Признак конца

        if (HEADER_SIZE + hiddenData.Length * 4 > bmpBytes.Length)
        {
            Console.WriteLine("Ошибка: изображение слишком мало для скрытия сообщения.");
            return;
        }

        for (int i = 0; i < hiddenData.Length; i++)
        {
            int pixelOffset = HEADER_SIZE + i * 4;

            bmpBytes[pixelOffset] &= 0xFC;
            bmpBytes[pixelOffset] |= (byte)((hiddenData[i] >> 6) & 0x03);

            bmpBytes[pixelOffset + 1] &= 0xFC;
            bmpBytes[pixelOffset + 1] |= (byte)((hiddenData[i] >> 4) & 0x03);

            bmpBytes[pixelOffset + 2] &= 0xFC;
            bmpBytes[pixelOffset + 2] |= (byte)((hiddenData[i] >> 2) & 0x03);

            bmpBytes[pixelOffset + 3] &= 0xFC;
            bmpBytes[pixelOffset + 3] |= (byte)(hiddenData[i] & 0x03);
        }

        File.WriteAllBytes(outputBmpPath, bmpBytes);
        Console.WriteLine("Сообщение скрыто.");
    }

    static void ExtractMessage(string bmpPath, string outputMessagePath)
    {
        byte[] bmpBytes = File.ReadAllBytes(bmpPath);
        MemoryStream messageStream = new MemoryStream();

        for (int i = 0; HEADER_SIZE + i * 4 < bmpBytes.Length; i++)
        {
            int pixelOffset = HEADER_SIZE + i * 4;
            byte extractedByte = 0;

            extractedByte |= (byte)((bmpBytes[pixelOffset] & 0x03) << 6);
            extractedByte |= (byte)((bmpBytes[pixelOffset + 1] & 0x03) << 4);
            extractedByte |= (byte)((bmpBytes[pixelOffset + 2] & 0x03) << 2);
            extractedByte |= (byte)(bmpBytes[pixelOffset + 3] & 0x03);

            if (extractedByte == 0xFF)
                break;

            messageStream.WriteByte(extractedByte);
        }

        File.WriteAllBytes(outputMessagePath, messageStream.ToArray());
        Console.WriteLine("Сообщение извлечено.");
    }

    static void Main()
    {
        Console.WriteLine("Выберите режим: 1 - скрыть, 2 - извлечь");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("Введите путь к BMP-файлу: ");
            string bmpPath = Console.ReadLine();
            Console.Write("Введите путь к текстовому файлу: ");
            string messagePath = Console.ReadLine();
            Console.Write("Введите путь для сохранения BMP с сообщением: ");
            string outputBmpPath = Console.ReadLine();

            HideMessage(bmpPath, messagePath, outputBmpPath);
        }
        else if (choice == "2")
        {
            Console.Write("Введите путь к BMP-файлу с сообщением: ");
            string bmpPath = Console.ReadLine();
            Console.Write("Введите путь для сохранения извлеченного сообщения: ");
            string outputMessagePath = Console.ReadLine();

            ExtractMessage(bmpPath, outputMessagePath);
        }
        else
        {
            Console.WriteLine("Неверный выбор.");
        }
    }
}
