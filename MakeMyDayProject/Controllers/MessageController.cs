using Databases.Neo4j;
using Databases.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MakeMyDayProject.Controllers
{
    public class MessageController : Controller
    {
        RedisFunctions redis = new RedisFunctions();
        Neo4jFunctions neo4j = new Neo4jFunctions();

        // GET: Message
        [Authorize]
        public ActionResult Index(string username)
        {
            try
            {
                if (username == User.Identity.Name)
                    throw new Exception("You can't send message to yourself :D");

                Databases.Neo4j.DomainModel.User userProfile = neo4j.GetUser(username);

                ViewBag.reciever = username;

                var messages = neo4j.GetAllMessagesBetweenUsers(User.Identity.Name, username);

                ViewBag.loggedPicture = neo4j.GetUserProfilePicture(User.Identity.Name).url.Replace('~', ' ');

                redis.RemoveUnreadMessages(User.Identity.Name, username);

                if (userProfile != null)
                    return View(neo4j.GetAllMessagesBetweenUsers(User.Identity.Name, username));
                else
                    throw new Exception("There is no user with that username :(");
            }
            catch (Exception ex)
            {
                return View("CustomError", ex);
            }
        }

        // GET: AllMessages
        [Authorize]
        public ActionResult AllMessages()
        {
            try
            {
                var unreadedMessages = redis.GetAllUnereadedMessagesInfo(User.Identity.Name).ToList();

                unreadedMessages.Reverse();

                return View(unreadedMessages);
            }
            catch (Exception ex)
            {
                return View("CustomError", ex);
            }
        }
    }
}