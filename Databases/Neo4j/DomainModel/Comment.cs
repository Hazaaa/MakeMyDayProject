using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel
{
    public class Comment
    {

        public Guid id { get; set; }
        public string text { get; set; }

        public User creator { get; set; }
        public string time { get; set; }

        public Comment()
        {
            id = Guid.NewGuid();
        }
    }
}
