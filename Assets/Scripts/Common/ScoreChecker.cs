using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReoGames
{
    public static class ScoreChecker
    {
        public static int GetStarScore(int score, int star3Score, int star2Score, int star1Score)
        {
            //★３
            if (score >= star3Score) { return  3; }
            //★２
            else if (score >= star2Score) { return 2; }
            //★１
            else if (score >= star1Score) { return 1; }
            //★０
            else { return 0; }
        }
    }
}

