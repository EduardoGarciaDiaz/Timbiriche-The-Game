using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public static class ShopManagement
    {
        public static List<Colors> GetColors()
        {
            using (var context = new TimbiricheDBEntities())
            {
                var colors = context.Colors.ToList();

                return colors;
            }
        }

        public static List<PlayerColors> GetPlayerColors(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var playerColors = context.PlayerColors
                    .Where(pc => pc.idPlayer == idPlayer)
                    .ToList();

                return playerColors;
            }
        }

        public static List<Styles> GetStyles()
        {
            List<Styles> styles = null;
            using (var context = new TimbiricheDBEntities())
            {
                var firstStyle = context.Styles.FirstOrDefault();
                
                if(firstStyle != null)
                {
                    styles = context.Styles.Where(s => s.idStyle != firstStyle.idStyle).ToList();

                }

                return styles;
            }
        }

        public static List<PlayerStyles> GetPlayerStyles(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var playerStyles = context.PlayerStyles
                    .Where(pc => pc.idPlayer == idPlayer)
                    .ToList();

                return playerStyles;
            }
        }

        public static bool BuyColor(int idColor, int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                PlayerColors playerColors = new PlayerColors
                {
                    idPlayer = idPlayer,
                    idColor = idColor
                };

                context.PlayerColors.Add(playerColors);
                int rowsAffected = context.SaveChanges();

                return rowsAffected > 0;
            }
        }

        public static bool BuyStyle(int idStyle, int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                PlayerStyles playerStyles = new PlayerStyles
                {
                    idPlayer = idPlayer,
                    idStyle = idStyle
                };

                context.PlayerStyles.Add(playerStyles);
                int rowsAffected = context.SaveChanges();

                return rowsAffected > 0;
            }
        }

        public static bool SubstractPlayerCoins(int idPlayer, int coinsToSubstract)
        {
            using (var context = new TimbiricheDBEntities())
            {
                int rowsAffected = 0;
                var player = context.Players.FirstOrDefault(p => p.idPlayer == idPlayer);

                if(player != null)
                {
                    player.coins -= coinsToSubstract;

                    rowsAffected = context.SaveChanges();
                }

                return rowsAffected > 0;
            }
        }
    }
}
