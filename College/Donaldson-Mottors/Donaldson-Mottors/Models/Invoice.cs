using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Invoice
    {
        //local attributes for an Invoice
        [Key]
        public int InvoiceNo { get; set; }
        [Required]
        [Display(Name = "Company Address:")]
        public string CompanyAddress { get; set; }
        [Required]
        [Display(Name = "CompanyInfo:")]
        public string CompanyInfo { get; set; }
        [Required]
        [Display(Name = "Customer Name:")]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "Customer Address:")]
        public string CustomerAddress { get; set; } 
        [Required]
        [Display(Name = "Customer Contact Information:")]
        public string CustomerContactInfo { get; set; }
        [Required]
        [Display(Name = "Labour Cost:")]
        public double LabourCost { get; set; }
        [Required]
        [Display(Name = "Total Cost of Products Used:")]
        public double ProductCost { get; set; }
        [Required]
        [Display(Name = "Total Cost:")]
        public double TotalCost { get; set; }
        [Required]
        [Display(Name = "Duration:")]
        public double NoHrs { get; set; }

        //navigational properties
        //One to One | one Booking has one Invoice
        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }


    }
}