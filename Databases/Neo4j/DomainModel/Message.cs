using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel
{
    public class Message
    {
        public Guid id { get; set; }
        public string text { get; set; }

        public User sender { get; set; }
        public string senderPic { get; set; }

        public User reciever { get; set; }

        public string timesent { get; set; }

        public Message()
        {
            id = Guid.NewGuid();
        }
    }
}
