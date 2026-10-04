using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Vehicle
    {
        [Key]
        [Display(Name = "Vehicle Registration Number")]
        [MaxLength(8)]
        [MinLength(5)]
        public string VehicleRegNO { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Make { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Model { get; set; }
        [Required]
        [Range(1900, 2300)]
        public int Year { get; set; }
        [Required]
        [Display(Name = "Engine Size")]
        [Range(0, 9999)]
        public int EngineSize { get; set; }
        [Required]
        [Range(0, 9999)]
        public int Miles { get; set; }

        [Required]
        public bool IsValid { get; set; }

        //Navigational Properties
        //One to Many | One Vehicle has many Bookings
        public List<Booking> Bookings   { get; set; }

        //One to Many | one Customer has many Vehicles
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }


    }

    public class VehicleViewModel //used as a viewmodel for creating Vehicles
    {
        [Display(Name = "Vehicle Registration Number")]
        [MaxLength(8)]
        [MinLength(5)]
        [Required]
        public string VehicleRegNO { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Make { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Model { get; set; }
        [Required]
        [Range(1900, 2300)]
        public int Year { get; set; }
        [Required]
        [Display(Name = "Engine Size")]
        [Range(0, 9999)]
        public int EngineSize { get; set; }
        [Required]
        [Range(0, 9999)]
        public int Miles { get; set; }


    }
}