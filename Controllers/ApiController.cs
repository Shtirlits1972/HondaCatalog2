using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HondaCatalog2.Models.Dto;
using HondaCatalog2.Models;
using System.IO;
using System.Configuration;
using Microsoft.VisualBasic;
using System.Text;

namespace HondaCatalog2.Controllers
{
    public class ApiController : Controller
    {
        [Route("/vehicle/vin")]
        public IActionResult GetListCarTypeInfo(string vin)
        {
            List<VehiclePropArr> list = ClassCrud.GetListCarTypeInfo(vin);  //  JHMED73600S205949
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
        public IActionResult GetSpareParts(string vehicle_id, string group_id, string lang)
        {
            // List<SpareParts> list = ClassCrud.GetSpareParts(nplblk, hmodtyp);   //  "B__0100", 1667
            DetailsInNode detailsInNode = ClassCrud.GetDetailsInNode(vehicle_id, group_id, lang);
            return Json(detailsInNode);
        }
        [Route("/vehicle/sgroups")]
        public IActionResult GetSgroups(string vehicle_id, string group_id, string code_lang = "EN")
        {
            List<Sgroups> list = ClassCrud.GetSgroups(vehicle_id, group_id, code_lang);  
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
        //  Получение информации по авто
        [Route("/vehicleAttr")]
        public IActionResult GetVehiclePropArr(string vehicle_id, int t=0)
        {
            try
            {
                VehiclePropArr result = ClassCrud.GetVehiclePropArr(vehicle_id);

                if (t == 0)
                {
                    return Json(result);
                }
                else 
                {
                    return View("~/Views/Home/VehicleAttr.cshtml", result);
                }
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Route("/locales")]
        public IActionResult GetLang()
        {
            List<lang> list = ClassCrud.GetLang();
            return Json(list);
        }

        [Route("/vehicle/wmi")]
        public IActionResult GetWmi()
        {
            List<string> list = ClassCrud.GetWmi();
            return Json(list);
        }

        [HttpPost]
        [Route("/vehicle/sgroups")]
        public IActionResult GetNodes(string[] codes, string[] node_ids)
        {
            List<node> list = ClassCrud.GetNodes(codes, node_ids);
            return Json(list);
        }
    }
}