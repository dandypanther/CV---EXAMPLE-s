using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Donaldson_Mottors.Models;
using Microsoft.AspNet.Identity;

namespace Donaldson_Mottors.Controllers
{
    public class BookingsController : Controller
    {
        private DonaldsonMottorsDbContext db = new DonaldsonMottorsDbContext();

        //Customer Booking Index
        [Authorize]
        public ActionResult CustomerIndex()
        {
            var userId = User.Identity.GetUserId();
            var bookings = db.Bookings.Include(b => b.Customer).Include(b => b.Vehicle).Include(b=>b.Payment).Where(b=>b.CustomerId == userId && b.Status != "Cancelled");
            return View(bookings.ToList());
        }


        // GET: Bookings
        [Authorize(Roles = "Admin, Manager, Stock-Controller, Accounts-Clerk, Mechanic")] //only staff
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Customer).Include(b => b.Vehicle);
            return View(bookings.ToList());
        }


        // GET: Bookings/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        // Customer version
        [Authorize]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId(); //get logged in user Id

            // get vehicles belonging to this customer
            var customerVehicles = db.Vehicles
                .Where(v => v.CustomerId == userId)
                .ToList();
            // dropdown list of Customer Vehicles
            // dropdown list of Customer Vehicles
            ViewBag.VehicleRegNO = new SelectList(
                customerVehicles,
                "VehicleRegNO",
                "VehicleRegNO"
            );

            return View(); //give the view to the User
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //Post for user creating a booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerCreateBookingViewModel booking)
        {
            var userId = User.Identity.GetUserId(); //get userId
            if (ModelState.IsValid)
            {
                //If no jobs are selected then return view model
                if (!(booking.IsJobType1 || booking.IsJobType2 || booking.IsJobType3 ||
                      booking.IsJobType4 || booking.IsJobType5 || booking.IsJobType6 ||
                      booking.IsJobType7 || booking.IsJobType8 || booking.IsJobType9 ||
                      booking.IsJobType10))
                {
                    ModelState.AddModelError("", "Please select at least one job from the list.");
                    // get vehicles belonging to this customer
                    var Id = User.Identity.GetUserId(); //get userId
                    var cusVehicles = db.Vehicles
                        .Where(v => v.CustomerId == Id)
                        .ToList();
                    // dropdown list of Customer Vehicles
                    // dropdown list of Customer Vehicles
                    ViewBag.VehicleRegNO = new SelectList(
                        cusVehicles,
                        "VehicleRegNO",
                        "VehicleRegNO"
                    );
                    return View(booking);
                }
                else if (db.Bookings.Any(b => b.DateOfBooking == booking.DateOfBooking))
                {
                    ModelState.AddModelError("", "A booking already exists at this date/time, please select another time.");
                    // get vehicles belonging to this customer
                    var Id = User.Identity.GetUserId(); //get userId
                    var cusVehicles = db.Vehicles
                        .Where(v => v.CustomerId == Id)
                        .ToList();
                    // dropdown list of Customer Vehicles
                    // dropdown list of Customer Vehicles
                    ViewBag.VehicleRegNO = new SelectList(
                        cusVehicles,
                        "VehicleRegNO",
                        "VehicleRegNO"
                    );
                    return View(booking);
                }

                //Create the New booking
                var newBooking = new Booking()
                {
                    DateOfBooking = booking.DateOfBooking,
                    CompletionStatus = false,
                    VehicleRegNO = booking.VehicleRegNO,
                    CustomerId = userId,
                    Status = "Pending Payment"
            };

                db.Bookings.Add(newBooking); //add the new booking to the DB

                //Create any new Jobs
                // This list will connect isJobType to the JobType it represents
                // This will be used to filter out jobs not relevent to this booking
                var jobsToCreate = new List<(bool selected, string jobTypeId)>
                {
                    (booking.IsJobType1, "JOB-2026-0001"),
                    (booking.IsJobType2, "JOB-2026-0002"),
                    (booking.IsJobType3, "JOB-2026-0003"),
                    (booking.IsJobType4, "JOB-2026-0004"),
                    (booking.IsJobType5, "JOB-2026-0005"),
                    (booking.IsJobType6, "JOB-2026-0006"),
                    (booking.IsJobType7, "JOB-2026-0007"),
                    (booking.IsJobType8, "JOB-2026-0008"),
                    (booking.IsJobType9, "JOB-2026-0009"),
                    (booking.IsJobType10, "JOB-2026-0010")
                };

                var jobDesc = ""; //will be used to create the Job Description for Bookings
                var firstDesc = true; //used to format jobdesc
                var totalCost = 0.00; //used to create the total cost of the booking

                // This foreach will loop for every job the User wants
                foreach (var job in jobsToCreate.Where(j => j.selected))
                {
                    var jobType = db.JobTypes.Include("StockUseds.Stock").FirstOrDefault(j => j.JobTypeId == job.jobTypeId);//get jobtype and include the stockused data and stock data
                    totalCost += + (jobType.NoOfHours * 60);

                    foreach (var stockUsed in jobType.StockUseds) //loop for every stock used in jobtype
                    {
                        totalCost += (stockUsed.Stock.Price * stockUsed.Quantity); // get cost by price times amount
                    }

                    if (firstDesc == false)
                    {
                        jobDesc = jobDesc + ", " + jobType.JobDescription; //format jobDescription
                    }
                    else
                    {
                        jobDesc = jobDesc + jobType.JobDescription; //format jobDescription
                    }
                    // Creates new Job, attaches Jobtype to the job
                    db.Jobs.Add(new Job
                    {
                        JobTypeId = job.jobTypeId,
                        Booking = newBooking
                    });
                }

                jobDesc = jobDesc + ".";
                newBooking.JobDescription = jobDesc;
                newBooking.TotalCost = totalCost;

                db.SaveChanges(); //update db

                return RedirectToAction("Create", "Payments", new { bookingId = newBooking.BookingId });
            }
            
            // get vehicles belonging to this customer
            
            var customerVehicles = db.Vehicles
                .Where(v => v.CustomerId == userId)
                .ToList();
            // dropdown list of Customer Vehicles
            // dropdown list of Customer Vehicles
            ViewBag.VehicleRegNO = new SelectList(
                customerVehicles,
                "VehicleRegNO",
                "VehicleRegNO"
            );
            return View(booking);
        }


        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // GET: Bookings/CreateForCustomer
        // Staff version for making on customer behalf
        [Authorize(Roles = "Admin, Manager, Mechanic")] //only staff
        [HttpGet]
        public ActionResult CreateForCustomer(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get customer from DB
            var customer = db.Users.OfType<Customer>()
                                   .FirstOrDefault(c => c.Id == id);
            var model = new StaffCreateBookingViewModel
            {
                CustomerId = id
            };
            ViewBag.CustomerName = customer.Name;

            // get vehicles belonging to this customer
            var customerVehicles = db.Vehicles
                .Where(v => v.CustomerId == id && v.IsValid == true)
                .ToList();
            // dropdown list of Customer Vehicles
            ViewBag.VehicleRegNO = new SelectList(
                customerVehicles,
                "VehicleRegNO",
                "VehicleRegNO"
            );

            return View(model); //give the view to the User
        }

        // Post: Create For Customer

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager, Mechanic")]
        public ActionResult CreateForCustomer(StaffCreateBookingViewModel booking)
        {
            var userId = booking.CustomerId; //get userId
            if (ModelState.IsValid)
            {
                //If no jobs are selected then return view model
                if (!(booking.IsJobType1 || booking.IsJobType2 || booking.IsJobType3 ||
                      booking.IsJobType4 || booking.IsJobType5 || booking.IsJobType6 ||
                      booking.IsJobType7 || booking.IsJobType8 || booking.IsJobType9 ||
                      booking.IsJobType10))
                {
                    ModelState.AddModelError("", "Please select at least one job from the list.");
                    // get vehicles belonging to this customer
                    var Id = booking.CustomerId; //get userId
                    var cusVehicles = db.Vehicles
                        .Where(v => v.CustomerId == Id)
                        .ToList();
                    // dropdown list of Customer Vehicles
                    // dropdown list of Customer Vehicles
                    ViewBag.VehicleRegNO = new SelectList(
                        cusVehicles,
                        "VehicleRegNO",
                        "VehicleRegNO"
                    );
                    return View(booking);
                }
                else if (db.Bookings.Any(b => b.DateOfBooking == booking.DateOfBooking))
                {
                    ModelState.AddModelError("", "A booking already exists at this date/time, please select another time.");
                    // get vehicles belonging to this customer
                    var Id = booking.CustomerId; //get userId
                    var cusVehicles = db.Vehicles
                        .Where(v => v.CustomerId == Id)
                        .ToList();
                    // dropdown list of Customer Vehicles
                    // dropdown list of Customer Vehicles
                    ViewBag.VehicleRegNO = new SelectList(
                        cusVehicles,
                        "VehicleRegNO",
                        "VehicleRegNO"
                    );
                    return View(booking);
                }

                //Create the New booking
                var newBooking = new Booking()
                {
                    DateOfBooking = booking.DateOfBooking,
                    CompletionStatus = false,
                    VehicleRegNO = booking.VehicleRegNO,
                    CustomerId = userId,
                    Status = "Pending Payment"
                };

                db.Bookings.Add(newBooking); //add the new booking to the DB

                //Create any new Jobs
                // This list will connect isJobType to the JobType it represents
                // This will be used to filter out jobs not relevent to this booking
                var jobsToCreate = new List<(bool selected, string jobTypeId)>
                {
                    (booking.IsJobType1, "JOB-2026-0001"),
                    (booking.IsJobType2, "JOB-2026-0002"),
                    (booking.IsJobType3, "JOB-2026-0003"),
                    (booking.IsJobType4, "JOB-2026-0004"),
                    (booking.IsJobType5, "JOB-2026-0005"),
                    (booking.IsJobType6, "JOB-2026-0006"),
                    (booking.IsJobType7, "JOB-2026-0007"),
                    (booking.IsJobType8, "JOB-2026-0008"),
                    (booking.IsJobType9, "JOB-2026-0009"),
                    (booking.IsJobType10, "JOB-2026-0010")
                };

                var jobDesc = ""; //will be used to create the Job Description for Bookings
                var firstDesc = true; //used to format jobdesc
                var totalCost = 0.00; //used to create the total cost of the booking

                // This foreach will loop for every job the User wants
                foreach (var job in jobsToCreate.Where(j => j.selected))
                {
                    var jobType = db.JobTypes.Include("StockUseds.Stock").FirstOrDefault(j => j.JobTypeId == job.jobTypeId);//get jobtype and include the stockused data and stock data
                    totalCost += +(jobType.NoOfHours * 60);

                    foreach (var stockUsed in jobType.StockUseds) //loop for every stock used in jobtype
                    {
                        totalCost += (stockUsed.Stock.Price * stockUsed.Quantity); // get cost by price times amount
                    }

                    if (firstDesc == false)
                    {
                        jobDesc = jobDesc + ", " + jobType.JobDescription; //format jobDescription
                    }
                    else
                    {
                        jobDesc = jobDesc + jobType.JobDescription; //format jobDescription
                    }
                    // Creates new Job, attaches Jobtype to the job
                    db.Jobs.Add(new Job
                    {
                        JobTypeId = job.jobTypeId,
                        Booking = newBooking
                    });
                }

                jobDesc = jobDesc + ".";
                newBooking.JobDescription = jobDesc;
                newBooking.TotalCost = totalCost;

                // Update Stock
                foreach (var job in newBooking.Jobs)
                {
                    var stockUsedList = db.StockUsed
                        .Include(s => s.Stock)
                        .Where(s => s.JobTypeId == job.JobTypeId)
                        .ToList();

                    foreach (var stockUsed in stockUsedList)
                    {
                        stockUsed.Stock.StockLevel -= stockUsed.Quantity;

                        if (stockUsed.Stock.StockLevel < 0)
                            stockUsed.Stock.StockLevel = 0;
                    }
                }

                db.SaveChanges(); //update db

                return RedirectToAction("Index", "Home");
            }

            // get vehicles belonging to this customer

            var customerVehicles = db.Vehicles
                .Where(v => v.CustomerId == userId)
                .ToList();
            // dropdown list of Customer Vehicles
            // dropdown list of Customer Vehicles
            ViewBag.VehicleRegNO = new SelectList(
                customerVehicles,
                "VehicleRegNO",
                "VehicleRegNO"
            );
            return View(booking);
        }



        // GET: Bookings/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var booking = db.Bookings
                            .Include(b => b.Jobs)
                            .FirstOrDefault(b => b.BookingId == id);//get booking with jobs

            booking.Status = "Cancelled"; //update status

            // Restore stock
            foreach (var job in booking.Jobs)
            {
                var stockUsedList = db.StockUsed
                    .Include(s => s.Stock)
                    .Where(s => s.JobTypeId == job.JobTypeId)
                    .ToList(); //get stock used

                foreach (var stockUsed in stockUsedList)
                {
                    stockUsed.Stock.StockLevel += stockUsed.Quantity; //add in what was removed
                }
            }



            db.SaveChanges();
            return RedirectToAction("Index", "Home");
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
