using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Databases.Neo4j.DomainModel;
using Databases.Neo4j;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using MakeMyDayProject.Models;
using System.Web;
using Microsoft.Ajax.Utilities;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNet.Identity;

namespace MakeMyDayProject.Controllers
{
    public class DataController : ApiController
    {
        Neo4jFunctions neo4j = new Neo4jFunctions();

        [Route("api/get-all-posts")]
        [HttpGet]
        public IEnumerable<Post> GetAllPosts()
        {
            return null;
        }


        [Route("test")]
        [HttpGet]
        public User Test()
        {
            return neo4j.GetUser("admin");
        }


        [Route("api/get-post/{id}")]
        [HttpGet]
        public Post GetPost(int id)
        {
            try
            {
                return neo4j.GetPost(id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [Route("api/get-user-posts/{username}")]
        [HttpGet]
        public IEnumerable<Post> GetAllUserPosts(string username)
        {
            try
            {
                return neo4j.GetUserPosts(username);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [Route("api/upload-post")]
        [HttpPost]
        public string UploadPost()
        {
            try
            {
                // Deserializing json to DataPostModel
                var post = JsonConvert.DeserializeObject<DataPostModel>(HttpContext.Current.Request["post"]);

                // Getting picture from post request
                HttpPostedFile uploadedPicture = HttpContext.Current.Request.Files["uploadedPicture"];
                

                // Getting logged user asp id and username that are in sql db
                var userId = User.Identity.GetUserId();
                var userName = User.Identity.GetUserName();

                User creator = new User
                {
                    aspid = userId,
                    username = userName
                };

                Post newPost = new Post
                {
                    content = post.Text
                };

                neo4j.CreatePostedRelationship(new Posted
                {
                    post = newPost,
                    user = creator,
                    time = DateTime.Now.ToString()
                });


                // Upload picture if user added picture
                if (uploadedPicture != null)
                {
                    var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Resource/UploadedPictures"), "postpic-" + newPost.id.ToString() + "." + uploadedPicture.FileName.Split('.')[1]);

                    // Saving picture to "server"
                    uploadedPicture.SaveAs(fileSavePath);

                    Picture newPicture = new Picture
                    {
                        url = "~/Resource/UploadedPictures/" + "postpic-" + newPost.id.ToString() + "." + uploadedPicture.FileName.Split('.')[1]
                    };

                    // Creating relationship between post and picture
                    neo4j.CreatePictureRelationship(null, new Databases.Neo4j.DomainModel.Relationships.HasPicture
                    {
                        picture = newPicture,
                        post = newPost,
                        time = DateTime.Now.ToString()
                    });
                }
                
                if(post.Hashtags.Count != 0)
                {
                    foreach (var hashtag in post.Hashtags)
                    {
                        Hashtag newHashtag = new Hashtag
                        {
                            text = hashtag
                        };

                        neo4j.CreateHashtagRelationship(new Databases.Neo4j.DomainModel.Relationships.HasHashtag
                        {
                            hashtag = newHashtag,
                            post = newPost,
                            time = DateTime.Now.ToString()
                        });
                    }
                }

                if(post.Tags.Count != 0)
                {
                    foreach (var tag in post.Tags)
                    {
                        neo4j.CreateTaggedRelationship(new Databases.Neo4j.DomainModel.Relationships.Tagged
                        {
                            post = newPost,
                            tagged = new User
                            {
                                username = tag.Split('@')[1]
                            },
                            time = DateTime.Now.ToString()
                        });
                    }
                }

                return userId;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}