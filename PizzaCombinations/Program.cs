using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Configuration;
using log4net.Config;
using log4net;

namespace PizzaCombinations
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static string _url = ConfigurationManager.AppSettings["dataUrl"];

        static void Main(string[] args)
        {
            try
            {
                // retrieve the data with a WebClient
                WebClient webClient = new WebClient();
                string request = webClient.DownloadString(_url);

                // store combinations in a dictionary
                Dictionary<string, int> combinations = new Dictionary<string, int>();

                if (string.IsNullOrEmpty(request))
                {
                    Console.WriteLine("There is no data.");
                    log.Error("There is no data.");
                    Console.ReadKey();
                    return;
                }

                List<Pizzas> items = JsonConvert.DeserializeObject<List<Pizzas>>(request);

                foreach (var item in items)
                {
                    // order combinations ascending
                    string combination = string.Join(",", item.Toppings.OrderBy(x => x));

                    // if exists, add one to the count, otherwise add new
                    if (combinations.ContainsKey(combination))
                    {
                        combinations[combination]++;
                    }
                    else
                    {
                        combinations.Add(combination, 1);
                    }
                }

                Console.WriteLine("Top 20 combinations displayed by most frequently ordered" + Environment.NewLine);

                Console.WriteLine(
                    string.Join(Environment.NewLine, combinations
                        .OrderByDescending(x => x.Value)
                        .Take(20)
                        .Select(x => "Combination: " + x.Key + " Ordered: " + x.Value.ToString() + " times"))
                );

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                log.Error(ex.Message, ex);
                Console.ReadKey();
            }
        }
    }

    public class Pizzas
    {
        public List<string> Toppings;
    }

}
