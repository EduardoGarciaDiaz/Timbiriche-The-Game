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

