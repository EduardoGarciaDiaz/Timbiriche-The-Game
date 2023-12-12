using System;
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
    public partial class UserManagerService : IFriendshipManager
    {
        public List<string> GetListUsernameFriends(int idPlayer)
        {
            FriendRequestManagement dataAccess = new FriendRequestManagement();

            try
            {
                return dataAccess.GetFriends(idPlayer);
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

        public bool ValidateFriendRequestSending(int idPlayerSender, string usernamePlayerRequested)
        {
            bool isFriendRequestValid = false;
            int idPlayerRequested = 0;
            bool hasRelation = false;
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();

            try
            {
                idPlayerRequested = userDataAccess.GetIdPlayerByUsername(usernamePlayerRequested);
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

            if (idPlayerRequested < 1)
            {
                return false;
            }

            if (idPlayerSender == idPlayerRequested)
            {
                return false;
            }

            try
            {
                hasRelation = friendRequestDataAccess.VerifyFriendship(idPlayerSender, idPlayerRequested);

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

            if (!hasRelation)
            {
                isFriendRequestValid = true;
            }

            return isFriendRequestValid;
        }

        public int AddRequestFriendship(int idPlayerSender, string usernamePlayerRequested)
        {
            int rowsAffected = -1;
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            try
            {
                int idPlayerRequested = userDataAccess.GetIdPlayerByUsername(usernamePlayerRequested);

                if (idPlayerRequested > 0)
                {
                    rowsAffected = friendRequestDataAccess.AddRequestFriendship(idPlayerSender, idPlayerRequested);
                }

                return rowsAffected;
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

        public List<string> GetUsernamePlayersRequesters(int idPlayer)
        {
            List<string> usernamePlayers = new List<string>();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            UserManagement userDataAccess = new UserManagement();

            try
            {
                List<int> playersRequestersId = friendRequestDataAccess.GetPlayerIdOfFriendRequesters(idPlayer);

                if (playersRequestersId != null)
                {
                    foreach (int idRequester in playersRequestersId)
                    {
                        usernamePlayers.Add(userDataAccess.GetUsernameByIdPlayer(idRequester));
                    }
                }

                return usernamePlayers;
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
    }

    public partial class UserManagerService : IFriendRequestManager 
    {
        public static readonly Dictionary<string, IFriendRequestManagerCallback> onlineFriendship = new Dictionary<string, IFriendRequestManagerCallback>();

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
                    HandlerException.HandleErrorException(ex);
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
                    HandlerException.HandleErrorException(ex);
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
                    HandlerException.HandleErrorException(ex);
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