using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullTextSearch.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace FullTextSearch.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly string _connectionString;

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }

        public AnalysisController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }
        public IActionResult Index(Analysis model)
        {
            return View(model);
        }
    }
}
