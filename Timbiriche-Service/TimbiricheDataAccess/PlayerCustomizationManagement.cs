using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;

namespace TimbiricheDataAccess
{
    public class PlayerCustomizationManagement
    {
        public List<PlayerColors> GetMyColorsByIdPlayer(int idPlayer)
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var myColors = (from pc in context.PlayerColors
                                    where pc.idPlayer == idPlayer
                                    select pc).ToList();

                    return myColors;
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

        public string GetHexadecimalColorByIdColor(int idColor)
        {
            string hexadecimalColor = null;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var colorEntity = context.Colors.Where(c => c.idColor == idColor).FirstOrDefault<Colors>();

                    if (colorEntity != null)
                    {
                        hexadecimalColor = colorEntity.hexadecimalCode;
                    }

                    return hexadecimalColor;
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

        public int UpdateMyColorSelected(int idPlayer, int idColor)
        {
            int rowsAffected = -1;

            if (idPlayer > 0)
            {
                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        var player = context.Players.Find(idPlayer);
                        player.idColorSelected = idColor;

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

        public bool SearchInMyColors(int idPlayer, int idColor)
        {
            bool hasColor = false;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var playerColor = (from pc in context.PlayerColors
                                       where pc.idPlayer == idPlayer && pc.idColor == idColor
                                       select pc).ToList();

                    if (playerColor.Count > 0)
                    {
                        hasColor = true;
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

            return hasColor;
        }

        public List<PlayerStyles> GetMyStylesByIdPlayer(int idPlayer)
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var myStyles = (from ps in context.PlayerStyles
                                    where ps.idPlayer == idPlayer
                                    select ps).ToList();

                    return myStyles;
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

        public string GetStylePathByIdStyle(int idStyle)
        {
            string stylePath = null;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var styleEntity = context.Styles.Where(s => s.idStyle == idStyle).FirstOrDefault<Styles>();

                    if (styleEntity != null)
                    {
                        stylePath = styleEntity.path;
                    }

                    return stylePath;
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

        public int UpdateMyStyleSelected(int idPlayer, int idStyle)
        {
            int rowsAffected = -1;

            if (idPlayer > 0)
            {
                try
                {
                    using (var context = new TimbiricheDBEntities())
                    {
                        var player = context.Players.Find(idPlayer);
                        player.idStyleSelected = idStyle;

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
    }
}
