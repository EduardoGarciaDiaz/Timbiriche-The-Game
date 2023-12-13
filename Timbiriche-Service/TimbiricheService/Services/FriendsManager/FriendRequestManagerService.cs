﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IFriendRequestManager 
    {
        protected static Dictionary<string, IFriendRequestManagerCallback> onlineFriendship = new Dictionary<string, IFriendRequestManagerCallback>();

        public void AddToOnlineFriendshipDictionary(string usernameCurrentPlayer)
        {
            IFriendRequestManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IFriendRequestManagerCallback>();

            if (!onlineFriendship.ContainsKey(usernameCurrentPlayer))
            {
                onlineFriendship.Add(usernameCurrentPlayer, currentUserCallbackChannel);
            }
        }

        public void SendFriendRequest(string usernamePlayerSender, string usernamePlayerRequested)
        {
            if (onlineFriendship.ContainsKey(usernamePlayerRequested))
            {
                try
                {
                    onlineFriendship[usernamePlayerRequested].NotifyNewFriendRequest(usernamePlayerSender);
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    RemoveFromOnlineFriendshipDictionary(usernamePlayerSender);
                }
            }
        }

        public void AcceptFriendRequest(int idPlayerRequested, string usernamePlayerRequested, string usernamePlayerSender)
        {
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();

            try
            {
                int idPlayerSender = userDataAccess.GetIdPlayerByUsername(usernamePlayerSender);
                int rowsAffected = friendRequestDataAccess.UpdateFriendRequestToAccepted(idPlayerRequested, idPlayerSender);

                if (rowsAffected > 0)
                {
                    InformFriendRequestAccepted(usernamePlayerSender, usernamePlayerRequested);
                    InformFriendRequestAccepted(usernamePlayerRequested, usernamePlayerSender);
                }
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        private void InformFriendRequestAccepted(string usernameTarget, string usernameNewFriend)
        {
            if (onlineFriendship.ContainsKey(usernameTarget))
            {
                try
                {
                    onlineFriendship[usernameTarget].NotifyFriendRequestAccepted(usernameNewFriend);
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    RemoveFromOnlineFriendshipDictionary(usernameTarget);
                }
            }
        }

        public void RejectFriendRequest(int idCurrentPlayer, string username)
        {
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();

            try
            {
                int idPlayerAccepted = userDataAccess.GetIdPlayerByUsername(username);

                friendRequestDataAccess.DeleteFriendRequest(idCurrentPlayer, idPlayerAccepted);
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public void DeleteFriend(int idCurrentPlayer, string usernameCurrentPlayer, string usernameFriendDeleted)
        {
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();

            try
            {
                int idPlayerFriend = userDataAccess.GetIdPlayerByUsername(usernameFriendDeleted);
                int rowsAffected = friendRequestDataAccess.DeleteFriendship(idCurrentPlayer, idPlayerFriend);

                if (rowsAffected > 0)
                {
                    InformFriendDeleted(usernameCurrentPlayer, usernameFriendDeleted);
                    InformFriendDeleted(usernameFriendDeleted, usernameCurrentPlayer);
                }
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        private void InformFriendDeleted(string usernameTarget, string usernameDeletedFriend)
        {
            if (onlineFriendship.ContainsKey(usernameTarget))
            {
                try
                {
                    onlineFriendship[usernameTarget].NotifyDeletedFriend(usernameDeletedFriend);
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    RemoveFromOnlineFriendshipDictionary(usernameTarget);
                }
            }
        }

        public void RemoveFromOnlineFriendshipDictionary(string username)
        {
            onlineFriendship.Remove(username);
        }
    }
}