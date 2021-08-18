using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyScraper.Models
{
    public class SkyProduct
    {
        public bool InStock { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }

        public SkyProduct GetCopy()
        {
            return new SkyProduct
            {
                InStock = InStock,
                Model = Model,
                Name = Name,
                Price = Price
            };
        }

        public static SkyProduct Default()
        {
            return new SkyProduct
            {
                InStock = false,
                Model = "",
                Name = "",
                Price = ""
            };
        }
    }
}
