using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinboard.Types
{
    public class SuggestedTagsResponse
    {
        public List<Tag> Popular { get; set; }
        public List<Tag> Recommended { get; set; }
    }
}
