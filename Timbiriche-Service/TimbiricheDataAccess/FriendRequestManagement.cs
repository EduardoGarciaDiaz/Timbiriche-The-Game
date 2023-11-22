using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public class FriendRequestManagement
    {
        private const string STATUS_FRIEND = "Friend";
        private const string STATUS_REQUEST = "Request";

        public bool VerifyFriendship(int idPlayerSender, int idPlayerRequested)
        {
            bool hasRelation = false;
            using (var context = new TimbiricheDBEntities())
            {
                var fsiendship = (from fs in context.FriendShips
                                  where
                                      (fs.idPlayer == idPlayerSender && fs.idPlayerFriend == idPlayerRequested)
                                      || (fs.idPlayer == idPlayerRequested && fs.idPlayerFriend == idPlayerSender) 
                                      && (fs.statusFriendship.Equals(STATUS_FRIEND) || fs.statusFriendship.Equals(STATUS_REQUEST))
                                  select fs).ToList();

                hasRelation = fsiendship.Any();
            }
            return hasRelation;
        }

        public int AddRequestFriendship(int idPlayerSender, int idPlayerRequested)
        {
            if (idPlayerSender > 0 && idPlayerRequested > 0)
            {
                FriendShips fsiendShip = new FriendShips();
                fsiendShip.idPlayer = idPlayerSender;
                fsiendShip.idPlayerFriend = idPlayerRequested;
                fsiendShip.statusFriendship = STATUS_REQUEST;

                using (var context = new TimbiricheDBEntities())
                {
                    var newFriendship = context.FriendShips.Add(fsiendShip);
                    try
                    {
                        return context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Entity: {entityValidationErrors.Entry.Entity.GetType().Name}, Field: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                            }
                        }
                    }

                }
            }
            return -1;
        }

        public bool IsFriend(int idPlayer, int idPlayerFriend)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var fsiendship = (from fs in context.FriendShips
                                  where
                                      ((fs.idPlayer == idPlayer && fs.idPlayerFriend == idPlayerFriend)
                                      || (fs.idPlayer == idPlayerFriend && fs.idPlayerFriend == idPlayer))
                                      && (fs.statusFriendship.Equals(STATUS_FRIEND))
                                  select fs).ToList();
                bool isFriend = fsiendship.Any();
                return isFriend;
            }
        }

        public List<int> GetPlayerIdOfFriendRequesters(int idPlayer)
        {
            List<int> playersId = new List<int>();

            using (var context = new TimbiricheDBEntities())
            {
                var playerFriendRequests = (
                    from fs in context.FriendShips
                    where (fs.idPlayerFriend == idPlayer && fs.statusFriendship.Equals(STATUS_REQUEST))
                    select fs.idPlayer
                ).ToList();

                if (playerFriendRequests.Any())
                {
                    foreach (var friendRequester in playerFriendRequests)
                    {
                        playersId.Add((int) friendRequester);
                    }
                }
                return playersId;
            }
        }

        public int UpdateFriendRequestToAccepted(int idCurrentPlayer, int idPlayerAccepted)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var friendship = context.FriendShips.FirstOrDefault(fs =>
                    (fs.idPlayer == idPlayerAccepted && fs.idPlayerFriend == idCurrentPlayer && fs.statusFriendship == STATUS_REQUEST)
                    || (fs.idPlayer == idCurrentPlayer && fs.idPlayerFriend == idPlayerAccepted && fs.statusFriendship == STATUS_REQUEST)
                );

                if (friendship != null)
                {
                    friendship.statusFriendship = STATUS_FRIEND; 
                    return context.SaveChanges(); 
                }

                return -1;
            }
        }

        public int DeleteFriendRequest(int idCurrentPlayer, int idPlayerAccepted)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var friendship = context.FriendShips.FirstOrDefault(fs =>
                    (fs.idPlayer == idPlayerAccepted && fs.idPlayerFriend == idCurrentPlayer && fs.statusFriendship == STATUS_REQUEST)
                    || (fs.idPlayer == idCurrentPlayer && fs.idPlayerFriend == idPlayerAccepted && fs.statusFriendship == STATUS_REQUEST)
                );

                if (friendship != null)
                {
                    context.FriendShips.Remove(friendship);
                    return context.SaveChanges();
                }

                return -1;
            }
        }

        public int DeleteFriendship(int idCurrentPlayer, int idPlayerFriend)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var friendship = context.FriendShips.FirstOrDefault(fs =>
                    (fs.idPlayer == idPlayerFriend && fs.idPlayerFriend == idCurrentPlayer && fs.statusFriendship == STATUS_FRIEND)
                    || (fs.idPlayer == idCurrentPlayer && fs.idPlayerFriend == idPlayerFriend && fs.statusFriendship == STATUS_FRIEND)
                );

                if (friendship != null)
                {
                    context.FriendShips.Remove(friendship);
                    return context.SaveChanges();
                }
                return -1;
            }
        }

        public List<string> GetFriends(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var friends = context.FriendShips
                .Where(f => (f.idPlayerFriend == idPlayer || f.idPlayer == idPlayer) && f.statusFriendship == STATUS_FRIEND)
                .SelectMany(f => new[] { f.idPlayer, f.idPlayerFriend })
                .Distinct()
                .Where(id => id != idPlayer)
                .Join(context.Players,
                      friendId => friendId,
                      player => player.idPlayer,
                      (friendId, player) => player.username)
                .ToList();
                return friends;
            }
        }
    }    
}