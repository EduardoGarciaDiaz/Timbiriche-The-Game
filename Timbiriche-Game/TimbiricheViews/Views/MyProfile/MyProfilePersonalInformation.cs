using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;
using System.Globalization;

namespace TimbiricheViews.Views
{
    public partial class XAMLMyProfile : Page
    {
        private void DpBirthdate_Loaded(object sender, RoutedEventArgs e)
        {
            int yearsAgoAllowed = 3;

            if (sender is DatePicker datePicker)
            {
                datePicker.DisplayDateEnd = DateTime.Today.AddYears(-yearsAgoAllowed);
            }
        }

        private void LoadPersonalDataPlayer()
        {
            tbxName.Text = playerLoggedIn.AccountFK.Name;
            tbxSurname.Text = playerLoggedIn.AccountFK.Surname;
            tbxLastName.Text = playerLoggedIn.AccountFK.LastName;
            dpBirthdate.Text = playerLoggedIn.AccountFK.Birthdate.ToString();
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            SaveDataChanges();
        }

        private void SaveDataChanges()
        {
            Account editedAccount = CreateEditedAccount();

            if (ValidateFields() && HasDifferenteData(editedAccount))
            {
                int rowsAffected = UpdateAccount(editedAccount);

                HandleResultOfUpdate(rowsAffected, editedAccount);
            }
        }

        private void HandleResultOfUpdate(int rowsAffected, Account editedAccount)
        {
            if (rowsAffected > 0)
            {
                ShowAccountModifiedMessage();
                playerLoggedIn.AccountFK = editedAccount;
            }
            else
            {
                ShowModifyAccountFailMessage();
            }
        }

        private Account CreateEditedAccount()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            DateTime.TryParse(dpBirthdate.Text, cultureInfo, DateTimeStyles.None, out DateTime birthdate);

            Account editedAccount = new Account()
            {
                IdAccount = playerLoggedIn.AccountFK.IdAccount,
                Name = tbxName.Text.Trim(),
                LastName = tbxLastName.Text.Trim(),
                Surname = tbxSurname.Text.Trim(),
                Birthdate = birthdate
            };

            return editedAccount;
        }

        private int UpdateAccount(Account editedAccount)
        {
            UserManagerClient userManagerClient = new UserManagerClient();
            int rowsAffected = -1;

            try
            {
                rowsAffected = userManagerClient.UpdateAccount(editedAccount);
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
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
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

        private void ShowAccountModifiedMessage()
        {
            string titleEmergentWindow = "Cuenta modificada";
            string descriptionEmergentWindow = "Se ha modificado con éxito";

            EmergentWindows.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }

        private void ShowModifyAccountFailMessage()
        {
            string titleEmergentWindow = "Error al modificar";
            string descriptionEmergentWindow = "No fue posible realizar los cambios, por favor intenta de nuevo";

            EmergentWindows.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }

        private bool ValidateFields()
        {
            SetDefaultStyles();
            bool isValid = true;
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            string errorTextBoxStyle = "ErrorTextBoxStyle";
            string errorDatePickerStyle = "ErrorDatePickerStyle";

            if (!ValidationUtilities.IsValidPersonalInformation(tbxName.Text.Trim()))
            {
                tbxName.Style = (Style)FindResource(errorTextBoxStyle);
                ImgNameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!ValidationUtilities.IsValidPersonalInformation(tbxLastName.Text.Trim()))
            {
                tbxLastName.Style = (Style)FindResource(errorTextBoxStyle);
                ImgLastNameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }

            if (!DateTime.TryParse(dpBirthdate.Text, cultureInfo, DateTimeStyles.None, out _))
            {
                dpBirthdate.Style = (Style)FindResource(errorDatePickerStyle);

                isValid = false;
            }

            return isValid;
        }

        private bool HasDifferenteData(Account editedAccount)
        {
            bool isDifferent = true;

            if (playerLoggedIn.AccountFK.Name == editedAccount.Name &&
                    playerLoggedIn.AccountFK.LastName == editedAccount.LastName &&
                    playerLoggedIn.AccountFK.Surname == editedAccount.Surname &&
                    playerLoggedIn.AccountFK.Birthdate == editedAccount.Birthdate)
            {
                isDifferent = false;
            }

            return isDifferent;
        }

        private void SetDefaultStyles()
        {
            string normalTextBoxStyle = "NormalTextBoxStyle";
            string normalDatePickerStyle = "NormalDatePickerStyle";

            tbxName.Style = (Style)FindResource(normalTextBoxStyle);
            tbxLastName.Style = (Style)FindResource(normalTextBoxStyle);
            dpBirthdate.Style = (Style)FindResource(normalDatePickerStyle);

            ImgNameErrorDetails.Visibility = Visibility.Hidden;
            ImgLastNameErrorDetails.Visibility = Visibility.Hidden;
        }
    }
}
