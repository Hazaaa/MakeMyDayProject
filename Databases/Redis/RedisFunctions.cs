using ServiceStack.Redis;
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

    }
}
