using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Databases.Neo4j;
using Databases.Neo4j.DomainModel;
using Databases.Redis;

namespace MakeMyDayProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                Neo4jFunctions neo4j = new Neo4jFunctions();

                if (Request.IsAuthenticated)
                    return View(neo4j.GetUser(User.Identity.Name));
                else
                    return View();
            }
            catch (Exception ex)
            {
                return View("CustomError", ex);
            } 
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