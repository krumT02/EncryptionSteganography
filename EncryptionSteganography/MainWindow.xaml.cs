using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using EncryptionSteganography.Encryption;

namespace EncryptionSteganography
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleVisibility_Click(object sender, RoutedEventArgs e)
        {

            // Създайте или използвайте вече създаден TextBox, който ще се показва и скрива.
            if (visiblePasswordTextBox.Visibility == Visibility.Collapsed)
            {
                visiblePasswordTextBox.Visibility = Visibility.Visible; // Покажете текстовото поле
                visiblePasswordTextBox.Text = passwordBox.Password;     // Копирайте паролата в текстовото поле
                passwordBox.Visibility = Visibility.Collapsed; // Скрийте PasswordBox
            }
            else
            {
                passwordBox.Visibility = Visibility.Visible; // Покажете PasswordBox
                passwordBox.Password = visiblePasswordTextBox.Text;         // Копирайте текста обратно в PasswordBox
                visiblePasswordTextBox.Visibility = Visibility.Collapsed;   // Скрийте текстовото поле
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Зареждане на изображението и поставяне на изображението в контролата Image
                displayedImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpeg|JPG Image|*.jpg";
            saveFileDialog.DefaultExt = "png"; // Default file extension
            if (saveFileDialog.ShowDialog() == true)
            {
                // Получаване на BitmapSource от Image контролата
                BitmapSource imageToSave = displayedImage.Source as BitmapSource;

                // Запазване на изображението
                SaveImageToFile(imageToSave, saveFileDialog.FileName);
            }
        }

        private void SaveImageToFile(BitmapSource imageToSave, string filename)
        {
            var encoder = new PngBitmapEncoder(); // Използвайте JpegBitmapEncoder за JPEG файлове
            encoder.Frames.Add(BitmapFrame.Create(imageToSave));

            using (var stream = new FileStream(filename, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEncryptionItem = EncyptionType.SelectedItem as ComboBoxItem;
            var selectedEncryption = selectedEncryptionItem?.Content.ToString();

            var selectedProcessItem = ProcessType.SelectedItem as ComboBoxItem;
            var selectedProcess = selectedProcessItem?.Content.ToString();
            

            if (selectedProcess == "Inclusion Message")
            {
                var encrypted = Encryptions.EncryptMessage(MessageTextBox.Text, passwordBox.Password, selectedEncryption);
                var sad = MessageTextBox.Text ;
                
                byte[] sadd = System.Text.Encoding.UTF8.GetBytes(sad);
                sad.ToArray();
                
                BitArray messageBits = new BitArray(sadd);
                

               
                
                

                // Получаваме WriteableBitmap от изображението в Image контролата
                WriteableBitmap writeableBitmap = new WriteableBitmap((BitmapSource)displayedImage.Source);

                // Вграждаме съобщението в изображението
                EmbedMessageInImage(writeableBitmap, messageBits);

                // Запазваме изображението
                SaveImage(writeableBitmap);
            }
            else if (selectedProcess == "Extraction Message")
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap((BitmapSource)displayedImage.Source);
                var decrypted = ExtractMessageFromImage(writeableBitmap);

                MessageBox.Show(ConvertBitArrayToString(decrypted));
               
            }
        }



        private void EmbedMessageInImage(WriteableBitmap image, BitArray messageBits)
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

        // Helper method to set the least significant bit of a byte
        private byte SetLSB(byte originalByte, bool bitValue)
        {
            // Clear the LSB of the original byte
            byte clearedLSB = (byte)(originalByte & 0xFE); // 0xFE = 1111 1110
                                                           // Set the LSB to the bitValue
            byte newByte = (byte)(clearedLSB | (bitValue ? 1 : 0));
            return newByte;
        }


        private void SaveImage(WriteableBitmap image)
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
        private BitArray ExtractMessageFromImage(WriteableBitmap image)
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
        private bool GetLSB(byte b)
        {
            return (b & 1) == 1;
        }


        // Извиква се когато искаме да извлечем съобщението
        private int GetMessageLengthFromImage(WriteableBitmap image)
        {
            int length = 0;
            int stride = image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[4 * stride]; // Прочитаме достатъчно пиксели за извличане на 32-битовата дължина
            image.CopyPixels(new Int32Rect(0, 0, image.PixelWidth, 1), pixels, stride, 0);

            // Използваме първите 32 бита (32 байта ако се взима по 1 бит от всяко RGB)
            for (int i = 0; i < 32; i++)
            {
                if ((pixels[i / 8 * 4] & (1 << (i % 8))) != 0) // Четем i-тия бит от синия канал
                {
                    length |= (1 << i);
                }
            }

            return length;
        }



        private string ConvertBitArrayToString(BitArray bits)
        {
            // Calculate the number of bytes needed to store the bits
            int numBytes = bits.Length / 8;
            if (bits.Length % 8 != 0) numBytes++; // Add an extra byte for remaining bits

            // Convert the BitArray to an array of bytes
            byte[] bytes = new byte[numBytes];
            bits.CopyTo(bytes, 0);

            // Convert the bytes array to a string using UTF8 encoding
            return Encoding.UTF8.GetString(bytes);
        }



    }
}
