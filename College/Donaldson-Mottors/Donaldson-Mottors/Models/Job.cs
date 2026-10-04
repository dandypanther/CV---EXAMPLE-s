using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Donaldson_Mottors.Models
{
    public class Job
    {

        [Key]
        public int JobId { get; set; }
        [Display(Name = "Completed on:")]
        public DateTime? CompletionDate { get; set; } //job is complete if it has completion date

        // Navigational Properties
        // One to many | 1 Booking has many Jobs
        [ForeignKey("Booking")]
        public int BookingId {  get; set; }
        public Booking Booking { get; set; }

        // One to many | 1 Staff has many Jobs
        [ForeignKey("StaffAssigned")]
        public string StaffId { get; set; }
        public Staff StaffAssigned { get; set; }

        // one to many | 1 JobType has many Jobs
        [ForeignKey("JobType")]
        public string JobTypeId { get; set; }
        public JobType JobType { get; set; }
    }
}