using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IPasswordReset
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool SendResetToken(string email);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool ValidateResetToken(string email, int token);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool ChangePassword(string newPassword, string email);
    }
}
