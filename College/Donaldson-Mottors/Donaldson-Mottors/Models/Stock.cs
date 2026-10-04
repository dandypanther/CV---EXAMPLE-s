using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Stock
    {

        [Key]
        public int StockId { get; set; }
        [Required]
        [Display(Name = "Product Name:")]
        [MaxLength(100)]
        [MinLength(5)]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Price:")]
        [Range(0.01, 9999.99)]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Stock Level:")]
        [Range(0,9999)]
        public int StockLevel { get; set; }

        // Navigational Properties
        // One to Many | One Supplier has Many Stock
        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        // Many to Many | Many JobTypes use Many Products/Stock
        // One to Many | one Stock has many StockUsed
        public List<StockUsed> StockUseds { get; set; }

    }
}