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
    public class JobsController : Controller
    {
        private DonaldsonMottorsDbContext db = new DonaldsonMottorsDbContext();

        // GET: Jobs
        [Authorize] //For Customers
        public ActionResult ViewJobHistory()
        {
            var userId = User.Identity.GetUserId(); //get userid
            var jobs = db.Jobs.Include(j => j.Booking).Include(j => j.JobType).Include(j => j.StaffAssigned).Where(j=>j.Booking.CustomerId == userId); //get users job histories

            return View(jobs.ToList());
        }

        [Authorize(Roles = "Admin, Manager, Mechanic")] //only staff
        public ActionResult ViewJobHistoryStaff()
        {
            var jobs = db.Jobs.Include(j => j.Booking).Include(j => j.JobType).Include(j => j.StaffAssigned);
            return View(jobs.ToList());
        }

        // POST: Jobs/Edit/5 //mark job as complete by adding compeletion date
        [HttpPost]
        public ActionResult UpdateStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var job = db.Jobs
                        .Include(j => j.Booking.Jobs)
                        .FirstOrDefault(j => j.JobId == id);//get jobs
            if (job.Booking == null)
            {
                return HttpNotFound();
            } //check it is not null

            if (job == null)
            {
                return HttpNotFound();
            }

            job.CompletionDate = DateTime.Now; //sets the completion date as now and hence completion status is now complete

            var booking = job.Booking; //get the booking

            bool allJobsComplete = booking.Jobs.All(j => j.CompletionDate != null); //are all jobs complete in booking
            if (allJobsComplete)
            {
                booking.CompletionStatus = true; //if true then booking status is complete
            }

            db.SaveChanges();
            TempData["UpdateStatusSuccess"] = "Success"; //message to viewjobhistorystaff that it worked

            return RedirectToAction("ViewJobHistoryStaff");
            
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JobId,CompletionDate,BookingId,StaffId,JobTypeId")] Job job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "JobDescription", job.BookingId);
            ViewBag.JobTypeId = new SelectList(db.JobTypes, "JobTypeId", "JobDescription", job.JobTypeId);
            ViewBag.StaffId = new SelectList(db.Users, "Id", "Name", job.StaffId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Job job = db.Jobs.Find(id);
            db.Jobs.Remove(job);
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
