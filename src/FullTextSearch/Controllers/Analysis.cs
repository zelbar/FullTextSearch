using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearch.Controllers
{
    public class Analysis : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
