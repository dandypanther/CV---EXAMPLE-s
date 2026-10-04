using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Donaldson_Mottors.Models
{
    // User acts as a parent class to Customer and Staff
    public abstract class User : IdentityUser
    {
        //shared attributes between Customer and Staff Stored here
        [Required]
        [Display(Name = "Full Name")] //Display Name to make easier to read when displayed
        [MaxLength(60)]//Max length for the name feild is here
        public string Name { get; set; } //Name feild is common to both Staff and Customer so it is stored in parent class

        [Required]
        [Display(Name = "Is Account Active:")]
        public bool IsActive { get; set; } // This feild is used to convey wither or not a User is still registered

        [NotMapped]
        public string CurrentRole { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    
}