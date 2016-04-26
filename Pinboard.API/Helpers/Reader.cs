using System.Collections.Generic;
using System.Linq;
using CsQuery;
using Pinboard.Types;

namespace Pinboard.Helpers
{
    public class Reader
    {
        public List<Post> Bookmarks = new List<Post>();

        public Reader(string htmlFile)
        {
            var cq = CQ.CreateFromFile(htmlFile);
            var bookmarks = cq["DT"];
            foreach (var bookmark in bookmarks)
            {
                var href = bookmark.FirstChild["href"];
                var tags = bookmark.FirstChild["tags"];
                var title = bookmark.FirstChild.InnerText;
                var comment = GetCommentFromBookmark(bookmark);

                Bookmarks.Add(new Post {Href = href, Description = title, Extended = comment, Tag = tags});
            }
        }

        private string GetCommentFromBookmark(IDomObject element)
        {
            var sibling = element.NextSibling;
            var text = sibling.InnerText.Trim();
            if (text.Length < 1)
            {
                return string.Empty;
            }

            return text;
        }

        public List<Tag> GetTags()
        {
            return Bookmarks.Select(x => new Tag(x.Tag)).ToList();
        }
    }
}