using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SkyScraper.Models;

namespace SkyScraper
{
    public static class ParserHelper
    {
        private readonly static HtmlWeb _web = new();
        static HtmlNode GetPageContentsToObject(string url)
        {

            return _web.Load(url).DocumentNode;
        }

        public static List<SkyProduct> GetItemsFromHTML(string url)
        {
            var productList = new List<SkyProduct>();
            var html = GetPageContentsToObject(url);
            var tableElement = html.SelectSingleNode("//*[@id=\"centerpanel\"]/div[2]/table[1]");
            var elems = tableElement.ChildNodes.Where(e => e.Name == "tr" && e.InnerHtml.Contains("MODELIS"));

            foreach (var nNode in elems)
            {

                productList.Add(GetProductInfo(
                    nNode.ChildNodes.Where(e => e.Name == "td"),
                    !nNode.Attributes["class"].Value.Contains("nostock")
                    ));
            }

            SaveStockToFile(url, productList);

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

        private static void SaveStockToFile(string url, List<SkyProduct> productList)
        {
            var obj = new { Url = url, Products = productList };
            using StreamWriter file = File.CreateText($"userdata/stock.json");
            JsonSerializer serializer = new();
            serializer.Serialize(file, obj);
        }

        public static List<SkyProduct> FetchStockFromFile(string url)
        {
            try
            {
                if (!File.Exists($"userdata/stock.json"))
                {
                    Console.WriteLine("Seems like it's the first time running the application, stock file doesn't exist");
                    return new List<SkyProduct> { SkyProduct.Default() };
                }
                using StreamReader file = File.OpenText($"userdata/stock.json");
                var deserializer = new JsonSerializer();
                var json = (SkyProductDTO)deserializer.Deserialize(file, typeof(SkyProductDTO));
                if (json.Url == url)
                {
                    Console.WriteLine("File loaded successfully!");
                    return json.Products;
                }
                else
                {
                    Console.WriteLine("URL from file doesn't match url you are checking. A new file will be made next time the URL is looked-up");
                    return new List<SkyProduct> { SkyProduct.Default() };
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error fetching file.");
                return new List<SkyProduct> { SkyProduct.Default() };
            }
        }
    }
}
