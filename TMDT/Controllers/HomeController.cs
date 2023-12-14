using TMDT.Models;
using System.Web.Mvc;

namespace TMDT.Controllers
{
    public class HomeController : Controller
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: KhachHangVangLai/Home
        public ActionResult Index()
        {
            var lstProduct = db.Combo;
            return View(lstProduct);
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