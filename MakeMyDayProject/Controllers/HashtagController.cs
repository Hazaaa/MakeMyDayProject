using Databases.Neo4j;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MakeMyDayProject.Controllers
{
    public class HashtagController : Controller
    {
        // GET: Hashtag
        [Authorize]
        public ActionResult Index(string hashtag)
        {
            try
            {
                ViewBag.hashtag = hashtag;
                return View();
            }
            catch (Exception ex)
            {
                return View("CustomError", ex);
            }

        }
    }
}