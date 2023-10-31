using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract]
    internal interface IEmailVerificationManager
    {
        [OperationContract]
        bool SendEmailToken(string email);
        [OperationContract]
        bool VerifyEmailToken(string token);
    }
}