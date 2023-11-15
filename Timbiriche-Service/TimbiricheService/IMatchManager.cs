using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    public interface IMatchManager
    {
        [OperationContract(IsOneWay = true)]
        void RegisterToTheMatch(string lobbyCode, string username);
        [OperationContract(IsOneWay = true)]
        void EndTurn(string lobbyCode, string typeLine, int row, int column, int points);
        [OperationContract(IsOneWay = true)]
        void EndTurnWithoutMovement(string lobbyCode);
        [OperationContract(IsOneWay = true)]
        void EndMatch(string lobbyCode);
        [OperationContract(IsOneWay = true)]
        void SendMessageToLobby(string lobbyCode, string senderUsername, string message);
    }

    [ServiceContract]
    public interface IMatchManagerCallback
    {
        [OperationContract]
        void NotifyNewTurn(string username);
        [OperationContract]
        void NotifyMovement(string typeLine, int row, int column);
        [OperationContract]
        void NotifyFirstTurn(int matchDurationInMinutes, int turnDurationInMinutes, string username);
        [OperationContract]
        void NotifyNewScoreboard(List<KeyValuePair<string, int>> scoreboard);
        [OperationContract]
        void NotifyEndOfTheMatch(List<KeyValuePair<string, int>> scoreboard, int coins);
        [OperationContract]
        void NotifyNewMessage(string senderUsername, string message);
    }
}
