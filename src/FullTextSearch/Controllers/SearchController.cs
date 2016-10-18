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
        public async Task<IActionResult> Index(Search search)
        {
            if (!string.IsNullOrEmpty(search.Query))
            {
                using (var connection = Connection)
                {
                    var sqb = new SearchQueryBuilder(search.Query, search.Operator);
                    connection.Open();
                    search.SqlQuery = sqb.SqlQuery(search.Type, search.NumberOfResults, search.Page);
                    var sw = new Stopwatch();
                    sw.Start();
                    search.Results = connection.Query<Paper>(search.SqlQuery);
                    sw.Stop();
                    search.QueryTime = sw.ElapsedMilliseconds;
                    var logger = new SearchQueryLogger();
                    try
                    {
                        await logger.Log(connection, search);
                    }
                    catch(Exception e)
                    {

                    }
                    return View(search);
                }
            }
            else
            {
                return View(new Search());
            }
        }
    }
}
