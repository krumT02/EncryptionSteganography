using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private  void ToggleVisibility_Click(object sender, RoutedEventArgs e)
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

        
    }
}
