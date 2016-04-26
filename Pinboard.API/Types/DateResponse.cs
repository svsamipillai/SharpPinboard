using System;
using System.Collections.Generic;

namespace Pinboard.Types
{
    public class DateResponse
    {
        private string _tag = string.Empty;
        public DateResponse(string tag)
        {
            _tag = tag;
        }

        private string Tag => _tag;

        public List<Tag> Tags => new List<Tag> { new Tag(_tag) };

        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}