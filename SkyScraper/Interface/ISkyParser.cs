using SkyScraper.Models;
using System.Collections.Generic;

namespace SkyScraper.Interface
{
    public interface ISkyParser
    {
        public List<SkyProduct> GetItemsFromHTML(string url);
        public List<SkyProduct> FetchStockFromFile(string url);
    }
}
