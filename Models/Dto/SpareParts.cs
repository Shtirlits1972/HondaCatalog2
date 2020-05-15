using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaCatalog2.Models.Dto
{
    public class SpareParts
    {
        public int hpartplblk { get; set; }
        public string xpartext { get; set; }
        public string xpartrmrk { get; set; }
        public string nplblk { get; set; }
        public string npl { get; set; }

        public List<hotspots> hotspots { get; set; }
        public override string ToString()
        {
            return xpartext;
        }
    }
}
