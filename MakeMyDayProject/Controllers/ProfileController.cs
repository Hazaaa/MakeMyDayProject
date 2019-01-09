using Databases.Neo4j;
using Databases.Neo4j.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MakeMyDayProject.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        [Authorize]
        public ActionResult Index(string username)
        {
            try
            {
                Neo4jFunctions neo4j = new Neo4jFunctions();

                Databases.Neo4j.DomainModel.User userProfile = neo4j.GetUser(username);

                if (userProfile != null)
                    return View(userProfile);
                else
                    throw new Exception("There is no user with that username :(");
            }
            catch (Exception ex)
            {
                return View("CustomError", ex);
            }
            
        }
    }
}