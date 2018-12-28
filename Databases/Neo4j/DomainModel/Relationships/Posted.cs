using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel
{
    public class Posted
    {

        public User user { get; set; }
        public Post post { get; set; }

        public string time { get; set; }

    }
}
