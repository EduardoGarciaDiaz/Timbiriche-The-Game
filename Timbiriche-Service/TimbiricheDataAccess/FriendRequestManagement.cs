using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;

namespace TimbiricheDataAccess
{
    public class FriendRequestManagement
    {
        private const string STATUS_FRIEND = "Friend";
        private const string STATUS_REQUEST = "Request";

        public bool VerifyFriendship(int idPlayerSender, int idPlayerRequested)
        {
            bool hasRelation = false;

            try
            {
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
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return hasRelation;
        }

        public int AddRequestFriendship(int idPlayerSender, int idPlayerRequested)
        {
            int rowsAffected = -1;

            if (idPlayerSender > 0 && idPlayerRequested > 0)
            {
                FriendShips fsiendShip = new FriendShips();
                fsiendShip.idPlayer = idPlayerSender;
                fsiendShip.idPlayerFriend = idPlayerRequested;
                fsiendShip.statusFriendship = STATUS_REQUEST;

                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        context.FriendShips.Add(fsiendShip);
                        rowsAffected = context.SaveChanges();
                    }
                }
                catch (EntityException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    throw new DataAccessException(ex.Message);
                }
                catch (SqlException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    throw new DataAccessException(ex.Message);
                }
                catch (Exception ex)
                {
                    HandlerExceptions.HandleFatalException(ex);
                    throw new DataAccessException(ex.Message);
                }
            }

            return rowsAffected;
        }

        public bool IsFriend(int idPlayer, int idPlayerFriend)
        {
            try
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
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }

        public List<int> GetPlayerIdOfFriendRequesters(int idPlayer)
        {
            List<int> playersId = new List<int>();

            try
            {
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
                            playersId.Add((int)friendRequester);
                        }
                    }
                    return playersId;
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }

        public int UpdateFriendRequestToAccepted(int idCurrentPlayer, int idPlayerAccepted)
        {
            int rowsAffected = -1;

            try
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
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected;
        }

        public int DeleteFriendRequest(int idCurrentPlayer, int idPlayerRejected)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var friendship = context.FriendShips.FirstOrDefault(fs =>
                        (fs.idPlayer == idPlayerRejected && fs.idPlayerFriend == idCurrentPlayer && fs.statusFriendship == STATUS_REQUEST)
                        || (fs.idPlayer == idCurrentPlayer && fs.idPlayerFriend == idPlayerRejected && fs.statusFriendship == STATUS_REQUEST)
                    );

                    if (friendship != null)
                    {
                        context.FriendShips.Remove(friendship);
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected;
        }

        public int DeleteFriendship(int idCurrentPlayer, int idPlayerFriend)
        {
            int rowsAffected = -1;

            try
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
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected;
        }

        public List<string> GetFriends(int idPlayer)
        {
            try
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
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }
    }    
}