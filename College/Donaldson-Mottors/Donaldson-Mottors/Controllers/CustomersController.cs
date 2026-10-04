using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Donaldson_Mottors.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Donaldson_Mottors.Controllers
{
    
    public class CustomersController : Controller
    {
        private DonaldsonMottorsDbContext db = new DonaldsonMottorsDbContext();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // GET: Customers
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk, Mechanic")]//limit access
        public ActionResult Index()
        {
            var customers = db.Users.OfType<Customer>().Where(c=>c.IsActive == true).ToList();
            return View(customers);
        }

        // GET: Customers/Details/5
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk")]//limit access
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customer = db.Users.OfType<Customer>()
                      .SingleOrDefault(c => c.Id == id);
            if (customer == null) return HttpNotFound(); //if user is not found throw error
            return View(customer);
        }

        // GET: Customers/Create
        //this method will allow me to add customers as a staff
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk")]//limit access
        public ActionResult Create()
        {
            return View();
        }


        // Post: Customers/ Create
        //this method will allow me to add customers as a staff
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Customer
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Town = model.Town,
                    Postcode = model.Postcode,
                    Street = model.Street,
                    TelephoneNo = model.TelephoneNo,
                    IsActive = true
                };

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Customer");
                    TempData["AddedCustomerSuccess"] = "Customer Added successfully!";//sends a message to index saying "success"
                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }

            return View(model);
        }

        // GET: Customers/Edit/5
        //this will allow the Staff to update a customers details
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk")]//limit access
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customer = db.Users.OfType<Customer>()
                      .SingleOrDefault(c => c.Id == id);//Find the user to edit
            if (customer == null) return HttpNotFound(); //if user is not found throw error

            var customerToEdit = new StaffChangeDetailsViewModel() // Customer Details to Update
            {
                UserId = id,
                Email = customer.Email,
                Name = customer.Name,
                Street = customer.Street,
                Town = customer.Town,
                Postcode = customer.Postcode,
                TelephoneNo=customer.TelephoneNo,
                IsActive =customer.IsActive
                
            };

            return View(customerToEdit);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk")]//limit access
        public async Task<ActionResult> Edit(StaffChangeDetailsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = db.Users.OfType<Customer>()
                      .SingleOrDefault(c => c.Id == model.UserId);
            if (customer == null) return HttpNotFound(); //if user is not found throw error

            customer.Email = model.Email;
            customer.Name = model.Name;
            customer.Street = model.Street;
            customer.Town = model.Town;
            customer.Postcode = model.Postcode;
            customer.TelephoneNo = model.TelephoneNo;
            customer.IsActive = model.IsActive;

            //This code below is used to update the Customers's password
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var result = await UserManager.RemovePasswordAsync(model.UserId); //if new password is to be added

                if (result.Succeeded)
                {
                    await UserManager.AddPasswordAsync(model.UserId, model.Password); //new password added
                }
            }

            if(customer.IsActive == false)
            {
                var vehicles = db.Vehicles.Where(v=> v.CustomerId == model.UserId && v.IsValid == true).ToList();
                foreach(var vehicle in vehicles)
                {
                    vehicle.IsValid = false;
                }
            }


            db.SaveChanges(); //updatest the User in the DB

            TempData["ChangedCustomerDetailsSuccess"] = "Customer updated successfully!";//sends a message to index saying "success"
            return RedirectToAction("Index"); //Redirect user to the Index page
        }

        // GET: Customers/Delete/5
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk")]//limit access
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customer = db.Users.OfType<Customer>()
                      .SingleOrDefault(c => c.Id == id);
            if (customer == null) return HttpNotFound(); //if user is not found throw error

            

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Accounts-Clerk")]//limit access
        public ActionResult DeleteConfirmed(string id)
        {
            var customer = db.Users.OfType<Customer>()
                      .SingleOrDefault(c => c.Id == id);
            if (customer == null) return HttpNotFound(); //if user is not found throw error
            
            customer.IsActive = false;
            if (customer.IsActive == false)
            {
                var vehicles = db.Vehicles.Where(v => v.CustomerId == id && v.IsValid == true).ToList();
                foreach (var vehicle in vehicles)
                {
                    vehicle.IsValid = false;
                }
            }
            db.SaveChanges();
            TempData["CustomerDelete"] = "Customer Is no Longer Registed";//sends a message to index saying "success"
            return RedirectToAction("Index"); //Redirect user to the Index page
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
