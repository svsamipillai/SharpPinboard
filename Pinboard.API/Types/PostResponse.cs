using System;

namespace Pinboard.Types
{
    public class PostResponse
    {
        public String Href { get; set; }
        public String Description { get; set; }
        public String Extended { get; set; }
        public String Hash { get; set; }
        public String Meta { get; set; }
        public String Shared { get; set; }
        public string ToRead { get; set; }
        public int Others { get; set; }
        public String Tag { get; set; }
        public DateTime Time { get; set; }
    }
}