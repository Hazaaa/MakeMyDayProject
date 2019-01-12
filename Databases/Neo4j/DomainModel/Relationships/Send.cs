using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel.Relationships
{
    public class Send
    {

        public User sender { get; set; }
        public Message message { get; set; }

        public string time { get; set; }

    }
}
