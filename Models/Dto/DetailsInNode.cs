using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaCatalog2.Models.Dto
{
    public class DetailsInNode
    {
        public string node_id { get; set; }
        public string name { get; set; }
        public List<Detail> parts { get; set; }
        public List<images> images { get; set; }
        public List<attributes> attributes { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
