using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel.Relationships
{
    public class Liked
    {

        public User user { get; set; }
        public Post post { get; set; }

        public string time { get; set; }

    }

    public class Disliked
    {
        public User user { get; set; }
        public Post post { get; set; }

        public string time { get; set; }
    }
}
