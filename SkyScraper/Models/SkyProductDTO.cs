using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyScraper.Models
{
    public class SkyProductDTO
    {
        public string Url { get; set; }
        public List<SkyProduct> Products { get; set; }
    }
}
