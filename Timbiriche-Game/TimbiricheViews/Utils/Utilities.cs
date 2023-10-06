using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TimbiricheViews.Utils
{

    internal class Utilities
    {

        private const string VALID_PERSONAL_INFORMATION = "^[A-Za-zÀ-ÖØ-öø-ÿ' ]{2,50}$";
        private const string VALID_EMAIL = "^(?=.{5,100}$)[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        private const string VALID_PASSWORD = "^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[\\W_])[A-Za-z\\d\\W_]{12,32}$";

        public static bool IsEmptyField(TextBox textBox)
        {
            bool isEmpty = false;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                isEmpty = true;
            }
                return isEmpty;
        }

        public static bool IsValidPersonalInformation(TextBox textBox)
        {
            Regex regex = new Regex(VALID_PERSONAL_INFORMATION);
            return regex.IsMatch(textBox.Text.Trim());
        }
        
        public static bool IsValidEmail(TextBox textBox)
        {
            Regex regex = new Regex(VALID_EMAIL);
            return regex.IsMatch(textBox.Text.Trim());
        }

        public static bool IsValidPassword(PasswordBox passwordBox)
        {
            Regex regex = new Regex(VALID_PASSWORD);
            return regex.IsMatch(passwordBox.Password.Trim());
        }

    }

}
