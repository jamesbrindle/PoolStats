using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PoolStats.Models
{
    public class PinViewModel
    {
        [Display(Name = "Pin Number")]
        public string PinNumber { get; set; }
    }
}
