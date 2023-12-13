using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }
    }
}
