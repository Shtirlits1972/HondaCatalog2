﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaCatalog2.Models.Dto
{
    public class PartsGroup
    {
        public string Id { get; set; }
        public string name { get; set; }
        public List<Sgroups> childs { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
}


