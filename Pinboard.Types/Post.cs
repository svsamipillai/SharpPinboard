using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Pinboard.Types
{
    public class Post
    {
        public string URL { get; set; }
        public string Description { get; set; }
        public string Extended { get; set; } //optional
        public Tags Tags { get; set; } //optional
        public DateTime? CreationTime { get; set; } //dt, optional
        public bool? Replace { get; set; } //optional
        public bool? Shared { get; set; } //optional
        public bool? ToRead { get; set; }  //optional
    }
}
