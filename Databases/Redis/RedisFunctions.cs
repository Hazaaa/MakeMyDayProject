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
    }
}
