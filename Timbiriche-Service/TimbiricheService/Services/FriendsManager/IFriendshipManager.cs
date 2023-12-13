using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IFriendshipManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        List<string> GetListUsernameFriends(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool ValidateFriendRequestSending(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        int AddRequestFriendship(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        List<string> GetUsernamePlayersRequesters(int idPlayer);
    }
}
