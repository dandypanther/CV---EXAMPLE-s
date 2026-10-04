using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Supplier
    {

        [Key]
        public int SupplierId { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        [Display(Name = "Supplier Name:")]
        public string SupplierName { get; set;}
        [Required]
        [MaxLength(120)]
        [MinLength(5)]
        [Display(Name = "Address 1:")]
        public string Address1 { get; set;}
        [MaxLength(120)]
        [Display(Name = "Address 2:")]
        public string Address2 { get; set;}
        [Required]
        [MaxLength(120)]
        [MinLength(2)]
        [Display(Name = "Postcode:")]
        public string Postcode { get; set;}
        [Required]
        [Display(Name = "Telephone Number:")]
        [MaxLength(20)]
        [MinLength(11)]
        public string TelephoneNO { get; set;}
        [Required]
        [Display(Name = "Email:")]
        [MaxLength(100)]
        [MinLength(5)]
        public string Email { get; set;}

        // Navigational properties
        // One to many | One Supplier has many Products/Stock
        public List<Stock> Stock { get; set;}


    }
}