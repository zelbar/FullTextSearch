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

        [ResponseCache(NoStore = true)]
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

        public IActionResult CommonTerms(int n = 10)
        {
            var rnd = new Random();
            var list = Connection.Query<string>(
                @"SELECT * FROM ts_stat('SELECT tsv FROM paper')
WHERE word ~ '^[^0-9]+$'
ORDER BY nentry DESC, ndoc DESC, word
LIMIT @Number OFFSET @Rand;"
            .Replace("@Number", n.ToString())
            .Replace("@Rand", rnd.Next(0, 500).ToString()));
            return new JsonResult(list);
        }
    }
}
