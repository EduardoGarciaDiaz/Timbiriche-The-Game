using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IPasswordReset
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool SendResetToken(string email);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool ValidateResetToken(string email, int token);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool ChangePassword(string newPassword, string email);
    }
}
