using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel.Relationships
{
    public class HasProfilePicture
    {

        public Picture profilePicture { get; set; }
        public User user { get; set; }

        public string time { get; set; }

    }
}
