using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Timers;

namespace SkyScraper
{
    class Program
    {
        private readonly static HtmlWeb _web = new HtmlWeb();
        private static List<SkyProduct> _products = new List<SkyProduct>
        {
            new SkyProduct
            {
                Price = "0",
                Name = "0",
                Model = "0",
                InStock = false
            }
        };
        private static Timer _timer;
        private static string _url;
        private static readonly int _timerDelay = 300000; //5 mins
        
        static void Main(string[] args)
        {

            if (!File.Exists($"link.txt"))
            {
                Console.WriteLine("URL not found!");

                return;
            }

            _url = File.ReadAllText(@"link.txt");
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            Console.WriteLine("\nPress the Enter key to exit the application...\n");

            GetItemsFromHTML(_url);
            SetTimer();

            Console.ReadLine();

            _timer.Stop();
            _timer.Dispose();

            Console.ResetColor();
            Console.WriteLine("Terminating the application...");
        }
        
        static HtmlNode GetPageContents(string url)
        {
            return _web.Load(url).DocumentNode;
        }

        static List<SkyProduct> GetItemsFromHTML(string url)
        {
            var productList = new List<SkyProduct>();
            var html = GetPageContents(url);
            var tableElement = html.SelectSingleNode("//*[@id=\"centerpanel\"]/div[2]/table[1]");
            var elems = tableElement.ChildNodes.Where(e => e.Name == "tr" && e.InnerHtml.Contains("MODELIS"));

            foreach (var nNode in elems)
            {

                productList.Add(GetProductInfo(
                    nNode.ChildNodes.Where(e => e.Name == "td"),
                    !nNode.Attributes["class"].Value.Contains("nostock")
                    ));
            }
            _products = productList;
            return productList;
        }

        static SkyProduct GetProductInfo(IEnumerable<HtmlNode> htmlNodes, bool inStock)
        {
            var priceString = htmlNodes.ElementAtOrDefault(4).InnerText.Trim().Replace(" ", string.Empty).Replace("€", "EUR");

            var itemInfo = htmlNodes.ElementAtOrDefault(2)
                .ChildNodes[0]
                .InnerHtml
                .Replace("\r\n", string.Empty)
                .Trim()
                .Split("<br>");

            return new SkyProduct
            {
                InStock = inStock,
                Model = itemInfo[0].Remove(0, 9),
                Name = itemInfo[1],
                Price = priceString
            };
        }

        static void PrintChangedItems(List<SkyProduct> productList)
        {
            foreach (var item in productList)
            {
                if (!_products.Any(i => i.Model == item.Model))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: New item {item.Model} detected with price of {item.Price}. \n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    var oldItem = _products.FirstOrDefault(i => i.Model == item.Model);
                    if (oldItem.InStock == false && oldItem.InStock != item.InStock)
                    {
                        Console.Beep();
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Item {item.Model} in now in stock! \n");
                    }
                    if (oldItem.Price != oldItem.Price)
                    {
                        Console.Beep();
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Item's {item.Model} price has changed from {oldItem.Price} to {item.Price}. \n");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var oldItem in _products)
            {
                if (!productList.Any(i => i.Model == oldItem.Model))
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: New item {oldItem.Model} detected with price of {oldItem.Price}. \n");
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        private static void SetTimer()
        {
            _timer = new Timer(_timerDelay);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            PrintChangedItems(GetItemsFromHTML(_url));
        }
    }

    public class SkyProduct
    {
        public bool InStock { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }
}
