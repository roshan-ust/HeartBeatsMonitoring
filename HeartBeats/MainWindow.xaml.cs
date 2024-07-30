using HeartBeats.Models;
using System;
using System.Windows;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HeartBeats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Login login;
        private HomePage homePage;
        private BasicProps user;

        public MainWindow()
        {
            InitializeComponent();
            user = new BasicProps();
            ShowLandingView();
        }

        private void ShowLoginView()
        {
            login = new Login(user);
            login.LoginClicked += LoginView_LoginClicked;
            MainGrid.Children.Clear();
            MainGrid.Children.Add(login);
        }

        private void LoginView_LoginClicked(object sender, RoutedEventArgs e)
        {
            ShowLandingView();
        }

        private void ShowLandingView()
        {
            Outlook.Application outlookApp = new Outlook.Application();

            // Get the MAPI namespace
            Outlook.NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

            try
            {
                // Attempt to logon with provided credentials
                //outlookNamespace.Logon(email, password);
                user.Name = outlookNamespace.CurrentUser.Name;
                Outlook.AddressEntry addressEntry = outlookNamespace.CurrentUser.AddressEntry;

                // Retrieve the SMTP address from the AddressEntry
                if (addressEntry != null && addressEntry.GetExchangeUser() != null)
                {
                    var exchangeUser = addressEntry.GetExchangeUser();
                    user.Email = exchangeUser.PrimarySmtpAddress;
                }
                else
                {
                    user.Email = addressEntry.Address;
                }

                user.DefaultStore = outlookNamespace.DefaultStore;
                homePage = new HomePage(user);
                MainGrid.Children.Clear();
                MainGrid.Children.Add(homePage);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Ensure to log off when done
                outlookNamespace.Logoff();
            }
        }
    }
}
