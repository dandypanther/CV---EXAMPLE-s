using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Staff : User
    {
        //All Staff related Attributes stored here
        
        [Required]
        [MaxLength(30)]
        public string Position { get; set; } //Staffs position at the company i.e mechanic, manager
        [Required]
        [MaxLength(100)]
        public string Address { get; set; } //Staffs Address
        [Required]
        public DateTime DateOfBirth { get; set; }


    }
}