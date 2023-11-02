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
                if (myColors != null )
                {
                    return myColors;
                }
                return null;
                
            }
        }

        public string GetHexadecimalColorByIdColor(int idColor)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var colorEntity = context.Colors.Where(c => c.idColor == idColor).FirstOrDefault<Colors>();

                if (colorEntity != null)
                {
                    string hexadecimalColor = colorEntity.hexadecimalCode;
                    return hexadecimalColor;
                }
                return null;
            }
        }

        public int UpdateMyColorSelected(int idPlayer, int idColor)
        {
            int rowsAffected = -1;
            using (var context = new TimbiricheDBEntities())
            {
                var player = context.Players.Find(idPlayer);
                //player.idColorSelected = idColor
                rowsAffected = context.SaveChanges();
            }
            return rowsAffected;

        }

    }
}
