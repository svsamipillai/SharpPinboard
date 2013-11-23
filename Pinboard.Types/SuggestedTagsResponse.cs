using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinboard.Types
{
    public class SuggestedTagsResponse
    {
        public Tags Popular { get; set; }
        public Tags Recommended { get; set; }
    }
}
