using Donaldson_Mottors.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Donaldson_Mottors.Models
{
    //change details viiew model here
    //This view model will be used to allow the user to modify their details
    public class ChangeDetailsViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } //Email feild

        [Required]
        [Display(Name = "Full Name")] //Display Name to make easier to read when displayed
        [MaxLength(60)]//Max length for the name feild is here
        public string Name { get; set; }

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

        //Password feilds
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    //different view model for staff
    //change details of the Staff in question
    public class ChangeDetailsViewModelStaff
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } //Email feild

        [Required]
        [Display(Name = "Full Name:")] //Display Name to make easier to read when displayed
        [MaxLength(60)]//Max length for the name feild is here
        public string Name { get; set; }

        [MaxLength(30)]
        [Display(Name = "Position:")]
        public string Position { get; set; } //Staffs position at the company i.e mechanic, manager
        [Required]
        [MaxLength(100)]
        [Display(Name = "Address:")]
        public string Address { get; set; } //Staffs Address
        [Required]
        [Display(Name = "Date of Birth:")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2010", ErrorMessage = "Invalid Date of Birth")]
        public DateTime DateOfBirth { get; set; } //Staffs Address

        //Password feilds
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    //StaffChangeDetailsViewModel is for when Staff edit a Customer
    //change details viiew model here
    //This view model will be used to allow the user to modify their details
    public class StaffChangeDetailsViewModel
    {
        [Required]
        public string UserId { get; set; } //Key feild

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } //Email feild

        [Required]
        [Display(Name = "Full Name")] //Display Name to make easier to read when displayed
        [MaxLength(60)]//Max length for the name feild is here
        public string Name { get; set; }

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
        [Required]
        public bool IsActive { get; set; }


        //Password feilds
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    // this is not a viewmodel, it is used to prevent bookings being made in the past
    public class FutureOrTodayDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true; // let [Required] handle any nulls

            DateTime date = Convert.ToDateTime(value); //get date value

            // prevent bookings in the past or today
            if (date.Date < DateTime.Today.AddDays(1))
                return false;

            // allowed booking hours: 08:00 - 20:00
            TimeSpan bookingTime = date.TimeOfDay;

            TimeSpan openingTime = new TimeSpan(8, 0, 0);  // 8am
            TimeSpan closingTime = new TimeSpan(20, 0, 0); // 8pm

            return bookingTime >= openingTime &&
                   bookingTime <= closingTime;
        }
    }
    

    //ViewModel for Creating Booking as a Customer

    public class CustomerCreateBookingViewModel
    {

        //View Model attributes here
        

        [Required]
        [Display(Name = "Date Of Booking")]
        [FutureOrTodayDate(ErrorMessage = "Bookings must be tomorrow or later, between 8am and 8pm")]  //custom verify to ensure that the booking is today onwards and between 8am and 8pm.
        public DateTime DateOfBooking { get; set; }

        [Display(Name = "Completion Status")]
        public bool CompletionStatus { get; set; }

        //One to Many | one Vehicle has many Bookings
        [Required]
        [ForeignKey("Vehicle")]
        [Display(Name = "Vehicle Registration Number")]
        [MaxLength(8)]
        public string VehicleRegNO { get; set; }

        // Multiple tick boxes for each JobType the User may want
        // Must choose one
        [Required]
        public bool IsJobType1 { get; set; }
        [Required]
        public bool IsJobType2 { get; set; }
        [Required]
        public bool IsJobType3 { get; set; }
        [Required]
        public bool IsJobType4 { get; set; }
        [Required]
        public bool IsJobType5 { get; set; }
        [Required]
        public bool IsJobType6 { get; set; }
        [Required]
        public bool IsJobType7 { get; set; }
        [Required]
        public bool IsJobType8 { get; set; }
        [Required]
        public bool IsJobType9 { get; set; }
        [Required]
        public bool IsJobType10 { get; set; }



    }

    public class StaffCreateBookingViewModel
    {

        //View Model attributes here
        [Required]
        public string CustomerId { get; set; }

        [Required]
        [Display(Name = "Date Of Booking")]
        [FutureOrTodayDate(ErrorMessage = "Bookings must be tomorrow or later, between 8am and 8pm")]  //custom verify to ensure that the booking is today onwards and between 8am and 8pm.
        public DateTime DateOfBooking { get; set; }

        [Display(Name = "Completion Status")]
        public bool CompletionStatus { get; set; }

        //One to Many | one Vehicle has many Bookings
        [Required]
        [ForeignKey("Vehicle")]
        [Display(Name = "Vehicle Registration Number")]
        [MaxLength(8)]
        public string VehicleRegNO { get; set; }

        // Multiple tick boxes for each JobType the User may want
        // Must choose one
        [Required]
        public bool IsJobType1 { get; set; }
        [Required]
        public bool IsJobType2 { get; set; }
        [Required]
        public bool IsJobType3 { get; set; }
        [Required]
        public bool IsJobType4 { get; set; }
        [Required]
        public bool IsJobType5 { get; set; }
        [Required]
        public bool IsJobType6 { get; set; }
        [Required]
        public bool IsJobType7 { get; set; }
        [Required]
        public bool IsJobType8 { get; set; }
        [Required]
        public bool IsJobType9 { get; set; }
        [Required]
        public bool IsJobType10 { get; set; }



    }

    public class UpdateStatusViewModel
    {
        [Required]
        public int JobId { get; set; }
        [Required]
        [Display(Name = "")]
        public DateTime CompletionDate { get; set; }

        public string JobDescription { get; set; }

    }

    //Create a new Staff
    public class CreateStaffViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } //Email feild

        [Required]
        [Display(Name = "Full Name:")] //Display Name to make easier to read when displayed
        [MaxLength(60)]//Max length for the name feild is here
        public string Name { get; set; }

        [MaxLength(30)]
        [Display(Name = "Position:")]
        public string Position { get; set; } //Staffs position at the company i.e mechanic, manager
        [Required]
        [MaxLength(100)]
        [Display(Name = "Address:")]
        public string Address { get; set; } //Staffs Address
        [Required]
        [Display(Name = "Date of Birth:")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2010", ErrorMessage = "Invalid Date of Birth")]
        public DateTime DateOfBirth { get; set; } //Staffs Address

        //Password feilds
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
    //used to allow managers to edit staff
    public class EditStaffViewModel
    {
        [Required]
        public string UserId { get; set; } //Key feild

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } //Email feild

        [Required]
        [Display(Name = "Full Name:")] //Display Name to make easier to read when displayed
        [MaxLength(60)]//Max length for the name feild is here
        public string Name { get; set; }

        [MaxLength(30)]
        [Display(Name = "Position:")]
        public string Position { get; set; } //Staffs position at the company i.e mechanic, manager
        [Required]
        [MaxLength(100)]
        [Display(Name = "Address:")]
        public string Address { get; set; } //Staffs Address
        [Required]
        [Display(Name = "Date of Birth:")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2010", ErrorMessage = "Invalid Date of Birth")]
        public DateTime DateOfBirth { get; set; } //Staffs Address

        //Password feilds
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool IsActive { get; set; }

    }


}