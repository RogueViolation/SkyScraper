using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using SkyScraper.Models;
using SkyScraper.Interface;

namespace SkyScraper
{
    public class SkyWorkflow : ISkyWorkflow
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly IPrinter _printer;
        private readonly ISkyParser _parser;
        private static List<SkyProduct> _products = new() { SkyProduct.Default() };
        private static Timer _timer;
        private string _url;
        private readonly int _timerDelay;
        private readonly string _version;

        public SkyWorkflow(IConfigurationReader configurationReader, IPrinter printer, ISkyParser parser)
        {
            _configurationReader = configurationReader;
            _printer = printer;
            _parser = parser;
            _timerDelay = int.Parse(_configurationReader.GetSection("timerDelay")); //5 mins
            _version = _configurationReader.GetSection("version");
            _timer = new Timer(_timerDelay);
        }
        public void Start()
        {
            if (!File.Exists($"userdata/link.txt"))
            {
                Console.WriteLine("URL not found!");

                return;
            }

            _url = File.ReadAllText($"userdata/link.txt");
            Console.WriteLine($"SkyScraper {_version}");
            Console.WriteLine($"Application started at {DateTime.Now:HH:mm:ss.fff}");
            Console.WriteLine("\nPress the Enter key to exit the application...\n");

            _products = _parser.FetchStockFromFile(_url);

            Console.WriteLine("Since you were gone...");
            var newProducts = _parser.GetItemsFromHTML(_url);
            var changes = _printer.PrintChangedItems(newProducts, _products);
            if (changes) _products = newProducts;

            SetTimer();

            Console.ReadLine();

            _timer.Stop();
            _timer.Dispose();

            Console.ResetColor();
            Console.WriteLine("Terminating the application...");
        }

        private void SetTimer()
        {
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Checking for changes.");
            var stock = _parser.GetItemsFromHTML(_url);
            _printer.PrintChangedItems(stock, _products);
            _products = stock;
        }
    }
}
