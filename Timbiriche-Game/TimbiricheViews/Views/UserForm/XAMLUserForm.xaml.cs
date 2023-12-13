using System;
using System.Globalization;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            const int yearsAgoAllowed = 3;

            if (sender is DatePicker datePicker)
            {
                datePicker.DisplayDateEnd = DateTime.Today.AddYears(-yearsAgoAllowed);
            }
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            TryCreateAccount();
        }

        private void TryCreateAccount()
        {
            if (ValidateFields())
            {
                string email = tbxEmail.Text.Trim().ToLower();
                string username = tbxUsername.Text.Trim();

                if (!ValidateUniqueIdentifier(email, username) && VerifyEmailCode(email))
                {
                    int rowsAffected = AddUser();

                    HandleResultOfAddUser(rowsAffected);
                }
            }
        }

        private int AddUser()
        {
            int rowsAffected = -1;

            try
            {
                Account newAccount = CreateNewAccount();
                Server.Player newPlayer = CreateNewPlayer(newAccount);

                Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
                rowsAffected = userManagerClient.AddUser(newPlayer);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }

            return rowsAffected;
        }

        private void HandleResultOfAddUser(int rowsAffected)
        {
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

        private bool VerifyEmailCode(string email)
        {
            bool isEmailVerified = false;

            if (SendEmail(email))
            {
                XAMLBeginnerCodeVerification codeWindow = new XAMLBeginnerCodeVerification();

                if (codeWindow.ShowDialog() == true)
                {
                    isEmailVerified = true;
                }
            }

            return isEmailVerified;
        }

        private bool SendEmail(string email)
        {
            bool isEmailSend = false;

            EmailVerificationManagerClient emailVerificationManagerClient = new EmailVerificationManagerClient();

            try
            {
                isEmailSend = emailVerificationManagerClient.SendEmailToken(email);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }

            return isEmailSend;
        }

        private Account CreateNewAccount()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            DateTime.TryParse(dpBirthdate.Text, cultureInfo, DateTimeStyles.None, out DateTime birthdate);

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
            string notBannedStatus = "Not-Banned";
            int defaultCoinsNumber = 0;
            int defaultIdColorSelected = 1;
            int defaultIdStyleSelected = 1;

            Server.Player newPlayer = new Server.Player()
            {
                Username = tbxUsername.Text.Trim(),
                Email = tbxEmail.Text.Trim().ToLower(),
                Password = pwBxPassword.Password.Trim(),
                Coins = defaultCoinsNumber,
                Status = notBannedStatus,
                AccountFK = account,
                IdColorSelected = defaultIdColorSelected,
                IdStyleSelected = defaultIdStyleSelected,
            };

            return newPlayer;
        }

        private void ShowAccountCreatedMessage()
        {
            string titleEmergentWindow = Properties.Resources.lbTitleAccountCreatedSuccess;
            string descriptionEmergentWindow = Properties.Resources.tbkDescriptionAccountCreatedSuccess;

            EmergentWindows.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }

        private void ShowCreateAccountFailMessage()
        {
            string titleEmergentWindow = Properties.Resources.lbTitleCreateAccountFail;
            string descriptionEmergentWindow = Properties.Resources.tbkDescriptionCreateAccountFail;

            EmergentWindows.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }


        public bool ValidateUniqueIdentifier(string email, string username)
        {
            bool existUserIdentifier = false;

            try
            {
                UserManagerClient userManagerClient = new UserManagerClient();

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
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }

            return existUserIdentifier;
        }

        public bool ValidateFields()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            bool isValid = true;
            string errorTextBoxStyle = "ErrorTextBoxStyle";
            string errorPasswordBoxStyle = "ErrorPasswordBoxStyle";
            string errorDatePickerStyle = "ErrorDatePickerStyle";

            SetDefaultStyles();
            ValidatePasswordProperties();

            if (!ValidationUtilities.IsValidPersonalInformation(tbxName.Text.Trim()))
            {
                tbxName.Style = (Style)FindResource(errorTextBoxStyle);
                ImgNameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidationUtilities.IsValidSurname(tbxSurname.Text.Trim()))
            {
                tbxSurname.Style = (Style)FindResource(errorTextBoxStyle);
                isValid = false;
            }

            if (!ValidationUtilities.IsValidPersonalInformation(tbxLastName.Text.Trim()))
            {
                tbxLastName.Style = (Style)FindResource(errorTextBoxStyle);
                ImgLastNameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidationUtilities.IsValidEmail(tbxEmail.Text.Trim()))
            {
                tbxEmail.Style = (Style)FindResource(errorTextBoxStyle);
                lbEmailError.Visibility = Visibility.Visible;
                ImgEmailErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidationUtilities.IsValidUsername(tbxUsername.Text.Trim()))
            {
                tbxUsername.Style = (Style)FindResource(errorTextBoxStyle);
                ImgUsernameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidationUtilities.IsValidPassword(pwBxPassword.Password.Trim()))
            {
                pwBxPassword.Style = (Style)FindResource(errorPasswordBoxStyle);
                ImgPasswordErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!DateTime.TryParse(dpBirthdate.Text, cultureInfo, DateTimeStyles.None, out _))
            {
                dpBirthdate.Style = (Style)FindResource(errorDatePickerStyle);

                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            string normalTextBoxStyle = "NormalTextBoxStyle";
            string normalPasswordBoxStyle = "NormalPasswordBoxStyle";
            string normalDatePickerStyle = "NormalDatePickerStyle";

            tbxName.Style = (Style)FindResource(normalTextBoxStyle);
            tbxLastName.Style = (Style)FindResource(normalTextBoxStyle);
            tbxEmail.Style = (Style)FindResource(normalTextBoxStyle);
            tbxUsername.Style = (Style)FindResource(normalTextBoxStyle);
            tbxSurname.Style = (Style)FindResource(normalTextBoxStyle);
            pwBxPassword.Style = (Style)FindResource(normalPasswordBoxStyle);
            dpBirthdate.Style = (Style)FindResource(normalDatePickerStyle);

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

            if (ValidationUtilities.IsValidSymbol(pwBxPassword.Password))
            {
                lbPasswordSymbolInstruction.Foreground = Brushes.GreenYellow;
            }

            if (ValidationUtilities.IsValidCapitalLetter(pwBxPassword.Password))
            {
                lbPasswordCapitalLetterInstruction.Foreground = Brushes.GreenYellow;
            }

            if (ValidationUtilities.IsValidLowerLetter(pwBxPassword.Password))
            {
                lbPasswordLowerLetterInstruction.Foreground = Brushes.GreenYellow;
            }

            if (ValidationUtilities.IsValidNumber(pwBxPassword.Password))
            {
                lbPasswordNumberInstruction.Foreground = Brushes.GreenYellow;
            }
        }
    }
}