using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimbiricheViews.Components;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{

    public partial class XAMLUserForm : Page
    {
        public XAMLUserForm()
        {
            InitializeComponent();
        }

        private void ImgBack_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DpBirthdate_Loaded(object sender, RoutedEventArgs e)
        {
            const int YEARS_AGO_ALLOWED = -3;
            if (sender is DatePicker datePicker)
            {
                datePicker.DisplayDateEnd = DateTime.Today.AddYears(YEARS_AGO_ALLOWED);
            }
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                string email = tbxEmail.Text.Trim().ToLower();
                string username = tbxUsername.Text.Trim();
                if (!ValidateUniqueIdentifier(email, username))
                {
                    if (VerifyEmailCode(email))
                    {
                        Account newAccount = CreateNewAccount();
                        Server.Player newPlayer = CreateNewPlayer(newAccount);
                        try
                        {
                            Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
                            int rowsAffected = userManagerClient.AddUser(newPlayer);
                            if (rowsAffected > 0)
                            {
                                ShowAccountCreatedMessage();
                                NavigationService.GoBack();
                            }
                            else
                            {
                                ShowCreateAccountFailMessage();
                            }
                        }
                        catch (EndpointNotFoundException ex)
                        {
                            Utilities.CreateConnectionFailedMessageWindow();
                            // TODO: Log the excepction
                        }
                    }
                }
            }
        }

        private bool VerifyEmailCode(string email)
        {
            bool isEmailVerified = false;
            if (sendEmail(email))
            {
                XAMLBeginnerCodeVerification codeWindow = new XAMLBeginnerCodeVerification();
                if (codeWindow.ShowDialog() == true)
                {
                    isEmailVerified = true;
                }
            }
            return isEmailVerified;
        }

        private bool sendEmail(string email)
        {
            bool isEmailSend = false;
            Server.EmailVerificationManagerClient emailVerificationManagerClient = new Server.EmailVerificationManagerClient();
            // TODO: Try-Catch
            isEmailSend = emailVerificationManagerClient.SendEmailToken(email);
            return isEmailSend;
        }

        private Account CreateNewAccount()
        {
            DateTime.TryParse(dpBirthdate.Text, out DateTime birthdate);
            Account newAccount = new Account()
            {
                Name = tbxName.Text.Trim(),
                LastName = tbxLastName.Text.Trim(),
                Surname = tbxSurname.Text.Trim(),
                Birthdate = birthdate
            };
            return newAccount;
        }

        private Server.Player CreateNewPlayer(Account account)
        {
            const string NOT_BANNED_STATUS = "Not-Banned";
            const int DEFAULT_NUMBER_OF_COINS = 0;
            const int DEFAULT_ID_COLOR_SELECTED = 1;
            const int DEFAULT_ID_STYLE_SELECTED = 1;

            Server.Player newPlayer = new Server.Player()
            {
                Username = tbxUsername.Text.Trim(),
                Email = tbxEmail.Text.Trim().ToLower(),
                Password = pwBxPassword.Password.Trim(),
                Coins = DEFAULT_NUMBER_OF_COINS,
                Status = NOT_BANNED_STATUS,
                AccountFK = account,
                IdColorSelected = DEFAULT_ID_COLOR_SELECTED,
                IdStyleSelected = DEFAULT_ID_STYLE_SELECTED,
            };
            return newPlayer;
        }

        private void ShowAccountCreatedMessage()
        {
            string titleEmergentWindow = Properties.Resources.lbTitleAccountCreatedSuccess;
            string descriptionEmergentWindow = Properties.Resources.tbkDescriptionAccountCreatedSuccess;
            Utilities.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }

        private void ShowCreateAccountFailMessage()
        {
            string titleEmergentWindow = Properties.Resources.lbTitleCreateAccountFail;
            string descriptionEmergentWindow = Properties.Resources.tbkDescriptionCreateAccountFail;
            Utilities.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }


        public bool ValidateUniqueIdentifier(string email, string username)
        {
            bool existUserIdentifier = false;
            try
            {
                Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
                if (userManagerClient.ValidateUniqueIdentifierUser(email))
                {
                    existUserIdentifier = true;
                    lbExistentEmail.Visibility = Visibility.Visible;
                }
                if (userManagerClient.ValidateUniqueIdentifierUser(username))
                {
                    existUserIdentifier = true;
                    lbExistentUsername.Visibility = Visibility.Visible;
                }
            }
            catch (EndpointNotFoundException ex)
            {
                Utilities.CreateConnectionFailedMessageWindow();
                // TODO: Log the excepction
            }
            return existUserIdentifier;
        }

        public bool ValidateFields()
        {
            SetDefaultStyles();
            ValidatePasswordProperties();
            bool isValid = true;

            if (!Utilities.IsValidPersonalInformation(tbxName.Text))
            {
                tbxName.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgNameErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidPersonalInformation(tbxLastName.Text))
            {
                tbxLastName.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgLastNameErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidEmail(tbxEmail.Text))
            {
                tbxEmail.Style = (Style)FindResource("ErrorTextBoxStyle");
                lbEmailError.Visibility = Visibility.Visible;
                ImgEmailErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidUsername(tbxUsername.Text))
            {
                tbxUsername.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgUsernameErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!Utilities.IsValidPassword(pwBxPassword.Password))
            {
                pwBxPassword.Style = (Style)FindResource("ErrorPasswordBoxStyle");
                ImgPasswordErrorDetails.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!DateTime.TryParse(dpBirthdate.Text, out _))
            {
                dpBirthdate.Style = (Style)FindResource("ErrorDatePickerStyle");
                isValid = false;
            }
            return isValid;
        }

        private void SetDefaultStyles()
        {
            tbxName.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxLastName.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxEmail.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxUsername.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxEmail.Style = (Style)FindResource("NormalTextBoxStyle");
            pwBxPassword.Style = (Style)FindResource("NormalPasswordBoxStyle");
            dpBirthdate.Style = (Style)FindResource("NormalDatePickerStyle");

            lbExistentEmail.Visibility = Visibility.Hidden;
            lbExistentUsername.Visibility = Visibility.Hidden;
            lbEmailError.Visibility = Visibility.Hidden;

            lbPasswordLengthInstruction.Foreground = Brushes.Red;
            lbPasswordSymbolInstruction.Foreground = Brushes.Red;
            lbPasswordCapitalLetterInstruction.Foreground = Brushes.Red;
            lbPasswordLowerLetterInstruction.Foreground = Brushes.Red;
            lbPasswordNumberInstruction.Foreground = Brushes.Red;

            ImgNameErrorDetails.Visibility = Visibility.Hidden;
            ImgLastNameErrorDetails.Visibility = Visibility.Hidden;
            ImgEmailErrorDetails.Visibility = Visibility.Hidden;
            ImgUsernameErrorDetails.Visibility = Visibility.Hidden;
            ImgPasswordErrorDetails.Visibility = Visibility.Hidden;
        }

        private void ValidatePasswordProperties()
        {
            if (pwBxPassword.Password.Trim().Length >= 12)
            {
                lbPasswordLengthInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidSymbol(pwBxPassword.Password))
            {
                lbPasswordSymbolInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidCapitalLetter(pwBxPassword.Password))
            {
                lbPasswordCapitalLetterInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidLowerLetter(pwBxPassword.Password))
            {
                lbPasswordLowerLetterInstruction.Foreground = Brushes.GreenYellow;
            }
            if (Utilities.IsValidNumber(pwBxPassword.Password))
            {
                lbPasswordNumberInstruction.Foreground = Brushes.GreenYellow;
            }
        }
    }
}