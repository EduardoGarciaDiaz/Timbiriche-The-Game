using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
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
