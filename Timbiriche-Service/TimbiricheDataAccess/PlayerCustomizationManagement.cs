using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public class PlayerCustomizationManagement
    {
        public List<PlayerColors> GetMyColorsByIdPlayer(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var myColors = (from pc in context.PlayerColors
                                where pc.idPlayer == idPlayer
                                select pc).ToList();
                
                 return myColors;
            }
        }

        public string GetHexadecimalColorByIdColor(int idColor)
        {
            string hexadecimalColor = null;

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

        public int UpdateMyColorSelected(int idPlayer, int idColor)
        {
            int rowsAffected = -1;

            if (idPlayer > 0)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players.Find(idPlayer);
                    player.idColorSelected = idColor;

                    rowsAffected = context.SaveChanges();
                }
            }            

            return rowsAffected;
        }

        public bool SearchInMyColors(int idPlayer, int idColor)
        {
            bool hasColor = false;

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

            return hasColor;
        }

        public List<PlayerStyles> GetMyStylesByIdPlayer(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var myStyles = (from ps in context.PlayerStyles
                                where ps.idPlayer == idPlayer
                                select ps).ToList();

                return myStyles;
            }
        }

        public string GetStylePathByIdStyle(int idStyle)
        {
            string stylePath = null;

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

        public int UpdateMyStyleSelected(int idPlayer, int idStyle)
        {
            int rowsAffected = -1;

            if (idPlayer > 0)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players.Find(idPlayer);
                    player.idStyleSelected = idStyle;

                    rowsAffected = context.SaveChanges();
                }
            }
                
            return rowsAffected;
        }
    }
}
