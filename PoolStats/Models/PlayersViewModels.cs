using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PoolStats.Models
{
    public class PlayersViewModel
    {
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "Player Name")]
        public string PlayerName { get; set; }
    }
}
