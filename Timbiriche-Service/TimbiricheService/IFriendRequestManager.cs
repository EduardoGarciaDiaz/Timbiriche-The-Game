﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IFriendshipManager
    {
        [OperationContract]
        List<string> GetListUsernameFriends(int idPlayer);
        [OperationContract]
        bool ValidateFriendRequestSending(int idPlayerSender, string usernamePlayerRequested);
        [OperationContract]
        int AddRequestFriendship(int idPlayerSender, string usernamePlayerRequested);
        [OperationContract]
        List<string> GetUsernamePlayersRequesters(int idPlayer);
    }

    [ServiceContract(CallbackContract = typeof(IFriendRequestManagerCallback))]
    public interface IFriendRequestManager
    {
        [OperationContract(IsOneWay = true)]
        void AddToOnlineFriendshipDictionary(string usernameCurrentPlayer);
        [OperationContract(IsOneWay = true)]
        void SendFriendRequest(string usernamePlayerSender, string usernamePlayerRequested);
        [OperationContract(IsOneWay = true)]
        void AcceptFriendRequest(int idPlayerRequested, string usernamePlayerRequested, string usernamePlayerSender);
        [OperationContract(IsOneWay = true)]
        void RejectFriendRequest(int idCurrentPlayer, string username);
        [OperationContract(IsOneWay = true)]
        void DeleteFriend(int idCurrentPlayer, string usernameCurrentPlayer, string usernameFriendDeleted);
    }

    [ServiceContract]
    public interface IFriendRequestManagerCallback
    {
        [OperationContract]
        void NotifyNewFriendRequest(string username);
        [OperationContract]
        void NotifyFriendRequestAccepted(string username);
        [OperationContract]
        void NotifyDeletedFriend(string username);
    }
}