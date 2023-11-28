using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{

    public partial class UserManagerService : IScoreboardManager
    {
        public List<GlobalScore> GetGlobalScores()
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            List<GlobalScore> globalScores = new List<GlobalScore>();
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

            return globalScores;
        }

        public int UpdateWins(int idPlayer)
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            int response = dataAccess.UpdateWinsPlayer(idPlayer);
            UpdateGlobalScore();
            return response;
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
                currentUserCallbackChannel.NotifyGlobalScoreboardUpdated();
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
            foreach (var user in globalScoreRealTime)
            {
                user.Value.NotifyGlobalScoreboardUpdated();
            }
        }
    }


}
