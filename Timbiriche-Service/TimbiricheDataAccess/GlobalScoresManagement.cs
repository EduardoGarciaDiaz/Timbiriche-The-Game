using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Exceptions;

namespace TimbiricheDataAccess
{
    public class GlobalScoresManagement
    {
        public List<GlobalScores> GetGlobalScores()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    List<GlobalScores> globalScores = (from globalScore in context.GlobalScores
                                                       orderby globalScore.winsNumber descending
                                                       select globalScore).ToList();

                    return globalScores;
                }
            }
            catch (EntityException ex)
            {
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex.Message);
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
