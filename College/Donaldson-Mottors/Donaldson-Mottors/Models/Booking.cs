using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Booking
    {
        //All booking related attributes stored here
        [Key]
        public int BookingId { get; set; }

        [Required]
        [Display(Name = "Date Of Booking")]
        public DateTime DateOfBooking { get; set; }
        [Required]
        [Display(Name = "Completion Status")]
        public bool CompletionStatus { get; set; }
        [MaxLength(2000)]
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; } //Acts as a total Job Descriptions merging all other jobs dscriptions
        
        [Display(Name = "Total Cost")]
        [Range(0, 99999.99)]
        public double TotalCost { get; set; } //total cost of booking
        
        [Display(Name ="Amount Paid")]
        [Range(0, 99999.99)]
        public double AmountPaid { get; set; } //shows wither or not a Customer paid in full or deposit
        [Required]
        public string Status { get; set; }

        //Navigational Properties
        //One to many | one Customer has many Bookings
        [Required]
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        //One to Many | one Vehicle has many Bookings
        [Required]
        [ForeignKey("Vehicle")]
        [Display(Name = "Vehicle Registration Number")]
        [MaxLength(8)]
        public string VehicleRegNO { get; set; }
        public Vehicle Vehicle { get; set; }

        // One to Many | one Booking has many Jobs
        public List<Job> Jobs { get; set; }

        // One to one | one Bookign has one Invoice
        public List<Invoice> Invoice { get; set; }

        // One to ONe | one Booking has one Payment
        public List<Payment> Payment { get; set; }


    }
}