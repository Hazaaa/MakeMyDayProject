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

        public IEnumerable<string> followers { get; set; }
        public IEnumerable<string> followed { get; set; }

        public int numberofposts { get; set; }

        public string profilepictureurl { get; set; }

        public User()
        {
            profilepictureurl = "~/Resource/defaultpic.jpg";
        }
    }
}
