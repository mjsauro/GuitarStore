using GuitarStore.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class ReceiptController : Controller
    {
        private GuitarStoreEntities db = new GuitarStoreEntities();

        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.First(x => x.TrackingNumber == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
            //return View();
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