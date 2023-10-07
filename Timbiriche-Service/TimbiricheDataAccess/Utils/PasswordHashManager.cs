using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace TimbiricheDataAccess.Utils
{
    public class PasswordHashManager
    {

        string _salt;

        public string salt { get { return _salt; } set { _salt = value; } }

        public string HashPassword(String password)
        {
            _salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, _salt);
            return hashedPassword;
        }

        public bool VerifýPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

    }
}
