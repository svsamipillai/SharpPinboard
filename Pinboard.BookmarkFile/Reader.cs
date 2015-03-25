using Pinboard.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinboard.BookmarkFile
{
    public class Reader
    {
        public List<Post> Bookmarks = new List<Post>();
        public Reader(string htmlFile)
        {
            /*
             <DT><A HREF="https://hsmr.cc/palinopsia/" ADD_DATE="1427064751" PRIVATE="0" TAGS="security">The Palinopsia Bug</A>
<DD>discussion:https://news.ycombinator.com/item?id=9245980
<DT><A HREF="http://wkhtmltopdf.org/" ADD_DATE="1426736279" PRIVATE="0" TAGS="html,pdf,converter">wkhtmltopdf</A>
<DT><A HREF="http://blog.flexvdi.es/2015/03/17/enabling-kvm-virtualization-on-the-raspberry-pi-2/" ADD_DATE="1426723180" PRIVATE="0" TAGS="raspberrypi,linux,virtualization">Enabling KVM virtualization for Raspberry Pi 2 | flexVDI</A>
             */
            var cq = CsQuery.CQ.CreateFromFile(htmlFile);
            var bookmarks = cq["DT"];
            foreach (var bookmark in bookmarks)
            {
                var href = bookmark.FirstChild["href"];
                var tags = bookmark.FirstChild["tags"];
                var title = bookmark.FirstChild.InnerText;
                var comment = GetCommentFromBookmark(bookmark);

                Bookmarks.Add(new Post() { Href = href, Description = title, Extended = comment, Tag = tags });
                
            }
        }

        private string GetCommentFromBookmark(CsQuery.IDomObject element)
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
