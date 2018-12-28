using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel.Relationships
{
    public class HasHashtag
    {

        public Post post { get; set; }
        public Hashtag hashtag { get; set; }
        
        public string time { get; set; }
    }
}
