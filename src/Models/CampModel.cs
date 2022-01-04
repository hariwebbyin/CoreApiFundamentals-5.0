using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; //used to put validations on models
using System.Linq;
using System.Threading.Tasks;


namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Required]
        [Range(1,100)]
        public int Length { get; set; } = 1;


        //Getting specific data from location entity (Camp has Location object,
        //it's gonna be matched from there automaticall, since we've added Location in the starting of each fields.
        
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }
        public string VenueName { get; set; }


        public ICollection<TalkModel> Talks { get; set; }
    }
}
