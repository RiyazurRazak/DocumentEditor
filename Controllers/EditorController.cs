using Microsoft.AspNetCore.Mvc;
using DocumentEditor.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DocumentEditor.Controllers
{
    public class EditorController : Controller
    {

        IConfiguration _configuration;
        SqlConnection _connection;


        public EditorController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("db"));
        }


        public IActionResult Index(string id)
        {
            Console.WriteLine(id);
            _connection.Open();
            Editor doc = new();
            string query = $"select * from documents where authorid='{id}'";
            SqlCommand cmd = new SqlCommand(query, _connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                doc.Id = reader.GetString(0);
                doc.author = reader.GetString(1);
                doc.content = reader.GetString(2);
                
            }
            reader.Close();
            _connection.Close();
            return View(doc);
        }

        public IActionResult View(string id)
        {
            List<Editor> list = new List<Editor>();
            _connection.Open();
            string query = $"select * from documents where authorid='{id}'";
            SqlCommand cmd = new SqlCommand(query, _connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Editor doc = new();
                doc.Id = reader.GetString(0);
                doc.author = reader.GetString(1);
                doc.content = reader.GetString(2);
                list.Add(doc);
            }
            reader.Close();
            _connection.Close();
            return View(list);
        }


        public IActionResult Edit(string id)
        {
            return RedirectToAction("Index", new {id = id});
        }

        [HttpPost]
        public IActionResult Save()
        {
            string data = Request.Form["content"];
            string id = Request.Form["author"];
            string docId = Request.Form["id"];
            _connection.Open();
            string query = $"Insert into documents values('{docId}', '{id}', '{data}')";
            SqlCommand cmd = new SqlCommand(query, _connection);
            cmd.ExecuteReader().Close();
            _connection.Close();
            return RedirectToAction("Index",new {id = id});
        }
    }
}
