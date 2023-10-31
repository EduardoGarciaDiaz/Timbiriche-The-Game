using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace TimbiricheService
{
    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager
    {
        [OperationContract(IsOneWay = true)]
        void CreateLobby(LobbyInformation lobbyInformation, LobbyPlayer lobbyPlayer);
        [OperationContract(IsOneWay = true)]
        void StartMatch();
        [OperationContract(IsOneWay = true)]
        void JoinLobby(String lobbyCode, LobbyPlayer lobbyPlayer);
    }

    public interface ILobbyManagerCallback
    {
        [OperationContract]
        void NotifyLobbyCreated();
        [OperationContract]
        void NotifyPlayersInLobby(List<LobbyPlayer> lobbyPlayers);
        [OperationContract]
        void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby);
        [OperationContract]
        void NotifyPlayerLeftLobby();
        [OperationContract]
        void NotifyLobbyIsFull();
        [OperationContract]
        void NotifyLobbyDoesNotExist();
    }

    [DataContract]
    public class LobbyPlayer
    {
        private string _username;
        private string _stylePath;
        private string _hexadecimalColor;
        private ILobbyManagerCallback _callbackChannel;

        [DataMember]
        public string Username { get { return _username; } set { _username = value; } }
        [DataMember]
        public string StylePath { get { return _stylePath; } set { _stylePath = value; } }
        [DataMember]
        public string HexadecimalColor { get { return _hexadecimalColor; } set { _hexadecimalColor = value; } }
        public ILobbyManagerCallback CallbackChannel { get { return _callbackChannel;  } set { _callbackChannel = value; } }
    }

    [DataContract]
    public class LobbyInformation
    {
        private int _matchDurationInMinutes;
        private int _status;

        [DataMember]
        public int MatchDurationInMinutes { get {  return _matchDurationInMinutes; } set { _matchDurationInMinutes = value; } }
        public int Status { get { return _status; } set { _status = value; } }
    }
}

