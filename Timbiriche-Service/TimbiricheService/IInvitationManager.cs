using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IInvitationManager
    {
        [OperationContract]
        bool SendInvitationToEmail(string lobbyCode, string email);
    }
}
