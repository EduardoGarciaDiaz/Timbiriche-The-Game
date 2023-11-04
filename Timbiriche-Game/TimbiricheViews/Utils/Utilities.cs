using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TimbiricheViews.Components;

namespace TimbiricheViews.Utils
{

    public class Utilities
    {

        private const string VALID_PERSONAL_INFORMATION = "^[A-Za-zÀ-ÖØ-öø-ÿ' ]{2,50}$";
        private const string VALID_USERNAME = "^[a-zA-Z0-9_]+$";
        private const string VALID_EMAIL = "^(?=.{5,100}$)[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        private const string VALID_PASSWORD = "^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[\\W_])[A-Za-z\\d\\W_]{12,32}$";
        private const string VALID_SYMBOLS = "^(?=.*[\\W_])";
        private const string VALID_CAPITAL_LETTERS = "^(?=.*[A-Z])";
        private const string VALID_LOWER_LETTERS = "^(?=.*[a-z])";
        private const string VALID_NUMBERS = "^(?=.*\\d)";

        public static bool IsValidPersonalInformation(string personalInformation)
        {
            Regex regex = new Regex(VALID_PERSONAL_INFORMATION);
            return regex.IsMatch(personalInformation.Trim());
        }

        public static bool IsValidUsername(string username)
        {
            Regex regex = new Regex(VALID_USERNAME);
            return regex.IsMatch(username.Trim());
        }
        
        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(VALID_EMAIL);
            return regex.IsMatch(email.Trim());
        }

        public static bool IsValidPassword(string password)
        {
            Regex regex = new Regex(VALID_PASSWORD);
            return regex.IsMatch(password.Trim());
        }

        public static bool IsValidSymbol(string password)
        {
            Regex regex = new Regex(VALID_SYMBOLS);
            return regex.IsMatch(password.Trim());
        }

        public static bool IsValidCapitalLetter(string password)
        {
            Regex regex = new Regex(VALID_CAPITAL_LETTERS);
            return regex.IsMatch(password.Trim());
        }

        public static bool IsValidLowerLetter(string password)
        {
            Regex regex = new Regex(VALID_LOWER_LETTERS);
            return regex.IsMatch(password.Trim());
        }

        public static bool IsValidNumber(string password)
        {
            Regex regex = new Regex(VALID_NUMBERS);
            return regex.IsMatch(password.Trim());
        }

        public static void CreateEmergentWindow(string titleEmergentWindow, string descriptionEmergentWindow)
        {
            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );
        }

        public static void CreateConnectionFailedMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbConnectionFailed;
            string descriptionEmergentWindow = Properties.Resources.lbConnectionFailedDetails;
            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );
        }

        public static SolidColorBrush CreateColorFromHexadecimal(string hexadecimalColor)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));
        }

    }

}