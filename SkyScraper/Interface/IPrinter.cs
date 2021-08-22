using SkyScraper.Models;
using System.Collections.Generic;

namespace SkyScraper.Interface
{
    public interface IPrinter
    {
        public bool PrintChangedItems(List<SkyProduct> productList, List<SkyProduct> productListOld);
    }
}
