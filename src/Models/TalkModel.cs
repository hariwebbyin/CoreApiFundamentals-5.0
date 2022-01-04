using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace CoreCodeCamp.Models
{
    public class TalkModel
    {
        public int TalkId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [StringLength(maximumLength: 4000, MinimumLength = 20)]
        public string Abstract { get; set; }

        [Required]
        [Range(100, 200)]
        public int Level { get; set; }
        public SpeakerModel Speaker { get; set; }
    }
}
