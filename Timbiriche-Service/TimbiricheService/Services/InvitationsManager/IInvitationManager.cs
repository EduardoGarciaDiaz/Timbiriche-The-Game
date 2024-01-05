using System.ServiceModel;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IInvitationManager
    {
        [OperationContract]
        bool SendInvitationToEmail(string lobbyCode, string email);
    }
}
