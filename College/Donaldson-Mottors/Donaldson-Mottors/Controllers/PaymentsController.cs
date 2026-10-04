using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Donaldson_Mottors.Models;
using Microsoft.AspNet.Identity;

namespace Donaldson_Mottors.Controllers
{
    public class PaymentsController : Controller
    {
        private DonaldsonMottorsDbContext db = new DonaldsonMottorsDbContext();

        // GET: Payments
        public ActionResult Index()
        {
            var payments = db.Payments.Include(p => p.Booking).Include(p => p.Customer);
            return View(payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        // This method will give the User the cost of this booking as well as some paying options.
        // It will create a Payment record for the db
        public ActionResult Create(int bookingId)
        {
            var booking = db.Bookings.Include(b => b.Jobs).FirstOrDefault(b => b.BookingId == bookingId); // get the booking

            //List of all the values sent into the view via viewbag
            ViewBag.BookingId = bookingId; // the booking id
            ViewBag.DateOfBooking = booking.DateOfBooking; //date of booking
            ViewBag.JobDescription = booking.JobDescription; //job descriptions
            ViewBag.TotalCost = booking.TotalCost; //Total Cost
            ViewBag.DepositCost = booking.TotalCost * 0.25;//Deposite Cost
            ViewBag.VehRegNo = booking.VehicleRegNO; //Vehicle Reg Number
            

            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int bookingId, string actionType)
        {
            var userId = User.Identity.GetUserId();
            var booking = db.Bookings
                            .Include(b => b.Jobs)
                            .FirstOrDefault(b => b.BookingId == bookingId); //find the booking
            if (actionType == "Cancel")
            {
                booking.Status = "Cancelled";
                TempData["CreateBookingCancel"] = "Cancel";
                return RedirectToAction("Index", "Home");
            }

                
            if(actionType == "Full")
            {
                var payment = new Payment() //create the Payment Record
                {
                    PaymentDateTime = DateTime.Now,
                    PaymentMethod = "PayPal",
                    Currency = "Pounds Sterling",
                    Status = "Processing",
                    Amount = booking.TotalCost,
                    BookingId = bookingId,
                    CustomerId = userId,
                };

                booking.AmountPaid = booking.TotalCost; //update the amount paid
                booking.Status = "Paid";//update booking status
                db.Payments.Add(payment);//add the record to the DB
            }
            else if (actionType == "Deposit")
            {
                var payment = new Payment() //create the Payment Record
                {
                    PaymentDateTime = DateTime.Now,
                    PaymentMethod = "PayPal",
                    Currency = "Pounds Sterling",
                    Status = "Processing",
                    Amount = booking.TotalCost * 0.25,
                    BookingId = bookingId,
                    CustomerId = userId,
                };

                booking.AmountPaid = booking.TotalCost * 0.25; //update the amount paid
                booking.Status = "Paid"; //update booking status
                db.Payments.Add(payment);//add the record to the DB
            }
            

            
            

            //Next Update stock table

            foreach (var job in booking.Jobs)
            {
                var stockUsedList = db.StockUsed
                    .Include(s => s.Stock)
                    .Where(s => s.JobTypeId == job.JobTypeId)
                    .ToList(); //list of stock used

                foreach (var stockUsed in stockUsedList)
                {
                    stockUsed.Stock.StockLevel -= stockUsed.Quantity; //update stock

                    if (stockUsed.Stock.StockLevel < 0)
                        stockUsed.Stock.StockLevel = 0; //prevent negitive stock levels
                }
            }

            
            db.SaveChanges();
            TempData["CreateBookingSuccess"] = "test";
            return RedirectToAction("Index", "Home");
        }

        //// GET: Payments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Payment payment = db.Payments.Find(id);
        //    if (payment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "CustomerId", payment.BookingId);
        //    ViewBag.CustomerId = new SelectList(db.Users, "Id", "Name", payment.CustomerId);
        //    return View(payment);
        //}

        //// POST: Payments/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PaymentId,PaymentMethod,Amount,Currency,PaymentDateTime,Status,PaymentGatewayTransactionReference,BookingId,CustomerId")] Payment payment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(payment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "CustomerId", payment.BookingId);
        //    ViewBag.CustomerId = new SelectList(db.Users, "Id", "Name", payment.CustomerId);
        //    return View(payment);
        //}

        //// GET: Payments/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Payment payment = db.Payments.Find(id);
        //    if (payment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(payment);
        //}

        //// POST: Payments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Payment payment = db.Payments.Find(id);
        //    db.Payments.Remove(payment);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
