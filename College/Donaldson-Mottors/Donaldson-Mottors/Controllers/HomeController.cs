using Donaldson_Mottors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;

namespace Donaldson_Mottors.Controllers
{
    public class HomeController : Controller
    {
        private DonaldsonMottorsDbContext db = new DonaldsonMottorsDbContext();
        public ActionResult Index()
        {
            bool notify = false; //used to notify the view that the message should appear
            string notifymsg = "Products:"; //used to send in the message to inform user to deal with low stock
            var stocks = db.Stock.ToList();
            
            foreach (var stock in stocks)
            {
                if(stock.StockLevel < 2)
                {
                    
                    if(notify == false)
                    {
                        notifymsg = notifymsg + " " + stock.ProductName;
                        notify = true;
                    }
                    else
                    {
                        notifymsg = notifymsg + ", " + stock.ProductName;
                    }
                }
            }
            notifymsg = notifymsg + " are all low on stock.\nActions should be taken";

            ViewBag.Notify = notify;
            ViewBag.Notifymsg = notifymsg;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}