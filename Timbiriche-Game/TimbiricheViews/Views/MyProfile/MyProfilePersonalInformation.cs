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

namespace TimbiricheViews.Views
{
    public partial class XAMLMyProfile : Page
    {
        private void DpBirthdate_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                datePicker.DisplayDateEnd = DateTime.Today.AddYears(-3);
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
            if (ValidateFields())
            {
                try
                {
                    Account editedAccount = CreateEditedAccount();

                    if (HasDifferenteData(editedAccount))
                    {
                        Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
                        int rowsAffected = userManagerClient.UpdateAccount(editedAccount);

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
                catch (FaultException<TimbiricheServerException> ex)
                {
                    EmergentWindows.CreateDataBaseErrorMessageWindow();
                    NavigationService.Navigate(new XAMLLogin());
                }
                catch (FaultException ex)
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
            }
        }

        private Account CreateEditedAccount()
        {
            DateTime.TryParse(dpBirthdate.Text, out DateTime birthdate);

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

            if (!ValidationUtilities.IsValidPersonalInformation(tbxName.Text.Trim()))
            {
                tbxName.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgNameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }
            if (!ValidationUtilities.IsValidPersonalInformation(tbxLastName.Text.Trim()))
            {
                tbxLastName.Style = (Style)FindResource("ErrorTextBoxStyle");
                ImgLastNameErrorDetails.Visibility = Visibility.Visible;

                isValid = false;
            }
            if (!DateTime.TryParse(dpBirthdate.Text, out _))
            {
                dpBirthdate.Style = (Style)FindResource("ErrorDatePickerStyle");

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
            tbxName.Style = (Style)FindResource("NormalTextBoxStyle");
            tbxLastName.Style = (Style)FindResource("NormalTextBoxStyle");
            dpBirthdate.Style = (Style)FindResource("NormalDatePickerStyle");

            ImgNameErrorDetails.Visibility = Visibility.Hidden;
            ImgLastNameErrorDetails.Visibility = Visibility.Hidden;
        }
    }
}
