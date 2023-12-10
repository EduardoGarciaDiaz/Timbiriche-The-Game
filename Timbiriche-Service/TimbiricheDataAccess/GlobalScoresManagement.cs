using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public class GlobalScoresManagement
    {
        public List<GlobalScores> GetGlobalScores()
        {
            using (var context = new TimbiricheDBEntities())
            {
                List<GlobalScores> globalScores = (from globalScore in context.GlobalScores
                                                   orderby globalScore.winsNumber descending
                                                   select globalScore).ToList();
                
                return globalScores;
            }
        }

        public int UpdateWinsPlayer(int idPlayer)
        {
            int rowsAffected = -1;

            if (idPlayer > 0)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    GlobalScores globalScorePlayer = (from globalScore in context.GlobalScores
                                                      where globalScore.idPlayer == idPlayer
                                                      select globalScore).FirstOrDefault<GlobalScores>();
                    int winsNumber = (int)globalScorePlayer.winsNumber;
                    winsNumber++;
                    globalScorePlayer.winsNumber = winsNumber;
                    rowsAffected = context.SaveChanges();
                }
            }

            return rowsAffected;
        }
    }
}
