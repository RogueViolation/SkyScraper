using System;
using System.Collections.Generic;
using System.Linq;
using SkyScraper.Models;
using SkyScraper.Interface;

namespace SkyScraper
{
    public class Printer : IPrinter
    {
        public bool PrintChangedItems(List<SkyProduct> productList, List<SkyProduct> productListOld)
        {
            bool anyChanges = false;
            foreach (var item in productList)
            {
                if (!productListOld.Any(i => i.Model == item.Model))
                {
                    anyChanges = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: New item {item.Model} detected with price of {item.Price}.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    var oldItem = productListOld.FirstOrDefault(i => i.Model == item.Model);
                    if (oldItem.InStock == false && oldItem.InStock != item.InStock)
                    {
                        Console.Beep();
                        anyChanges = true;
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Item {item.Model} in now in stock!");
                    }
                    if (oldItem.Price != item.Price)
                    {
                        Console.Beep();
                        anyChanges = true;
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Item's {item.Model} price has changed from {oldItem.Price} to {item.Price}.");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var oldItem in productListOld)
            {
                if (!productList.Any(i => i.Model == oldItem.Model))
                {
                    anyChanges = true;
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: New item {oldItem.Model} detected with price of {oldItem.Price}.");
                }
            }

            if (!anyChanges) Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: No new changes...");

            Console.ResetColor();
            return anyChanges;
        }
    }
}
