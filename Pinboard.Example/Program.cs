using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pinboard.Example
{
    internal class Program
    {
        private static Api _client;

        private static void Main(string[] args)
        {
            const string secret = "";
            const string username = "";
            _client = new Api($"{username}:{secret}");

            Console.WriteLine();
            PrintTags();
            Console.ReadLine();
        }

        private static async void PrintTags()
        {
            var tags = await _client.GetTags();

            foreach (var  item in tags.Data)
            {
                Console.WriteLine($"{item.Name}:{item.Count}");
            }
        }
    }
}