using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using FullTextSearch.Models;

namespace FullTextSearch.Controllers
{
    public class PaperController : Controller
    {
        private readonly string _connectionString;

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }

        public PaperController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public IActionResult Index()
        {
            var model = Connection.Query<Paper>("SELECT * from paper");
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var model = Connection.Query<Paper>("SELECT * FROM paper WHERE id = @Id", new { Id = id }).FirstOrDefault();
            if (model == null) return NotFound();
            else return View(model);
        }

        public IActionResult Add()
        {
            return View("Editor");
        }

        [HttpPost]
        public IActionResult Add(Paper paper)
        {
            Connection.Query("INSERT INTO paper(id,eventtype,title,abstract,papertext) VALUES(@Id, @EventType,@Title,@Abstract,@PaperText);", paper);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var model = Connection.Query<Paper>("SELECT * FROM paper WHERE id = @Id", new { Id = id })
                .FirstOrDefault();
            if (model == null) return NotFound();
            else return View("Editor", model);
        }

        [HttpPost]
        public IActionResult Edit(Paper paper)
        {
            var model = Connection.Query<Paper>("UPDATE paper SET eventtype=@EventType,title=@Title,abstract=@Abstract,papertext=@Papertext WHERE id=@Id", paper)
                .FirstOrDefault();
            TempData["Message"] = "Chagnes successfully saved";
            return View("Editor", model);
        }

        public IActionResult Delete(int id)
        {
            Connection.Query("DELETE FROM paper WHERE id=@Id", new { Id = id });
            return RedirectToAction("Index");
        }
    }
}
