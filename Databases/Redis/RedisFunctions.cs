using Databases.Neo4j.DomainModel;
using ServiceStack.Redis;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Redis
{
    public class RedisFunctions
    {

        readonly RedisClient redis = new RedisClient(RedisConfig.SingleHost);

        // Clears redis db
        public bool RemoveAll()
        {
            redis.FlushAll();
            return (redis.GetAllKeys().Count == 0) ? true : false;
        }

        // 10 latest posts that are first visible on page will be saved in redis
        public void PushLatestPost(Post newPost)
        {
            // Pushes new post on start of the list
            redis.EnqueueItemOnList("latestposts", JsonSerializer.SerializeToString<Post>(newPost));

            // List only need to store 10 posts so we have to trim always last one if number of posts is > 10
            redis.TrimList("latestposts", 0, 9);
        }

        // Return all latest posts from redis (always returns 10)
        public List<Post> GetLatestPosts()
        {
            List<Post> latestposts = new List<Post>();

            foreach (string jsonstring in redis.GetAllItemsFromList("latestposts"))
            {
                Post p = (Post)JsonSerializer.DeserializeFromString(jsonstring, typeof(Post));
                latestposts.Add(p);
            }

            return latestposts;
        }

        // Checking if post is in list and deleting if is
        public bool DeletePost(string id)
        {
            //count > 0: Remove elements equal to value moving from head to tail.
            //count < 0: Remove elements equal to value moving from tail to head.
            //count = 0: Remove all elements equal to value.
            long result = 0;
            foreach (string jsonstring in redis.GetAllItemsFromList("latestposts"))
            {
                var splitedId = id.Split('-');
                if(jsonstring.Contains(String.Join("", splitedId)))
                {
                    result = redis.RemoveItemFromList("latestposts", jsonstring);
                }
            }

            if (result == 0)
                return false;
            else
                return true;
        }

        // Saving hashtag that user used
        public void PushUserHashtag(string username, Hashtag newHashtag)
        {
            // Adding hashtag that user used to list and it will be unique for every user
            redis.EnqueueItemOnList("user:" + username + ":hashtags", JsonSerializer.SerializeToString<Hashtag>(newHashtag));

            // List will only save last 10 used hashtags
            redis.TrimList("user:" + username + ":hashtags", 0, 9);
        }

        // Getting users hashtags
        public List<Hashtag> GetUserHashtags(string username)
        {
            List<Hashtag> hashtags = new List<Hashtag>();

            foreach(string jsonstring in redis.GetAllItemsFromList("user:" + username + ":hashtags"))
            {
                Hashtag h = (Hashtag)JsonSerializer.DeserializeFromString(jsonstring, typeof(Hashtag));

                hashtags.Add(h);
            }

            return hashtags;
        }

        // Saving latest activities of user
        public void PushLatestActivity(string username, string activity)
        {
            // Adding latest activities to user list in redis
            //redis.EnqueueItemOnList("user:" + username + ":latestactivities", JsonSerializer.SerializeToString(activity, activity.GetType()));

            redis.EnqueueItemOnList("user:" + username + ":latestactivities", activity);

            // List will only save last 10 acitvities
            redis.TrimList("user:" + username + ":latestactivities", 0, 9);
        }

        // Getting user latest activities
        public List<string> GetUserLatestActivities(string username)
        {
            List<string> activities = new List<string>();

            foreach (string activity in redis.GetAllItemsFromList("user:" + username + ":latestactivities"))
            {
                activities.Add(activity);
            }

            return activities;
        }

        // Saving top rated posts
        public void PushTopRatedPosts(IEnumerable<Post> posts)
        {
            foreach (var post in posts)
            {
                redis.AddItemToList("topratedposts", JsonSerializer.SerializeToString(post, typeof(Post)));
            }

            // List will expire every 60min so data is always up to date
            redis.Expire("topratedposts", 60);
        }

        // Getting top rated posts
        public List<Post> GetTopRatedPosts()
        {
            if (redis.Exists("topratedposts") == 1)
            {
                List<Post> topRatedPosts = new List<Post>();

                foreach (string post in redis.GetAllItemsFromList("topratedposts"))
                {
                    Post p = (Post)JsonSerializer.DeserializeFromString(post, typeof(Post));

                    topRatedPosts.Add(p);
                }

                return topRatedPosts;
            }
            else
                return null;
        }

        // Saving username when user is registered so search is faster
        public void PushUsernameToSearchList(string username)
        {
            redis.AddItemToList("usersforsearch", username);
        }

        // Getting all usernames for search
        public List<string> GetAllUsernamesForSearch()
        {
            return redis.GetAllItemsFromList("usersforsearch");
        }

        // Saving username of user that sended a message so user reciever can be informed how many unreaded messages he has and from whom
        public void PushNewUnreadMessage(string user, string fromUser)
        {
            // With this redis will try to find this user in sorted set and increment it with 1 but if it doesn't exists he will add it as new one with score 1
            redis.IncrementItemInSortedSet("user:" + user + ":unreadmessages", fromUser, 1);
        }

        // Getting all number of unreaded messages with users who sent them
        public IDictionary<string, double> GetAllUnereadedMessagesInfo(string user)
        {
            return redis.GetAllWithScoresFromSortedSet("user:" + user + ":unreadmessages");
        }

        // When unreaded messages are read then we set score to 0 for that user
        public void RemoveUnreadMessages(string user, string fromUser)
        {
            // ZADD options but only for Redis 3.0.2
            // XX: Only update elements that already exist. Never add elements.
            // NX: Don't update already existing elements. Always add new elements.

            // Because in this redis client isn't available ZADD options then if key exists in list we remove it and then add it with score 0 because there is no update in redis sorted set
            if(redis.RemoveItemFromSortedSet("user:" + user + ":unreadmessages", fromUser))
                redis.AddItemToSortedSet("user:" + user + ":unreadmessages", fromUser, 0);
        }
    }
}
