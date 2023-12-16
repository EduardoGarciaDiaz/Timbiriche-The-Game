using System.ServiceModel;

namespace TimbiricheService
{
    [ServiceContract]
    public interface ILobbyExistenceChecker
    {
        [OperationContract]
        bool ExistLobbyCode(string lobbyCode);

        [OperationContract]
        bool ExistUsernameInLobby(string lobbyCode, string username);
    }
}
