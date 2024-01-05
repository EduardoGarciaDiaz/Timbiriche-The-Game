using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    public interface IMatchManager
    {
        [OperationContract(IsOneWay = true)]
        void RegisterToTheMatch(string lobbyCode, string username, string hexadecimalColor);

        [OperationContract(IsOneWay = true)]
        void EndTurn(string lobbyCode, Movement movement);
        
        [OperationContract(IsOneWay = true)]
        void EndTurnWithoutMovement(string lobbyCode);
        
        [OperationContract(IsOneWay = true)]
        void EndMatch(string lobbyCode);
        
        [OperationContract(IsOneWay = true)]
        void SendMessageToLobby(string lobbyCode, string senderUsername, string message, int idSenderPlayer);
        
        [OperationContract(IsOneWay = true)]
        void LeftMatch(string lobbyCode, string username);
    }

    [ServiceContract]
    public interface IMatchManagerCallback
    {
        [OperationContract]
        void NotifyNewTurn(string username);
        
        [OperationContract]
        void NotifyMovement(Movement movement);
        
        [OperationContract]
        void NotifyFirstTurn(float matchDurationInMinutes, float turnDurationInMinutes, string username);
        
        [OperationContract]
        void NotifyNewScoreboard(List<KeyValuePair<LobbyPlayer, int>> scoreboard);
        
        [OperationContract]
        void NotifyEndOfTheMatch(List<KeyValuePair<LobbyPlayer, int>> scoreboard, int coinsEarned);
        
        [OperationContract]
        void NotifyNewMessage(string senderUsername, string message, int idSenderPlayer);
        
        [OperationContract]
        void NotifyPlayerLeftMatch();
        
        [OperationContract]
        void NotifyOnlyPlayerInMatch();
    }
}
