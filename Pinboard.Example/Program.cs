using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security;
using System.Text;
using System.Configuration;
using Pinboard.Types;
using ServiceStack.Text;

namespace Pinboard.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            API client;
            string token = ConfigurationManager.AppSettings["token"];
            
            if (string.IsNullOrEmpty(token) || token == "YOUR_API_KEY_HERE")
            {
                UpdateSettings();
            }
            token = ConfigurationManager.AppSettings["token"];
            string url = ConfigurationManager.AppSettings["APIBaseURL"];
            string username = ConfigurationManager.AppSettings["Username"];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(url))
            { 
                client = new API(ConfigurationManager.AppSettings["token"]);
            }
            else
            {
                client = new API(token,url,username);
            }

      
            var posts = client.GetAllPosts(new Tags("rss-feed"));
            var urls = posts.Select(post => post.Href).Aggregate((a, b) => a + Environment.NewLine + b);
            urls.PrintDump();
            Console.ReadLine();
        }

        private static void UpdateSettings(ConfigurationUserLevel configurationLevel = ConfigurationUserLevel.None)
        {
            bool needsUsername = false;
            string username = string.Empty;
            const string DeliciousAPI = "https://api.del.icio.us/v1/";
            Configuration config = ConfigurationManager.OpenExeConfiguration(configurationLevel);
            Console.WriteLine("Updating app.config at " + config.FilePath);
            Console.WriteLine("Use which site?");
            Console.WriteLine("1: Pinboard (default)");
            Console.WriteLine("2: Delicious");
            Console.WriteLine("Your selection: ");
            
            string selection = Console.ReadLine();
            switch (selection)
            {
                case "1": //don't need to update the config
                    break;
                case "2":
                    config.AppSettings.Settings.Add("APIBaseURL", DeliciousAPI);
                    needsUsername = true; //unlike Pinboard, Delicious uses a username:password pair.
                    break;
                default:
                    Console.WriteLine("Invalid selection, using default (Pinboard)");
                    break;
            }

            if (needsUsername)
            {
                Console.WriteLine("Please enter your username: ");
                username = Console.ReadLine();
                config.AppSettings.Settings.Add("Username", username);
            }

            string passwordMessage = "Please enter your " + (needsUsername ? "password" : "token") + ":";
            Console.WriteLine(passwordMessage);
            config.AppSettings.Settings.Add("token",GetPassword());
            
            config.Save(ConfigurationSaveMode.Full);
        }

        private static string GetPassword()
        {
            ConsoleKeyInfo key;
            StringBuilder password = new StringBuilder();
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password.Remove(password.Length - 1,1);
                        Console.Write("\b \b");
                    }
                }
            } 
            while (key.Key != ConsoleKey.Enter);
            return password.ToString();
        }
    }
}
