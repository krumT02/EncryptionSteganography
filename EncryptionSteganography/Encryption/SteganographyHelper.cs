using Microsoft.Win32;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

class SteganographyHelper
{


    public static void EmbedMessageInImage(WriteableBitmap image, BitArray messageBits)
    {
        int index = 0; // Индекс за текущия бит от съобщението
        int stride = image.PixelWidth * 4; // 4 bytes per pixel for BGRA32 format
        byte[] pixels = new byte[image.PixelHeight * stride];
        image.CopyPixels(pixels, stride, 0);

        // Convert the message length to bits and store it in the first 16 pixels
        int messageLength = messageBits.Length;
        BitArray lengthBits = new BitArray(BitConverter.GetBytes(messageLength));
        for (int i = 0; i < 16; i++)
        {
            int byteIndex = i * 4; // Конвертираме индекса на пиксела в индекса на байта
            pixels[byteIndex] = SetLSB(pixels[byteIndex], lengthBits[i]);
        }

        // Embed the actual message after the metadata about its length
        index = 64; // Skip the first 64 bytes (16 pixels * 4 bytes per pixel)
        for (int bitIndex = 0; bitIndex < messageBits.Length; bitIndex++)
        {
            if (index < pixels.Length)
            {
                pixels[index] = SetLSB(pixels[index], messageBits[bitIndex]);
                index += 4; // Move to the next pixel's blue component
            }
            else
            {
                // Optional: Handle the case where the image is too small to hold the message
                break;
            }
        }

        // Write the modified pixels back to the WriteableBitmap
        image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), pixels, stride, 0);
    }
    public static byte SetLSB(byte originalByte, bool bitValue)
    {
        // Clear the LSB of the original byte
        byte clearedLSB = (byte)(originalByte & 0xFE); // 0xFE = 1111 1110
                                                       // Set the LSB to the bitValue
        byte newByte = (byte)(clearedLSB | (bitValue ? 1 : 0));
        return newByte;
    }
    public static void SaveImage(WriteableBitmap image)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "PNG Image|*.png";
        if (saveFileDialog.ShowDialog() == true)
        {
            using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
            }
        }

    }
    public static BitArray ExtractMessageFromImage(WriteableBitmap image)
    {
        int stride = image.PixelWidth * 4; // 4 bytes per pixel for BGRA32 format
        byte[] pixels = new byte[image.PixelHeight * stride];
        image.CopyPixels(pixels, stride, 0);

        // Extract the length of the message from the first 16 pixels
        byte[] lengthBytes = new byte[4];
        BitArray lengthBits = new BitArray(16);
        for (int i = 0; i < 16; i++)
        {
            int byteIndex = i * 4; // Convert pixel index to byte index
            lengthBits[i] = GetLSB(pixels[byteIndex]);
        }
        lengthBits.CopyTo(lengthBytes, 0);
        int messageLength = BitConverter.ToInt32(lengthBytes, 0);

        // Prepare to extract the actual message
        BitArray messageBits = new BitArray(messageLength);
        int index = 64; // Skip the first 64 bytes (16 pixels * 4 bytes per pixel)
        for (int bitIndex = 0; bitIndex < messageLength; bitIndex++)
        {
            if (index < pixels.Length)
            {
                messageBits[bitIndex] = GetLSB(pixels[index]);
                index += 4; // Move to the next pixel's blue component
            }
            else
            {
                // Optional: Handle the case where the image does not contain the expected amount of data
                break;
            }
        }

        return messageBits;
    }

    // Helper method to get the least significant bit of a byte
    public static bool GetLSB(byte b)
    {
        return (b & 1) == 1;
    }







    public static Byte[] ConvertBitArrayToString(BitArray bits)
    {
        // Calculate the number of bytes needed to store the bits
        int numBytes = bits.Length / 8;
        if (bits.Length % 8 != 0) numBytes++; // Add an extra byte for remaining bits

        // Convert the BitArray to an array of bytes
        byte[] bytes = new byte[numBytes];
        bits.CopyTo(bytes, 0);

        // Convert the bytes array to a string using UTF8 encoding
        return bytes;
    }
}