using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PoolStats.Models
{
    public class PlayersViewModel
    {
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "Player Name")]
        [Required(ErrorMessage = "Player name is required")]
        [MaxLength(100, ErrorMessage = "Maximum 100 characters allowed")]
        public string PlayerName { get; set; }
    }
}
