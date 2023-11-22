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
        void RegisterToTheMatch(string lobbyCode, string username, string hexadecimalColor);
        [OperationContract(IsOneWay = true)]
        void EndTurn(string lobbyCode, Movement movement);
        [OperationContract(IsOneWay = true)]
        void EndTurnWithoutMovement(string lobbyCode);
        [OperationContract(IsOneWay = true)]
        void EndMatch(string lobbyCode);
        [OperationContract(IsOneWay = true)]
        void SendMessageToLobby(string lobbyCode, string senderUsername, string message, int idSenderPlayer);
    }

    [ServiceContract]
    public interface IMatchManagerCallback
    {
        [OperationContract]
        void NotifyNewTurn(string username);
        [OperationContract]
        void NotifyMovement(Movement movement);
        [OperationContract]
        void NotifyFirstTurn(int matchDurationInMinutes, int turnDurationInMinutes, string username);
        [OperationContract]
        void NotifyNewScoreboard(List<KeyValuePair<LobbyPlayer, int>> scoreboard);
        [OperationContract]
        void NotifyEndOfTheMatch(List<KeyValuePair<LobbyPlayer, int>> scoreboard, int coinsEarned);
        [OperationContract]
        void NotifyNewMessage(string senderUsername, string message, int idSenderPlayer);
    }

    [DataContract]
    public class Movement
    {
        private string _typeline;
        private int _row;
        private int _column;
        private int _earnedPoints;
        private string _hexadecimalColor;
        private string _stylePath;

        [DataMember]
        public string TypeLine { get { return _typeline; } set { _typeline = value; } }
        [DataMember]
        public int Row { get { return _row; } set { _row = value; } }
        [DataMember] 
        public int Column { get { return _column; } set { _column = value; } }
        [DataMember]
        public int EarnedPoints { get {  return _earnedPoints; } set { _earnedPoints = value; } }
        [DataMember]
        public string HexadecimalColor { get {  return _hexadecimalColor; } set { _hexadecimalColor = value; } }
        [DataMember]
        public string StylePath { get { return _stylePath; } set { _stylePath = value; } }
    }
}
