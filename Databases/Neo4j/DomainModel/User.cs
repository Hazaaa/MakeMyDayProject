using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel
{
    public class User
    {
        public string aspid { get; set; }
        public string username { get; set; }

        public int numberoffollowers { get; set; }
        public List<User> followers { get; set; }

        public int numberoffollowed { get; set; }
        public List<User> followed { get; set; }

        public int numberofposts { get; set; }
        public List<Post> posts { get; set; }

        public string profilepictureurl { get; set; }

        public User()
        {
            profilepictureurl = "~/Resources/defaultpic.jpg";
        }
    }
}
