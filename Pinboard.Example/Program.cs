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
            var secret = ConfigurationManager.AppSettings["Secret"];
            var username = ConfigurationManager.AppSettings["Username"];
            _client = new Api($"{username}:{secret}");

            Console.WriteLine();
            PrintTags();
            Console.ReadLine();
        }

        private static async void PrintTags()
        {
            var tags = await _client.GetTags();

            if (tags?.Data == null)
            {
                Console.WriteLine("Error!");
                return;
            }

            foreach (var  item in tags.Data)
            {
                Console.WriteLine($"{item.Name}:{item.Count}");
            }
        }

        private static void GetPinboardMethods()
        {
            var api = Assembly.GetAssembly(typeof(Api));
            var mainType = api.GetType("Pinboard.Api");
            var methods = mainType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            for (int i = 0; i < methods.Length; i++)
            {
                Console.WriteLine(i + ":" + methods[i].Name + " " + PrintParameters(methods[i].GetParameters()));
            }

            try
            {
                Console.WriteLine("Choose a method to run (or a number not listed here to exit): ");
                string selection = Console.ReadLine();
                int methodNumber = int.Parse(selection.Trim());
                if (methodNumber > methods.Length + 1 || methodNumber < 0)
                {
                    Environment.Exit(1);
                }
                List<object> parameterArray = new List<object>();
                foreach (ParameterInfo methodParam in methods[methodNumber].GetParameters())
                {
                    Console.Write("Set the value of {0} (optional: {1}): ", methodParam.Name, methodParam.IsOptional);
                    var paramValue = Console.ReadLine().Trim();

                    parameterArray.Add(paramValue);
                }

                try
                {
                    var response = methods[methodNumber].Invoke(_client, parameterArray.ToArray());
                    if (((Task)response).IsFaulted)
                    {
                        Console.WriteLine("Fault!");
                    }

                    response.PrintDump();
                }
                catch (TargetInvocationException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        private static string PrintParameters(ParameterInfo[] parameterInfo)
        {
            StringBuilder builder = new StringBuilder("(");
            foreach (var info in parameterInfo)
            {
                builder.Append(info.Name + ",");
            }
            builder.Append(")");
            return builder.ToString();
        }

        private static void UpdateSettings(ConfigurationUserLevel configurationLevel = ConfigurationUserLevel.None)
        {
            bool needsUsername = false;
            const string deliciousApi = "https://api.del.icio.us/v1/";
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
                    config.AppSettings.Settings.Add("APIBaseURL", deliciousApi);
                    needsUsername = true; //unlike Pinboard, Delicious uses a username:password pair.
                    break;

                default:
                    Console.WriteLine("Invalid selection, using default (Pinboard)");
                    break;
            }

            if (needsUsername)
            {
                Console.WriteLine("Please enter your username: ");
                var username = Console.ReadLine();
                config.AppSettings.Settings.Add("Username", username);
            }

            string passwordMessage = "Please enter your " + (needsUsername ? "password" : "token") + ":";
            Console.WriteLine(passwordMessage);
            config.AppSettings.Settings.Add("token", GetPassword());

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
                        password.Remove(password.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return password.ToString();
        }
    }
}