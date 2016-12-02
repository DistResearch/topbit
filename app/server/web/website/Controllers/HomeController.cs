namespace App.Web.Controllers
{
    using System.Web.Mvc;
    using App.Web;

    [NoCache]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Blog()
        {
            return View();
        }

        public ActionResult Weibo()
        {
            return View();
        }

        public ActionResult API()
        {
            return View();
        }

        public ActionResult Work()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("", "Work");
            }
            return View();
        }
    }
}
