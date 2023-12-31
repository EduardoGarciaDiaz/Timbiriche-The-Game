﻿using System;
using System.Globalization;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

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
                UpdateAccount(editedAccount);
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
                HandleResultOfUpdate(rowsAffected, editedAccount);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerExceptions>)
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
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }

            return rowsAffected;
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

        private void ShowAccountModifiedMessage()
        {
            string titleEmergentWindow = Properties.Resources.lbAccountModifiedTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkAccountModifiedDescription;

            EmergentWindows.CreateEmergentWindow(titleEmergentWindow, descriptionEmergentWindow);
        }

        private void ShowModifyAccountFailMessage()
        {
            string titleEmergentWindow = Properties.Resources.lbModifyAccountFailTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkModifyAccountFailDescription;

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
            tbxSurname.Style = (Style)FindResource(normalTextBoxStyle);

            ImgNameErrorDetails.Visibility = Visibility.Hidden;
            ImgLastNameErrorDetails.Visibility = Visibility.Hidden;
        }
    }
}
