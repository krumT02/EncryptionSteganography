using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

class SteganographyHelper
{
    public enum State
    {
        Hiding,
        Filling_With_Zeros
    };

    public static WriteableBitmap EmbedText(string text, WriteableBitmap writeableBmp)
    {
        // Конвертираме текста в масив от битове
        BitArray textBits = new BitArray(Encoding.UTF8.GetBytes(text));

        int index = 0;
        int stride = writeableBmp.PixelWidth * (writeableBmp.Format.BitsPerPixel / 8); // Пресмятаме stride на изображението
        byte[] pixels = new byte[writeableBmp.PixelHeight * stride];
        writeableBmp.CopyPixels(pixels, stride, 0);

        for (int i = 0; i < textBits.Length; i++)
        {
            // Получаваме текущия бит от textBits
            bool bit = textBits[i];

            // Запазваме бита в последния бит на съответния цветов компонент
            // Алтернативно, може да изберете да запишете само в един от цветовете, например само в синия (B)
            if (i % 3 == 0)
            {
                // Записваме в синия канал
                pixels[index] = SetLSB(pixels[index], bit);
            }
            else if (i % 3 == 1)
            {
                // Записваме в зеления канал
                pixels[index + 1] = SetLSB(pixels[index + 1], bit);
            }
            else if (i % 3 == 2)
            {
                // Записваме в червения канал
                pixels[index + 2] = SetLSB(pixels[index + 2], bit);
                index += 4; // Минаваме към следващия пиксел след всеки трети бит
            }

            // Уверете се, че не излизаме извън границите на масива
            if (index >= pixels.Length - 3)
            {
                break;
            }
        }

        // Записваме променените пиксели обратно в writeableBmp
        writeableBmp.WritePixels(new Int32Rect(0, 0, writeableBmp.PixelWidth, writeableBmp.PixelHeight), pixels, stride, 0);

        return writeableBmp;
    }

    private static byte SetLSB(byte byteValue, bool bit)
    {
        // Задаваме най-малко значимия бит (LSB) на byteValue според стойността на bit
        return bit ? (byte)(byteValue | 1) : (byte)(byteValue & ~1);
    }


    public static string ExtractText(WriteableBitmap bmp)
    {
        int colorUnitIndex = 0;
        int bitIndex = 0;
        byte[] bytes = new byte[bmp.PixelWidth * bmp.PixelHeight];
        int stride = bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8);
        byte[] pixels = new byte[bmp.PixelHeight * stride];
        bmp.CopyPixels(pixels, stride, 0);
        BitArray bits = new BitArray(bmp.PixelWidth * bmp.PixelHeight * 3);

        for (int i = 0; i < pixels.Length; i += 4)
        {
            for (int n = 0; n < 3; n++)
            {
                int shift = (i % 4) * 8;
                bool bit = (pixels[i + n] & 1) == 1;
                bits.Set(bitIndex, bit);
                bitIndex++;
            }
        }

        for (int i = 0; i < bits.Length; i += 8)
        {
            if (i + 8 > bits.Length) break;

            byte b = ConvertToByte(bits, i);
            if (b == 0) break;

            bytes[colorUnitIndex] = b;
            colorUnitIndex++;
        }

        return Encoding.UTF8.GetString(bytes, 0, colorUnitIndex);
    }

    private static byte ConvertToByte(BitArray bits, int startIndex)
    {
        byte b = 0;
        for (int i = 0; i < 8; i++)
        {
            if (bits[startIndex + i])
            {
                b |= (byte)(1 << (7 - i));
            }
        }
        return b;
    }


    public static int reverseBits(int n)
    {
        int result = 0;

        for (int i = 0; i < 8; i++)
        {
            result = result * 2 + n % 2;

            n /= 2;
        }

        return result;
    }
}