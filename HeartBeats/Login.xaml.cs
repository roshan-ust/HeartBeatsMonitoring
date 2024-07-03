using HeartBeats.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        private BasicProps _user;
        public Login(BasicProps user)
        {
            InitializeComponent();
            _user = user;
        }


        public event RoutedEventHandler LoginClicked;

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Example login validation
            if (IsValidUser(EmailTextBox.Text, PasswordBox.Password))
            {
                LoginClicked?.Invoke(this, e);
            }
            else
            {
                MessageBox.Show("Invalid email or password. Please try again.");
            }
        }

        // Example validation function - replace with your authentication logic
        private bool IsValidUser(string email, string password)
        {
            // Create an instance of the Outlook application
            Outlook.Application outlookApp = new Outlook.Application();

            // Get the MAPI namespace
            Outlook.NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

            try
            {
                // Attempt to logon with provided credentials
                outlookNamespace.Logon(email, password);
                _user.Email = email;
                _user.DefaultStore = outlookNamespace.DefaultStore;

                return true;

                // Proceed with accessing Outlook data...
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
            finally
            {
                // Ensure to log off when done
                outlookNamespace.Logoff();
            }
        }
    }
}
