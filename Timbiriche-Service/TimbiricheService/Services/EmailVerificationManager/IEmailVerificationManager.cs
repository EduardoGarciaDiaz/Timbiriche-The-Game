using System.ServiceModel;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IEmailVerificationManager
    {
        [OperationContract]
        bool SendEmailToken(string email, string username);

        [OperationContract]
        bool VerifyEmailToken(string token, string username);
    }
}