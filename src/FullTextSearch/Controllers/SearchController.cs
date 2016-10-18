using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Npgsql;
using Dapper;
using System.Text;
using FullTextSearch.Tools;
using FullTextSearch.Models;
using System.Diagnostics;

namespace FullTextSearch.Controllers
{
    public class SearchController : Controller
    {
        private readonly string _connectionString;

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }

        public SearchController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public IActionResult Index(Search search)
        {
            if (!string.IsNullOrEmpty(search.Query))
            {
                using (var connection = Connection)
                {
                    var sqb = new SearchQueryBuilder(search.Query, search.Operator);
                    connection.Open();
                    var model = new Search();
                    model.SqlQuery = sqb.SqlQuery(search.Type, search.NumberOfResults, search.Page);
                    var sw = new Stopwatch();
                    sw.Start();
                    model.Results = connection.Query<Paper>(model.SqlQuery);
                    sw.Stop();
                    model.QueryTime = sw.ElapsedMilliseconds;
                    return View(model);
                }
            }
            else
            {
                return View(new Search());
            }
        }
    }
}
