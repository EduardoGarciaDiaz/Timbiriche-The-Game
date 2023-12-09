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
        void JoinLobby(string lobbyCode, LobbyPlayer lobbyPlayer);
        [OperationContract(IsOneWay = true)]
        void JoinLobbyAsHost(string lobbyCode);
        [OperationContract]
        void ExitLobby(string lobbyCode, string username);
        [OperationContract]
        void ExpulsePlayerFromLobby(string lobbyCode, string username);
    }

    [ServiceContract]
    public interface ILobbyManagerCallback
    {
        [OperationContract]
        void NotifyLobbyCreated(string lobbyCode);
        [OperationContract]
        void NotifyPlayersInLobby(string lobbyCode, List<LobbyPlayer> lobbyPlayers);
        [OperationContract]
        void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby);
        [OperationContract]
        void NotifyPlayerLeftLobby(string username);
        [OperationContract]
        void NotifyHostPlayerLeftLobby();
        [OperationContract]
        void NotifyStartOfMatch();
        [OperationContract]
        void NotifyLobbyIsFull();
        [OperationContract]
        void NotifyLobbyDoesNotExist();
        [OperationContract]
        void NotifyExpulsedFromLobby();
    }

    [DataContract]
    public class LobbyPlayer
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int IdStylePath { get; set; }
        [DataMember]
        public int IdHexadecimalColor { get; set; }
        [DataMember]
        public string StylePath { get; set; }
        [DataMember]
        public string HexadecimalColor { get; set; }

        public ILobbyManagerCallback CallbackChannel { get; set; }
        public IMatchManagerCallback MatchCallbackChannel { get; set; }
        public IPlayerColorsManagerCallback ColorCallbackChannel { get; set; }
        public IPlayerStylesManagerCallback StyleCallbackChannel { get; set; }
        public IBanManagerCallback BanManagerChannel { get; set; }

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
        [DataMember]
        public float MatchDurationInMinutes { get; set; }

        [DataMember]
        public float TurnDurationInMinutes { get; set; }
    }
}

