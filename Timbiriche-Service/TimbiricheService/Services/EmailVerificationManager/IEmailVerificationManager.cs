using System.ServiceModel;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IEmailVerificationManager
    {
        [OperationContract]
        bool SendEmailToken(string email);

        [OperationContract]
        bool VerifyEmailToken(string token);
    }
}