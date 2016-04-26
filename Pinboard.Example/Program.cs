using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pinboard.Types;

namespace Pinboard.Example
{
    internal class Program
    {
        private static Api _client;

        private static void Main(string[] args)
        {
            const string secret = "C89A20D444673110DAB2";
            const string username = "socratees";
            _client = new Api($"{username}:{secret}");

            var tags = PrintTags("health", 2);
            PrintLinks(tags);
        }

        private static List<Tag> PrintTags(string tagName, int count = 2)
        {
            var tags = _client.GetTags("xml").Result;

            var sortedTagList = tags.Data.FindAll(x => x.tag.Contains(tagName)).OrderByDescending(x => x.count).Take(count);

            foreach (var item in sortedTagList)
            {
                Console.WriteLine($"{item.tag}:({item.count})");
            }

            return sortedTagList as List<Tag>;
        }

        private static List<Post> PrintLinks(List<Tag> tags)
        {
            var links = _client.GetPostsAsync(tags).Result;

            foreach (var item in links.Data)
            {
                Console.WriteLine(item.Href);
            }
            return links.Data;
        }
    }
}