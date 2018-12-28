using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel
{
    public class Commented
    {

        public User commentator { get; set; }
        public Comment comment { get; set; }
        public string time { get; set; }

    }
}
