using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;

namespace TimbiricheDataAccess
{
    public class CoinsManagement
    {
        public int UpdateCoins(string username, int coinsEarned)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players.SingleOrDefault(p => p.username == username);

                    if (player != null)
                    {
                        player.coins += coinsEarned;
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                HandlerException.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerException.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerException.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected;
        }
    }
}
