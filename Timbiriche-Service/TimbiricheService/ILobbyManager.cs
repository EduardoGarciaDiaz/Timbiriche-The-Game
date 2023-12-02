using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace TimbiricheService
{
    [ServiceContract]
    public interface ILobbyExistenceChecker
    {
        [OperationContract]
        bool ExistLobbyCode(string lobbyCode);
    }

    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager
    {
        [OperationContract(IsOneWay = true)]
        void CreateLobby(LobbyInformation lobbyInformation, LobbyPlayer lobbyPlayer);
        [OperationContract(IsOneWay = true)]
        void StartMatch(string lobbyCode);
        [OperationContract(IsOneWay = true)]
        void JoinLobby(String lobbyCode, LobbyPlayer lobbyPlayer);
        [OperationContract]
        void ExitLobby(String lobbyCode, String username);
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
        void NotifyPlayerLeftLobby(String username);
        [OperationContract]
        void NotifyHostPlayerLeftLobby();
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
        private int _idStylePath;
        private int _idHexadecimalColor;
        private string _stylePath;
        private string _hexadecimalColor;
        private ILobbyManagerCallback _callbackChannel;
        private IMatchManagerCallback _matchManagerCallback;
        private IPlayerColorsManagerCallback _colorManagerCallback;
        private IPlayerStylesManagerCallback _styleManagerCallback;

        [DataMember]
        public string Username { get { return _username; } set { _username = value; } }
        [DataMember]
        public int IdStylePath { get { return _idStylePath; } set { _idStylePath = value; } }
        [DataMember]
        public int IdHexadecimalColor { get { return _idHexadecimalColor; } set { _idHexadecimalColor = value; } }
        [DataMember]
        public string StylePath { get { return _stylePath; } set { _stylePath = value; } }
        [DataMember]
        public string HexadecimalColor { get { return _hexadecimalColor; } set { _hexadecimalColor = value; } }
        public ILobbyManagerCallback CallbackChannel { get { return _callbackChannel;  } set { _callbackChannel = value; } }
        public IMatchManagerCallback MatchCallbackChannel { get { return _matchManagerCallback; } set { _matchManagerCallback = value; } }
        public IPlayerColorsManagerCallback ColorCallbackChannel { get { return _colorManagerCallback; } set { _colorManagerCallback = value; } }
        public IPlayerStylesManagerCallback StyleCallbackChannel { get { return _styleManagerCallback; } set { _styleManagerCallback = value; } }

        public override bool Equals(object obj)
        {
            if(obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                LobbyPlayer otherPlayer = (LobbyPlayer)obj;
                return Username == otherPlayer.Username && IdStylePath == otherPlayer.IdStylePath && 
                    IdHexadecimalColor == otherPlayer.IdHexadecimalColor && StylePath == otherPlayer.StylePath &&
                    HexadecimalColor == otherPlayer.HexadecimalColor;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract]
    public class LobbyInformation
    {
        private float _matchDurationInMinutes;
        private float _turnDurationInMinutes;
        private int _status;

        [DataMember]
        public float MatchDurationInMinutes { get {  return _matchDurationInMinutes; } set { _matchDurationInMinutes = value; } }
        [DataMember]
        public float TurnDurationInMinutes { get { return _turnDurationInMinutes; } set { _turnDurationInMinutes = value; } }
    }
}

