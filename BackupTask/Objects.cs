using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupTask
{
    public class TwoPlayerScore
    {
        public int Player1_ID { get; set; }

        public String Player1_Name { get; set; }

        public int Score1 { get; set; }

        public double PercentageWinning1 { get; set; }

        //

        public int Player2_ID { get; set; }

        public String Player2_Name { get; set; }

        public int Score2 { get; set; }

        public double PercentageWinning2 { get; set; }
    }

    public class FourPlayerScore
    {
        public String Players1_IDs { get; set; }

        public String Players1_Name { get; set; }

        public int Score1 { get; set; }

        public double PercentageWinning1 { get; set; }

        //

        public String Players2_IDs { get; set; }

        public String Players2_Name { get; set; }

        public int Score2 { get; set; }

        public double PercentageWinning2 { get; set; }
    }

    public class TwoPlayerStat
    {
        public int PlayerID { get; set; }

        public String PlayerName { get; set; }

        public int TotalGames { get; set; }

        public int TotalGamesWon { get; set; }

        public int TotalGamesLost { get; set; }

        public double PercentageWinning { get; set; }

        public double PlayerCommitment { get; set; }

        public double Points { get; set; }
    }
}
