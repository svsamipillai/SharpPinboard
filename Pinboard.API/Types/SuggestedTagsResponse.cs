using System.Collections.Generic;

namespace Pinboard.Types
{
    public class SuggestedTagsResponse
    {
        public List<Tag> Popular { get; set; }
        public List<Tag> Recommended { get; set; }
    }
}