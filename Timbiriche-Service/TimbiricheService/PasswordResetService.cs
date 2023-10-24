using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{
    public partial class PasswordResetService : IPasswordReset
    {
        public bool GenerateResetToken(string email)
        {
            bool tokenGenerated = false;
            UserManagement userManagement = new UserManagement();
            int idPlayer = userManagement.EmailExists(email);
            if (idPlayer != 0)
            {
                int token = GenerateToken();
                //TODO: Create an Entity and send it to PasswordReset
                //Cane tokenenGerated to True
            }
            return tokenGenerated;
        }

        public bool ValidateResetToken(string email, int token)
        {
            throw new NotImplementedException();
        }

        private int GenerateToken()
        {
            Random random = new Random();
            int token = random.Next(000000, 999999);
            return token;
        }

    }
}
