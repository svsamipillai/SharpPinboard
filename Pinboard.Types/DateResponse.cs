using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinboard.Types
{
    public class DateResponse
    {
        private string Tag { get; set; }

        public List<Tag> Tags
        {
            get
            {
                return new List<Tag>() { new Tag(Tag) };
            }
        }

        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
