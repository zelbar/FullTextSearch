using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearch.Controllers
{

    public class HomeController : Controller
    {
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
