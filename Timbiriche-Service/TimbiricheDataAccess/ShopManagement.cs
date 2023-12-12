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
    public static class ShopManagement
    {
        public static List<Colors> GetColors()
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var colors = context.Colors.ToList();

                    return colors;
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
        }

        public static List<PlayerColors> GetPlayerColors(int idPlayer)
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var playerColors = context.PlayerColors
                        .Where(pc => pc.idPlayer == idPlayer)
                        .ToList();

                    return playerColors;
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
        }

        public static List<Styles> GetStyles()
        {
            List<Styles> styles = null;
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var firstStyle = context.Styles.FirstOrDefault();

                    if (firstStyle != null)
                    {
                        styles = context.Styles.Where(s => s.idStyle != firstStyle.idStyle).ToList();

                    }

                    return styles;
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
        }

        public static List<PlayerStyles> GetPlayerStyles(int idPlayer)
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var playerStyles = context.PlayerStyles
                        .Where(pc => pc.idPlayer == idPlayer)
                        .ToList();

                    return playerStyles;
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
        }

        public static bool BuyColor(int idColor, int idPlayer)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    Colors colorToBuy = context.Colors.Find(idColor);

                    if (colorToBuy != null)
                    {
                        PlayerColors playerColors = new PlayerColors
                        {
                            idPlayer = idPlayer,
                            idColor = idColor
                        };

                        context.PlayerColors.Add(playerColors);
                        rowsAffected = context.SaveChanges();
                    }

                    return rowsAffected > 0;
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
        }

        public static bool BuyStyle(int idStyle, int idPlayer)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    Styles styleToBuy = context.Styles.Find(idStyle);

                    if (styleToBuy != null)
                    {
                        PlayerStyles playerStyles = new PlayerStyles
                        {
                            idPlayer = idPlayer,
                            idStyle = idStyle
                        };

                        context.PlayerStyles.Add(playerStyles);
                        rowsAffected = context.SaveChanges();
                    }

                    return rowsAffected > 0;
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
        }

        public static bool SubstractPlayerCoins(int idPlayer, int coinsToSubstract)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {

                    var player = context.Players.FirstOrDefault(p => p.idPlayer == idPlayer);

                    if (player != null && player.coins > coinsToSubstract)
                    {
                        player.coins -= coinsToSubstract;

                        rowsAffected = context.SaveChanges();
                    }

                    return rowsAffected > 0;
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
        }
    }
}
