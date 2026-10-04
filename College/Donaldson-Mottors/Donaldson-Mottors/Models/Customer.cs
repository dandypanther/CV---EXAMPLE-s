using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Customer : User
    {
        //All Customer related Attributes stored here
        
        [Required]
        [MaxLength(100)]//length restricts the length of these feilds.
        public string Street { get; set; }
        [Required]
        [MaxLength(100)]
        public string Town { get; set; }
        [Required]
        [MaxLength(8)]
        public string Postcode { get; set; }
        [Required]
        [Display(Name = "Telephone Number")]
        [MaxLength(20)]
        public string TelephoneNo { get; set; }
        

        //Navigational Properties
        //One to many | One Customer has Many Bookings
        public List<Booking> Bookings { get; set; }

        //One to Many | One Customer has many Vehicles
        public List<Vehicle> Vehicles { get; set; }

        // One to Many | one Customer has Many Payments
        public List<Payment> payments { get; set; }
    }
}