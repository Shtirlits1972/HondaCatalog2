using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HondaCatalog2.Models;
using HondaCatalog2.Models.Dto;
using System.IO;

namespace HondaCatalog2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //List<Filters> list = ClassCrud.GetFilters("CIVIC", "4", "1982", "JH");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
