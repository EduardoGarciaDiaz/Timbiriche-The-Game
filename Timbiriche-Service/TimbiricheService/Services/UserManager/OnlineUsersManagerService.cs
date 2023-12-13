using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class UserManagerService : IOnlineUsersManager
    {
        private static Dictionary<string, IUserManagerCallback> onlineUsers = new Dictionary<string, IUserManagerCallback>();

        public void RegisterUserToOnlineUsers(int idPlayer, string username)
        {
            IUserManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IUserManagerCallback>();
            List<string> onlineUsernames = onlineUsers.Keys.ToList();
            List<string> onlineFriends = new List<string>();

            if (!onlineUsers.ContainsKey(username))
            {
                onlineUsers.Add(username, currentUserCallbackChannel);
            }

            onlineFriends = onlineUsernames
                .Where(onlineUsername => IsFriend(idPlayer, onlineUsername))
                .ToList();

            try
            {
                currentUserCallbackChannel.NotifyOnlineFriends(onlineFriends);
            }
            catch (CommunicationException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                UnregisterUserToOnlineUsers(username);
            }

            NotifyUserLoggedInToFriends(idPlayer, username);
        }

        private void NotifyUserLoggedInToFriends(int idPlayer, string username)
        {
            foreach (var user in onlineUsers.ToList())
            {
                if (user.Key != username && IsFriend(idPlayer, user.Key))
                {
                    try
                    {
                        user.Value.NotifyUserLoggedIn(username);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        UnregisterUserToOnlineUsers(username);
                    }
                }
            }
        }

        private bool IsFriend(int currentIdPlayer, string onlineUsername)
        {
            FriendRequestManagement friendRequestDataAccess = new FriendRequestManagement();
            UserManagement userDataAccess = new UserManagement();
            int idOnlinePlayer = userDataAccess.GetIdPlayerByUsername(onlineUsername);

            bool isFriend = friendRequestDataAccess.IsFriend(currentIdPlayer, idOnlinePlayer);

            return isFriend;
        }

        public void UnregisterUserToOnlineUsers(string username)
        {
            if (onlineUsers.ContainsKey(username))
            {
                onlineUsers.Remove(username);
                onlineFriendship.Remove(username);

                foreach (var user in onlineUsers.ToList())
                {
                    try
                    {
                        user.Value.NotifyUserLoggedOut(username);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        UnregisterUserToOnlineUsers(username);
                    }
                }
            }
        }
    }
}
