using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinboard.Types
{
    public class DateResponse
    {
        private string Tag { get; set; }
        public Tags Tags { get { return new Tags(Tag); } }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
