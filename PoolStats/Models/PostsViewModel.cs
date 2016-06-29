using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PoolStats.Models
{
    public class PostsViewModel
    {
        [Display(Name = "ID")]
        [Required]
        public int ID { get; set; }

        [Display(Name = "Player Name")]
        [Required]
        public String PlayerName { get; set; }

        [Display(Name = "Post Date")]
        [Required]
        public DateTime PostDate { get; set; }

        [Display(Name = "Message")]
        [Required]
        public String Message { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }
    }
}
