﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaCatalog2.Models.Dto
{
    public class ModelCar
    {
         public  string model_id { get; set; }  //  
         public string model { get; set; }
         public string seo_url { get; set; }

        public override string ToString()
        {
            return model;
        }
    }
}

