using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService.Match
{
    public static class CoinsEarn
    {
        public static int CalculateExtraCoins(int playerPosition, int points)
        {
            int maxNumberOfPlayers = 4;
            int multiplier = 2;
            int additionalPointsEarned = 10;

            return points * multiplier + additionalPointsEarned * (maxNumberOfPlayers - playerPosition);
        }
    }
}
