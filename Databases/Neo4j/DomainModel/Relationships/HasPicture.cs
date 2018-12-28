﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.Neo4j.DomainModel.Relationships
{
    public class HasPicture
    {

        public Post post { get; set; }
        public Picture picture { get; set; }

        public string time { get; set; }
    }
}
