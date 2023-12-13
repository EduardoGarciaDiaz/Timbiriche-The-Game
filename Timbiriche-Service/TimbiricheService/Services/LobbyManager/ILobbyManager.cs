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
}

