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
    [Authorize]
    public class VehiclesController : Controller
    {
        private DonaldsonMottorsDbContext db = new DonaldsonMottorsDbContext();

        // GET: Vehicles
        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();//gets the Users Id
            var vehicles = db.Vehicles.Include(v => v.Customer).Where(v=> v.CustomerId == userId && v.IsValid == true); //finds all Vehicles owned by the User
            return View(vehicles.ToList());
        }

        // GET: Vehicles/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // GET: Vehicles/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(VehicleViewModel vehicle)
        {
            if (ModelState.IsValid)
            {
                var newVehicle = new Vehicle()
                {
                    VehicleRegNO = vehicle.VehicleRegNO,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Miles = vehicle.Miles,
                    Year = vehicle.Year,
                    EngineSize = vehicle.EngineSize,
                    CustomerId = User.Identity.GetUserId(),
                    IsValid = true
                };

                //Validation that the Vehicle being added has never existed.
                //Since Deleted Vehicles still exist but are not valid they might share the same regNO
                //if a vehicle like this exists it will become active again and be edited to the new inputs
                var doesVehicleExist= db.Vehicles.FirstOrDefault(v=>v.VehicleRegNO == vehicle.VehicleRegNO);
                if (doesVehicleExist == null)
                {
                    db.Vehicles.Add(newVehicle); //add vehicle
                    db.SaveChanges(); //save changes
                    return RedirectToAction("Index"); //redirect to vehicles page
                }
                else
                {
                    //now Update the Vehicle Details
                    doesVehicleExist.Make = vehicle.Make;
                    doesVehicleExist.Model = vehicle.Model;
                    doesVehicleExist.Miles = vehicle.Miles;
                    doesVehicleExist.Year = vehicle.Year;
                    doesVehicleExist.EngineSize = vehicle.EngineSize;
                    doesVehicleExist.CustomerId = User.Identity.GetUserId();
                    doesVehicleExist.IsValid = true;
                    
                    db.SaveChanges(); //save changes
                    return RedirectToAction("Index"); //redirect to vehicles page
                }
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }

            VehicleViewModel vehicleToEdit = new VehicleViewModel()
            {
                VehicleRegNO = vehicle.VehicleRegNO,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Miles = vehicle.Miles,
                Year = vehicle.Year,
                EngineSize = vehicle.EngineSize,
            };

            return View(vehicleToEdit);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(VehicleViewModel vehicle)
        {
            if (ModelState.IsValid)
            {
                var editVehicle = db.Vehicles
                    .FirstOrDefault(v => v.VehicleRegNO == vehicle.VehicleRegNO); //get vehicle

                if (editVehicle == null)
                {
                    return HttpNotFound();
                }

                // update existing vehicle
                editVehicle.Make = vehicle.Make;
                editVehicle.Model = vehicle.Model;
                editVehicle.Miles = vehicle.Miles;
                editVehicle.Year = vehicle.Year;
                editVehicle.EngineSize = vehicle.EngineSize;

                db.SaveChanges(); //save changes

                return RedirectToAction("Index"); //redirect
            }

            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        [Authorize]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(string id)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            vehicle.IsValid = false;
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
