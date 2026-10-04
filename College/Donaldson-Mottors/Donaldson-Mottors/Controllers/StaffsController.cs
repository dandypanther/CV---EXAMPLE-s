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
    [Authorize(Roles = "Admin,Manager")]
    public class StaffsController : Controller
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

        // GET: Staffs
        public ActionResult Index()
        {
            var staff = db.Users.OfType<Staff>().Where(c => c.IsActive == true).ToList();
            return View(staff); //get all staff who are active
        }

        // GET: Staffs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var staff = db.Users.OfType<Staff>()
                      .SingleOrDefault(c => c.Id == id);
            if (staff == null) return HttpNotFound(); //if user is not found throw error
            return View(staff); //pass in staff
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            ViewBag.Positions = new SelectList(new List<string>
            {
                "Mechanic",
                "Accounts-Clerk",
                "Stock-Controller"
            }); //different roles

            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Staff()
                {
                    UserName = model.Name,
                    Email = model.Email,
                    Name = model.Name,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    Position = model.Position,
                    IsActive = true
                };

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Position);
                    TempData["AddedStaffSuccess"] = "Staff Added successfully!";//sends a message to index saying "success"
                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }

            ViewBag.Positions = new SelectList(new List<string>
            {
                "Mechanic",
                "Accounts-Clerk",
                "Stock-Controller"
            }); //differnt roles


            return View(model);
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var staff = db.Users.OfType<Staff>()
                     .SingleOrDefault(c => c.Id == id);
            if (staff == null)
            {
                return HttpNotFound();
            }

            var staffToEdit = new EditStaffViewModel()
            {
                UserId = id,
                Email = staff.Email,
                Name = staff.Name,
                Position = staff.Position,
                Address = staff.Address,
                DateOfBirth= staff.DateOfBirth,
                
            };


            return View(staffToEdit);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditStaffViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var staff = db.Users.OfType<Staff>()
                      .SingleOrDefault(c => c.Id == model.UserId);
            if (staff == null) return HttpNotFound(); //if user is not found throw error

            staff.Email = model.Email;
            staff.Name = model.Name;
            staff.Position = model.Position;
            staff.Address = model.Address;
            staff.DateOfBirth = model.DateOfBirth;
            staff.IsActive = model.IsActive;

            //This code below is used to update the Staffs's password
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var result = await UserManager.RemovePasswordAsync(model.UserId); //if new password is to be added

                if (result.Succeeded)
                {
                    await UserManager.AddPasswordAsync(model.UserId, model.Password); //new password added
                }
            }
            var currentRoles = await UserManager.GetRolesAsync(staff.Id); //get current role

            // remove existing roles
            if (currentRoles.Any())
            {
                await UserManager.RemoveFromRolesAsync(staff.Id, currentRoles.ToArray());
            }

            // add new selected role
            await UserManager.AddToRoleAsync(staff.Id, model.Position);
            if (staff.IsActive == false)
            {
                var Jobs = db.Jobs.Where(v => v.StaffId == model.UserId).ToList();
                foreach (var job in Jobs)
                {
                    job.StaffId = null;
                    job.StaffAssigned = null;
                }
            }


            db.SaveChanges(); //updatest the User in the DB

            TempData["ChangedCustomerDetailsSuccess"] = "Customer updated successfully!";//sends a message to index saying "success"
            return RedirectToAction("Index"); //Redirect user to the Index page
        }

        // GET: Staffs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var staff = db.Users.OfType<Staff>()
                     .SingleOrDefault(c => c.Id == id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var staff = db.Users.OfType<Staff>()
                     .SingleOrDefault(c => c.Id == id);
            staff.IsActive = false;
            if (staff.IsActive == false)
            {
                var Jobs = db.Jobs.Where(v => v.StaffId == id).ToList();
                foreach (var job in Jobs)
                {
                    job.StaffId = null;
                    job.StaffAssigned = null;
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
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
