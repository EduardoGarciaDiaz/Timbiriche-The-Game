﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public class CoinsManagement
    {
        public int UpdateCoins(string username, int coinsEarned)
        {
            using(var context = new TimbiricheDBEntities())
            {
                var player = context.Players.SingleOrDefault(p => p.username == username);

                if(player != null)
                {
                    player.coins += coinsEarned;
                    context.SaveChanges();
                }
            }
        }
    }
}
