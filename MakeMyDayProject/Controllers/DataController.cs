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
using Databases.Redis;

namespace MakeMyDayProject.Controllers
{
    public class DataController : ApiController
    {
        Neo4jFunctions neo4j = new Neo4jFunctions();
        RedisFunctions redis = new RedisFunctions();

        #region GET

        [Route("api/get-posts/{skip}")]
        [HttpGet]
        public IEnumerable<Post> GetPosts(int skip)
        {
            try
            {
                return neo4j.GetAllPosts(skip);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [Route("api/get-latest-posts")]
        [HttpGet]
        public IEnumerable<Post> GetLatestPosts()
        {
            try
            {
                return redis.GetLatestPosts();
            }
            catch (Exception)
            {
                return null;
            }
        }


        [Route("api/get-post/{id}")]
        [HttpGet]
        public Post GetPost(string id)
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


        [Route("api/get-post-comments/{id}")]
        [HttpGet]
        public IEnumerable<Comment> GetPostComments(string id)
        {
            try
            {
                return neo4j.GetPostComments(id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [Route("api/get-user-posts")]
        [HttpGet]
        public IEnumerable<Post> GetAllUserPosts(string username, int skip)
        {
            try
            {
                return neo4j.GetAllPosts(skip, username);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/get-user-hashtags/{username}")]
        [HttpGet]
        public IEnumerable<Hashtag> GetUserHashtags(string username)
        {
            try
            {
                return redis.GetUserHashtags(username);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/get-hashtags")]
        [HttpGet]
        public IEnumerable<Hashtag> GetAllHashtags()
        {
            try
            {
                return neo4j.GetHashtags();
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/get-user-activities/{username}")]
        [HttpGet]
        public IEnumerable<string> GetUserActivities(string username)
        {
            try
            {
                return redis.GetUserLatestActivities(username);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/get-posts-with-hashtag/{hashtag}")]
        [HttpGet]
        public IEnumerable<Post> GetPostsWithHashtag(string hashtag)
        {
            try
            {
                return neo4j.GetPostsWithHashtag(hashtag);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/get-top-rated-posts")]
        [HttpGet]
        public IEnumerable<Post> GetTopRatedPosts()
        {
            try
            {
                var posts = redis.GetTopRatedPosts();
                if (posts != null)
                    return posts;
                else
                {
                    var topPosts = neo4j.GetTopRatedPosts();
                    redis.PushTopRatedPosts(topPosts);

                    return topPosts;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/get-all-users")]
        [HttpGet]
        public IEnumerable<string> GetAllUsers()
        {
            try
            {
                return redis.GetAllUsernamesForSearch();
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/get-all-messages/{reciever}")]
        [HttpGet]
        public IEnumerable<Message> GetAllMessages(string reciever)
        {
            try
            {
                return neo4j.GetAllMessagesBetweenUsers(User.Identity.Name, reciever);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/count-unreaded-messages")]
        [HttpGet]
        public double CountUnreadedMessages()
        {
            try
            {
                var unreadedMessagesValues = redis.GetAllUnereadedMessagesInfo(User.Identity.Name).Values.ToList().Sum();

                return unreadedMessagesValues;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #region POST

        [Route("api/upload-post")]
        [HttpPost]
        public Post UploadPost()
        {
            try
            {
                // Deserializing json to DataPostModel
                var post = JsonConvert.DeserializeObject<DataPostModel>(HttpContext.Current.Request["post"]);

                // Getting picture from post request
                HttpPostedFile uploadedPicture = HttpContext.Current.Request.Files["uploadedPicture"];

                // This post will be saved in redis
                Post redisLatestPost = new Post();

                // Getting logged user asp id and username that are in sql db
                var userId = User.Identity.GetUserId();
                var userName = User.Identity.GetUserName();

                User creator = neo4j.GetUser(userName);
                redisLatestPost.creatorPict = creator.profilepictureurl;

                Post newPost = new Post
                {
                    content = post.Text
                };

                Posted newPosted = new Posted
                {
                    post = newPost,
                    user = creator,
                    time = DateTime.Now.ToString()
                };

                neo4j.CreatePostedRelationship(newPosted);
                redis.PushLatestActivity(userName, "You added new post.");

                // Setting properties for redis post
                redisLatestPost.id = newPost.id;
                redisLatestPost.creator = creator;
                redisLatestPost.content = newPost.content;
                redisLatestPost.timeCreated = DateTime.Now.ToString();

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

                    redisLatestPost.pictureurl = newPicture.url;
                }

                // Adding hashtags if there are any
                if (post.Hashtags.Count != 0)
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

                        redis.PushUserHashtag(userName, newHashtag);
                    }
                }

                // Adding tags if there are any
                if (post.Tags.Count != 0)
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

                // Adding post to redis
                redis.PushLatestPost(redisLatestPost);

                return redisLatestPost;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/upload-comment")]
        [HttpPost]
        public Comment UploadComment([FromBody] DataCommentModel comment)
        {
            try
            {
                Post post = new Post { id = new Guid(comment.postId) };
                User user = new User { username = User.Identity.GetUserName() };
                Comment newComment = new Comment { text = comment.text, creator = user, time = DateTime.Now.ToString() };

                redis.PushLatestActivity(user.username, "You commented post: " + comment.postId); 

                if (neo4j.CreateCommentRelationships(new HasComment
                {
                    comment = newComment,
                    post = post,
                    time = DateTime.Now.ToString()
                }, new Commented
                {
                    comment = newComment,
                    commentator = user,
                    time = DateTime.Now.ToString()
                }))
                    return newComment;
                else
                    return null;

            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/like-post")]
        [HttpPost]
        public string LikePost()
        {
            try
            {
                var postId = HttpContext.Current.Request["id"];
                var user = HttpContext.Current.Request["username"];

                Post likedPost = new Post { id = new Guid(postId) };
                User liker = new User { username = user };

                redis.PushLatestActivity(user, "You liked post: " + postId);

                neo4j.CreateLikedDislikedRelationship(new Databases.Neo4j.DomainModel.Relationships.Liked
                {
                    post = likedPost,
                    user = liker,
                    time = DateTime.Now.ToString()
                });

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/dislike-post")]
        [HttpPost]
        public string DislikePost()
        {
            try
            {
                var postId = HttpContext.Current.Request["id"];
                var user = HttpContext.Current.Request["username"];

                Post dislikedPost = new Post { id = new Guid(postId) };
                User disliker = new User { username = user };

                redis.PushLatestActivity(user, "You disliked post: " + postId);

                neo4j.CreateLikedDislikedRelationship(null, new Databases.Neo4j.DomainModel.Relationships.Disliked
                {
                    post = dislikedPost,
                    time = DateTime.Now.ToString(),
                    user = disliker
                });

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/follow-user")]
        [HttpPost]
        public string FollowUser()
        {
            try
            {
                var userToFollow = HttpContext.Current.Request["userToFollow"];

                neo4j.CreateFollowRelationship(new Databases.Neo4j.DomainModel.Relationships.Follow
                {
                    followed = new User { username = userToFollow },
                    follower = new User { username = User.Identity.Name},
                    time = DateTime.Now.ToString()
                });
                redis.PushLatestActivity(User.Identity.Name, "You are following " + userToFollow + ".");
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/unfollow-user")]
        [HttpPost]
        public string UnfollowUser()
        {
            try
            {
                var userToUnfollow = HttpContext.Current.Request["userToUnfollow"];

                neo4j.DeleteFollowRelationship(User.Identity.Name, userToUnfollow);

                redis.PushLatestActivity(User.Identity.Name,"You unfollowed " + userToUnfollow + ".");
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/upload-profile-picture")]
        [HttpPost]
        public string UploadProfilePicture()
        {
            try
            {
                HttpPostedFile uploadedPicture = HttpContext.Current.Request.Files["uploadedPicture"];

                var username = User.Identity.Name;

                var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Resource/UploadedPictures"), "profilepic-" + username + "." + uploadedPicture.FileName.Split('.')[1]);

                // Saving picture to "server"
                uploadedPicture.SaveAs(fileSavePath);

                neo4j.UpdateProfilePicture(username, "~/Resource/UploadedPictures/profilepic-" + username + "." + uploadedPicture.FileName.Split('.')[1]);

                redis.PushLatestActivity(username, "You've changed your profile picture.");

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/send-message")]
        [HttpPost]
        public Message SendMessage()
        {
            try
            {
                var sender = neo4j.GetUser(User.Identity.Name);
                var reciever =  neo4j.GetUser(HttpContext.Current.Request["reciever"]);
                var messageText = HttpContext.Current.Request["text"];

                Message message = new Message { text = messageText };

                neo4j.CreateSendRecieveRelationships(new Databases.Neo4j.DomainModel.Relationships.Send { message = message, sender = sender, time = DateTime.Now.ToString() },
                    new Databases.Neo4j.DomainModel.Relationships.Recieve { message = message, reciever = reciever, time = DateTime.Now.ToString() });

                redis.PushNewUnreadMessage(reciever.username, User.Identity.Name);

                message.timesent = DateTime.Now.ToString();

                redis.PushLatestActivity(reciever.username, "You recieved new message from " + User.Identity.Name + ".");

                return message;   
            }
            catch (Exception)
            {

                return null;

            }
        }

        #endregion

        #region PUT

        [Route("api/updatecomment")]
        [HttpPut]
        public string UpdateComment(DataCommentModel comment)
        {
            try
            {
                neo4j.UpdateComment(comment.commentid, comment.text);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region DELETE

        [Route("api/delete-comment/{id}")]
        [HttpDelete]
        public string DeleteComment(string id)
        {
            try
            {
                neo4j.DeleteComment(id);

                redis.PushLatestActivity(User.Identity.Name, "You deleted your comment: " + id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Route("api/delete-post/{id}")]
        [HttpDelete]
        public string DeletePost(string id)
        {
            try
            {
                redis.DeletePost(id);
                neo4j.DeletePost(id);

                redis.PushLatestActivity(User.Identity.Name, "You deleted your post: " + id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

    }
}