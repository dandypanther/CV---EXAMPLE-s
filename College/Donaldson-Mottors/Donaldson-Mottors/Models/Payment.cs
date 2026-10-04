using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    //used to store payment records
    public class Payment
    {

        //Local attributes
        [Key]
        public int PaymentId { get; set; }
        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }
        [Required]
        [Range(0.01, 9999.99)]
        public double Amount { get; set; }
        [Required]
        [MaxLength(20)]
        public string Currency { get; set; }
        [Required]
        public DateTime PaymentDateTime { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        //Payment Gateway transaction reference NO
        [MaxLength(255)]
        public string PaymentGatewayTransactionReference { get; set; }

        //Navigational Properties
        //One to One | One Payment has One Booking
        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        //One to Many | One Customer makes Many Payments
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}