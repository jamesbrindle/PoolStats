using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PoolStats.Models
{
    public class StatsViewModel
    {
        public string PlayerName { get; set; }
        public int? GamesWon { get; set; }
        public int? GamesLost { get; set; }
        public int? TotalGames { get; set; }
        public double PercentageWon { get; set; }
        public double PercentageGamePlay { get; set; }
    }
}
