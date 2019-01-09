using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel
{
    public class Post
    {

        public Guid id { get; set; }
        public string content { get; set; }

        public int likes { get; set; }
        public IEnumerable<string> wholiked { get; set; }
        public int dislikes { get; set; }
        public IEnumerable<string> whodisliked { get; set; }

        public IEnumerable<Comment> comments { get; set; }

        public string pictureurl { get; set; }

        public IEnumerable<Hashtag> hashtags { get; set; }

        public string timeCreated { get; set; }

        public User creator { get; set; }
        public string creatorPict { get; set; }

        public IEnumerable<string> taggedusers { get; set; }

        public Post()
        {
            id = Guid.NewGuid();
            likes = 0;
            dislikes = 0;
            content = "";
        }
    }
}
