using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DocumentEditor.Controllers
{
    public class AuthController : Controller
    {

        IConfiguration _configuration;
        SqlConnection _connection;


        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("db"));
        }
        // GET: AuthController
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(IFormCollection collection)
        {
            try
            {
                _connection.Open();
                string username = Request.Form["username"];
                string password = Request.Form["password"];
                string id = Guid.NewGuid().ToString();
                string query = $"Insert into users values('{id}', '{username}', '{password}')";
                SqlCommand cmd = new SqlCommand(query, _connection);
                cmd.ExecuteReader().Close();
                _connection.Close();
                return RedirectToAction("Index", "Editor", new { id = id });
            }
            catch (SqlException err)
            {
                return Problem(err.Message);
            }
        }

        [HttpPost]
        public ActionResult Login(IFormCollection collection)
        {
            try
            {
                string username = Request.Form["username"];
                string password = Request.Form["password"];
                string userId = "";
                _connection.Open();
                string query = $"select * from users where username='{username}' and password='{password}'";
                SqlCommand cmd = new SqlCommand(query, _connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userId = reader.GetString(0);
                }
                reader.Close();
                _connection.Close();
                return RedirectToAction("View", "Editor", new { id =  userId });
            }
            catch (SqlException err)
            {
                return Problem(err.Message);
            }
        }

    }
}
