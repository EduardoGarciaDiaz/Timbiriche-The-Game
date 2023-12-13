using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IFriendshipManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        List<string> GetListUsernameFriends(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool ValidateFriendRequestSending(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        int AddRequestFriendship(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        List<string> GetUsernamePlayersRequesters(int idPlayer);
    }
}
