using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class StockUsed
    {
        //Many JobTypes use Many Products/Stock
        //Composite Key
        [Key]
        [Column(Order = 0)]
        [ForeignKey("JobType")]
        public string JobTypeId { get; set; }
        public JobType JobType { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Stock")]
        public int StockId { get; set; }
        public Stock Stock { get; set; }

        [Required]
        public int Quantity { get; set; }

    }
}