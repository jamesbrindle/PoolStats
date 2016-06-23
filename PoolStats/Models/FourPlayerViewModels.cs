using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PoolStats.Models
{
    public class FourPlayerViewModel
    {
        [Display(Name = "Players 1")]
        public string Players1 { get; set; }

        [Display(Name = "Score")]
        public int Score1 { get; set; }

        [Display(Name = "Players 2")]
        public string Players2 { get; set; }

        [Display(Name = "Score")]
        public int Score2 { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "ID")]
        public int ID { get; set; }
    }
}
