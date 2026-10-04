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
using Rotativa;

namespace Donaldson_Mottors.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private DonaldsonMottorsDbContext db = new DonaldsonMottorsDbContext();

        // GET: Invoices
        // For Staff only
        [Authorize(Roles = "Admin, Manager, Stock-Controller, Accounts-Clerk, Mechanic")] //only staff
        public ActionResult Index()
        {
            var invoices = db.Invoices.Include(i => i.Booking);
            return View(invoices.ToList());
        }

        // GET: Invoices
        // For Customers
        //[Authorize(Roles = "Customer")]
        public ActionResult MyInvoices()
        {
            var userid = User.Identity.GetUserId(); //get userid
            var invoices = db.Invoices.Include(i => i.Booking).Include(i => i.Booking.Customer).Where(i=> i.Booking.CustomerId == userid).ToList(); //get users invoices
            return View(invoices.ToList());//pass invoices into view
        }

        // GET: Invoices/ViewInvoice/5
        // Displays the selected Invoice
        public ActionResult ViewInvoice(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get all invoice details including the information found on Booking, jobs, etc...
            Invoice invoice = db.Invoices.Include(i=>i.Booking)
                .Include(i => i.Booking.Customer)
                .Include(i => i.Booking.Vehicle)
                .Include(i => i.Booking.Jobs.Select(j => j.JobType))
                .Include(i => i.Booking.Jobs.Select(j => j.JobType.StockUseds.Select(su => su.Stock)))
                .FirstOrDefault(i=>i.InvoiceNo == id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoices/BookingsToRaise
        [Authorize(Roles = "Admin, Manager, Stock-Controller, Accounts-Clerk, Mechanic")] //only staff
        public ActionResult BookingsToRaise()
        {
            var bookings = db.Bookings.Include(b=>b.Customer).Where(b => !b.Invoice.Any() && b.CompletionStatus == true).ToList();
            return View(bookings);
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager, Stock-Controller, Accounts-Clerk, Mechanic")] //only staff
        public ActionResult Raise(int id)
        {
            var booking = db.Bookings
                            .Include(b => b.Customer)
                            .Include(b => b.Jobs.Select(j => j.JobType.StockUseds.Select(s => s.Stock)))
                            .FirstOrDefault(b => b.BookingId == id); //get teh booking and include its customer, jobs, stockused and the stock information as well

            var labourCost = 0.00;
            var productCost = 0.00;
            var noHrs = 0.00;

            foreach(Job job in booking.Jobs)
            {
                noHrs += job.JobType.NoOfHours;
                labourCost += job.JobType.NoOfHours * job.JobType.LabourRate;
                foreach(StockUsed stockUsed in job.JobType.StockUseds)
                {
                    productCost += stockUsed.Quantity * stockUsed.Stock.Price;
                }
            }
            var invoice = new Invoice()
            {
                CompanyAddress = "145 Clydebank Road. Glasgow United Kingdom, G81 2RX",
                CompanyInfo = "Contact us at: ContactUs@DonaldsonMottors.com\nOr Contact us at : +44 1410 4852 08",
                CustomerName = booking.Customer.Name,
                CustomerAddress = booking.Customer.Street + ", " + booking.Customer.Town + ". " + booking.Customer.Postcode,
                CustomerContactInfo = "Contact Customer at: " + booking.Customer.Email + "\nOr at: " + booking.Customer.TelephoneNo,
                LabourCost = labourCost,
                ProductCost = productCost,
                TotalCost = booking.TotalCost,
                BookingId = id,
                NoHrs = noHrs,
            };//create Invoice to Raise

            db.Invoices.Add(invoice); //add invoice
            db.SaveChanges();//save changes to db
            return RedirectToAction("Index");
        }

        //DownloadInvoice
        // GET: DownloadInvoice //acts as download for invoice to pdf

        public ActionResult DownloadInvoice(int id)
        {
            var invoice = db.Invoices
                            .Include(i => i.Booking)
                            .Include(i => i.Booking.Jobs)
                            .Include(i => i.Booking.Jobs.Select(j => j.JobType))
                            .Include(i => i.Booking.Jobs.Select(j => j.JobType.StockUseds.Select(s => s.Stock)))
                            .FirstOrDefault(i => i.InvoiceNo == id); //get invoice
            if (invoice == null)
            {
                return HttpNotFound();
            }//null invoice check
            return new ViewAsPdf("InvoicePDF", invoice)
            {
                FileName = "InvoicePDF.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait
            };
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
