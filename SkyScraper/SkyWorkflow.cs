using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using SkyScraper.Models;

namespace SkyScraper
{
    class SkyWorkflow
    {
        private static List<SkyProduct> _products = new() { SkyProduct.Default() };
        private static Timer _timer;
        private static string _url;
        private static readonly int _timerDelay = 300000; //5 mins
        public static void Start()
        {
            if (!File.Exists($"userdata/link.txt"))
            {
                Console.WriteLine("URL not found!");

                return;
            }

            _url = File.ReadAllText($"userdata/link.txt");
            Console.WriteLine("SkyScraper v0.1");
            Console.WriteLine($"Application started at {DateTime.Now:HH:mm:ss.fff}");
            Console.WriteLine("\nPress the Enter key to exit the application...\n");

            _products = ParserHelper.FetchStockFromFile(_url);

            Console.WriteLine("Since you were gone...");
            var newProducts = ParserHelper.GetItemsFromHTML(_url);
            var changes = Printer.PrintChangedItems(newProducts, _products);
            if (changes) _products = newProducts;

            SetTimer();

            Console.ReadLine();

            _timer.Stop();
            _timer.Dispose();

            Console.ResetColor();
            Console.WriteLine("Terminating the application...");
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
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Checking for changes.");
            var stock = ParserHelper.GetItemsFromHTML(_url);
            Printer.PrintChangedItems(stock, _products);
            _products = stock;
        }
    }
}
