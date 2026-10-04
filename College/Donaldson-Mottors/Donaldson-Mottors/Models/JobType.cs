using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class JobType
    {
        [Key]
        public string JobTypeId { get; set; }
        [Required]
        public string JobDescription { get; set; }
        [Required]
        public double LabourRate { get; set; }
        [Required]
        public double NoOfHours { get; set; }

        // Navigational Properties

        //many to many one Job Type uses many Products/Stock
        //one to many | one JobType has many StockUsed

        public List<StockUsed> StockUseds { get; set; }
    }
}