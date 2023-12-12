using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IPasswordReset
    {
        [OperationContract]
        bool SendResetToken(string email);

        [OperationContract]
        bool ValidateResetToken(string email, int token);
        
        [OperationContract]
        bool ChangePassword(string newPassword, string email);
    }
}
