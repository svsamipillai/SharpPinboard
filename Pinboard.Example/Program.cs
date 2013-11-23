using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Pinboard.Types;

namespace Pinboard.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Pinboard.API client = new API(ConfigurationManager.AppSettings["token"]);
            //Console.WriteLine(client.LastUpdated());
            //Console.WriteLine(client.AddPost(new Post() { URL = "http://example.com/1", Description = "Example, API Testing" }).Code);
            //Console.WriteLine(client.DeletePost("http://example.com/1").Code);
            //Console.WriteLine(.Count);
            var posts = client.GetRecentPosts();
            Console.WriteLine(posts.ToString());

        }
    }
}
