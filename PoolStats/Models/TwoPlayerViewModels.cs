using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PoolStats.Models
{
    public class TwoPlayerViewModel
    {
        [Display(Name = "Player 1")]
        [Required]
        public string Player1 { get; set; }

        [Display(Name = "Score")]
        public int Score1 { get; set; }

        [Display(Name = "Player 2")]
        [Required]
        public string Player2 { get; set; }

        [Display(Name = "Score")]
        public int Score2 { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "ID")]
        public int ID { get; set; }
    }
}
