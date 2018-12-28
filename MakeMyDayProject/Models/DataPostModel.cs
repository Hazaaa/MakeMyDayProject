using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MakeMyDayProject.Models
{
    public class DataPostModel
    {

        public string Text { get; set; }
        public List<string> Hashtags { get; set; }
        public List<string> Tags { get; set; }

    }
}