using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MakeMyDayProject.Controllers
{
    public class TopRatedController : Controller
    {
        // GET: TopRated
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("CustomError", ex);
            }

        }
    }
}