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
        void StartMatch(string lobbyCode);
        [OperationContract(IsOneWay = true)]
        void JoinLobby(String lobbyCode, LobbyPlayer lobbyPlayer);
    }

    public interface ILobbyManagerCallback
    {
        [OperationContract]
        void NotifyLobbyCreated(string lobbyCode);
        [OperationContract]
        void NotifyPlayersInLobby(string lobbyCode, List<LobbyPlayer> lobbyPlayers);
        [OperationContract]
        void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby);
        [OperationContract]
        void NotifyPlayerLeftLobby();
        [OperationContract]
        void NotifyStartOfMatch();
        [OperationContract]
        void NotifyLobbyIsFull();
        [OperationContract]
        void NotifyLobbyDoesNotExist();
    }

    [DataContract]
    public class LobbyPlayer
    {
        private string _username;
        private int _stylePath;
        private int _hexadecimalColor;
        private ILobbyManagerCallback _callbackChannel;
        private IMatchManagerCallback _matchManagerCallback;
        private IPlayerColorsManagerCallback _colorManagerCallback;

        [DataMember]
        public string Username { get { return _username; } set { _username = value; } }
        [DataMember]
        public int StylePath { get { return _stylePath; } set { _stylePath = value; } }
        [DataMember]
        public int HexadecimalColor { get { return _hexadecimalColor; } set { _hexadecimalColor = value; } }
        public ILobbyManagerCallback CallbackChannel { get { return _callbackChannel;  } set { _callbackChannel = value; } }
        public IMatchManagerCallback MatchCallbackChannel { get { return _matchManagerCallback; } set { _matchManagerCallback = value; } }
        public IPlayerColorsManagerCallback ColorCallbackChannel { get { return _colorManagerCallback; } set { _colorManagerCallback = value; } }
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

