using System;
using System.Collections.Generic;

namespace Pinboard.Types
{
    public class DateResponse
    {
        public DateResponse(string tag)
        {
            Tag = tag;
        }

        private string Tag { get; }

        public List<Tag> Tags => new List<Tag> { new Tag(Tag) };

        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}