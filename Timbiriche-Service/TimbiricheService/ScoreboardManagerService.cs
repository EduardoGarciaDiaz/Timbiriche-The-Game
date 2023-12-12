using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{

    public partial class UserManagerService : IScoreboardManager
    {
        public List<GlobalScore> GetGlobalScores()
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            List<GlobalScore> globalScores = new List<GlobalScore>();

            try
            {
                List<GlobalScores> scores = dataAccess.GetGlobalScores();

                foreach (GlobalScores score in scores)
                {
                    GlobalScore globalScore = new GlobalScore
                    {
                        IdGlobalScore = score.idGlobalScore,
                        IdPlayer = (int)score.idPlayer,
                        WinsNumber = (int)score.winsNumber
                    };

                    globalScores.Add(globalScore);
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

            return globalScores;
        }

        public int UpdateWins(int idPlayer)
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            try
            {
                int response = dataAccess.UpdateWinsPlayer(idPlayer);
                UpdateGlobalScore();

                return response;
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

    public partial class UserManagerService : IGlobalScoreManager
    {
        private static Dictionary<string, IGlobalScoreManagerCallback> globalScoreRealTime = new Dictionary<string, IGlobalScoreManagerCallback>();

        public void SubscribeToGlobalScoreRealTime(string usernameCurrentPlayer)
        {
            if (!globalScoreRealTime.ContainsKey(usernameCurrentPlayer))
            {
                IGlobalScoreManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGlobalScoreManagerCallback>();
                globalScoreRealTime.Add(usernameCurrentPlayer, currentUserCallbackChannel);

                try
                {
                    currentUserCallbackChannel.NotifyGlobalScoreboardUpdated();
                }
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    UnsubscribeToGlobalScoreRealTime(usernameCurrentPlayer);
                }
            }
        }

        public void UnsubscribeToGlobalScoreRealTime(string usernameCurrentPlayer)
        {
            if (globalScoreRealTime.ContainsKey(usernameCurrentPlayer))
            {
                IGlobalScoreManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGlobalScoreManagerCallback>();
                globalScoreRealTime.Remove(usernameCurrentPlayer);
            }
        }

        public void UpdateGlobalScore()
        {
            foreach (var user in globalScoreRealTime.ToList())
            {
                try
                {
                    user.Value.NotifyGlobalScoreboardUpdated();
                }
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    UnsubscribeToGlobalScoreRealTime(user.Key);
                }
            }
        }
    }


}
