using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{
    public partial class UserManagerService : IFriendshipManager
    {
        public bool ValidateFriendRequestSending(int idPlayerSender, string usernamePlayerRequested)
        {
            bool isFriendRequestValid = false;
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();

            int idPlayerRequested = userDataAccess.GetIdPlayerByUsername(usernamePlayerRequested);
            if (idPlayerRequested < 1)
            {
                return false;
            }
            if (idPlayerSender == idPlayerRequested)
            {
                return false;
            }

            bool hasRelation = friendRequestDataAccess.VerifyFriendship(idPlayerSender, idPlayerRequested);
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

            int idPlayerRequested = userDataAccess.GetIdPlayerByUsername(usernamePlayerRequested);
            if (idPlayerRequested > 0) 
            {
                rowsAffected = friendRequestDataAccess.AddRequestFriendship(idPlayerSender, idPlayerRequested);
            }
            return rowsAffected;
        }

        public List<string> GetUsernamePlayersRequesters(int idPlayer)
        {
            List<string> usernamePlayers = new List<string>();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            UserManagement userDataAccess = new UserManagement();
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
    }

    public partial class UserManagerService : IFriendRequestManager 
    {
        public static Dictionary<string, IFriendRequestManagerCallback> onlineFriendship = new Dictionary<string, IFriendRequestManagerCallback>();

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
                onlineFriendship[usernamePlayerRequested].NotifyNewFriendRequest(usernamePlayerSender);
            }
        }

        public void AcceptFriendRequest(int idPlayerRequested, string usernamePlayerRequested, string usernamePlayerSender)
        {
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            
            int idPlayerSender = userDataAccess.GetIdPlayerByUsername(usernamePlayerSender);
            int rowsAffected = friendRequestDataAccess.UpdateFriendRequestToAccepted(idPlayerRequested, idPlayerSender);
            
            if (rowsAffected > 0)
            {
                InformFriendRequestAccepted(usernamePlayerSender, usernamePlayerRequested);
                InformFriendRequestAccepted(usernamePlayerRequested, usernamePlayerSender);
            }
        }

        private void InformFriendRequestAccepted(string usernameTarget, string usernameNewFriend)
        {
            if (onlineFriendship.ContainsKey(usernameTarget))
            {
                onlineFriendship[usernameTarget].NotifyFriendRequestAccepted(usernameNewFriend);
            }
        }

        public void RejectFriendRequest(int idCurrentPlayer, string username)
        {
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            int idPlayerAccepted = userDataAccess.GetIdPlayerByUsername(username);
            int rowsAffected = friendRequestDataAccess.DeleteFriendRequest(idCurrentPlayer, idPlayerAccepted);
        }

        public void DeleteFriend(int idCurrentPlayer, string usernameCurrentPlayer, string usernameFriendDeleted)
        {
            int rowsAffected = -1;
            UserManagement userDataAccess = new UserManagement();
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            int idPlayerFriend = userDataAccess.GetIdPlayerByUsername(usernameFriendDeleted);
            rowsAffected = friendRequestDataAccess.DeleteFriendship(idCurrentPlayer, idPlayerFriend);

            if (rowsAffected > 0)
            {
                InformFriendDeleted(usernameCurrentPlayer, usernameFriendDeleted);
                InformFriendDeleted(usernameFriendDeleted, usernameCurrentPlayer);
            }
        }

        private void InformFriendDeleted(string usernameTarget, string usernameDeletedFriend)
        {
            if (onlineFriendship.ContainsKey(usernameTarget))
            {
                onlineFriendship[usernameTarget].NotifyDeletedFriend(usernameDeletedFriend);
            }
        }
    }
}