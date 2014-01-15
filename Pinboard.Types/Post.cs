using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace Pinboard.Types
{
    public class Post
    {
        public string Href { get; set; }
        public string Description { get; set; }
        public string Extended { get; set; } //optional
        public string Tag { get; set; } //optional todo: check this for both pinboard and delicious
        public DateTime? CreationTime { get; set; } //dt, optional
        public string Replace { get; set; } //optional
        public string Shared { get; set; } //optional
        public string ToRead { get; set; }  //optional
    }
}
