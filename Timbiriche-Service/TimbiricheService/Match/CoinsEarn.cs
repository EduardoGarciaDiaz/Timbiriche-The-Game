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
            const int MAX_NUMBER_OF_PLAYERS = 4;

            return points * 2 + 10 * (MAX_NUMBER_OF_PLAYERS - playerPosition);
        }
    }
}
