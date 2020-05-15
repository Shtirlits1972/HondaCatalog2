using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HondaCatalog2.Models.Dto;
using HondaCatalog2.Models;
using System.IO;
using System.Configuration;

namespace HondaCatalog2.Controllers
{
    public class ApiController : Controller
    {
        [Route("/vehicle/vin")]
        public IActionResult GetListCarTypeInfo(string vin)
        {
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfo(vin);  //  JHMED73600S205949
            List<header> headerList = ClassCrud.GetHeaders();

            var result = new
            {
                headers = headerList,
                items = list,
                cntitems = list.Count,
                page = 1
            };

            return Json(result);
        }
        //   /image/{image_id} 
        [Route("/image")]
        public IActionResult GetImage(string image_id)
        {

            string FilderImagePath = Ut.GetImagePath();  //"wwwroot/image/";
            //image_id = "Yamato.jpg";
            string fullPath = FilderImagePath + image_id;

            if(System.IO.File.Exists(fullPath))
            {
                byte[] file = System.IO.File.ReadAllBytes(fullPath);
                return Ok(file);
            }

            return NotFound("Картинка не найдена.");
        }

        [Route("/models")]
        public IActionResult GetModels()
        {
            List<ModelCar> list = ClassCrud.GetModelCars();
            return Json(list);
        }


        [Route("/mgroups")]
        public IActionResult GetPartsGroups(string vehicle_id)
        {
            List<PartsGroup> list = ClassCrud.GetPartsGroup(vehicle_id);
            return Json(list);
        }

        [Route("/vehicle")]
        public IActionResult GetSpareParts(string nplblk, int hmodtyp)
        {
            List<SpareParts> list = ClassCrud.GetSpareParts(nplblk, hmodtyp);   //  "B__0100", 1667
            return Json(list);
        }

        [Route("/vehicle/sgroups")]
        public IActionResult GetSgroups(int hmodtyp, string nplgrp, string npl="" )
        {
            List<Sgroups> list = ClassCrud.GetSgroups(hmodtyp, nplgrp, npl);  //  8030, "2", "19SELKD1"
            return Json(list);
        }

        [Route("/ﬁlters")]
        public IActionResult GetFilters(string modelId)
        {
            //  List<Filters> list = ClassCrud.GetFilters("CIVIC", "4", "1982", "JH");
            List<Filters> list = ClassCrud.GetFilters(modelId);   //  "CIVIC", "4", "1982", "JH"
            return Json(list);
        }

        [Route("/ﬁlter-cars")]
        public IActionResult GetListCarTypeInfoFilterCars(string modelId, string [] param, int page=1, int page_size=10)
        {
            if(param.Length == 3)
            {
                List<header> headerList = ClassCrud.GetHeaders();

                string ctrsmtyp = param[0];
                string carea = param[1];
                string nengnpf = param[2];

                List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfoFilterCars(modelId, ctrsmtyp, carea, nengnpf); // 

                list = list.Skip((page - 1) * page_size).Take(page_size).ToList();

                var result = new
                {
                    headers = headerList,
                    items = list,
                    cntitems = list.Count,
                    page = page
                };

                return Json(result);
            }

            return NotFound("Проверьте параметры запроса!");
        }
    }
}