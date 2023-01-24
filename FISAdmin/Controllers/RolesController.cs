using FISAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace FISAdmin.Controllers
{
    public class RolesController : Controller
    {

        private readonly IConfiguration config;
        public List<RolesModel> result = new List<RolesModel>();
        public string type = "Access Roles";

        public RolesController(IConfiguration configuration)
        {
            this.config = configuration;
        }

        //GET: Index
        public IActionResult Index()
        {
            
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Id,Name FROM AspNetRoles";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RolesModel role = new RolesModel();

                        role.Id = dr.GetString(0);
                        role.Name = dr.GetString(1);
                       
                        result.Add(role);

                    }
                }
            }

            ViewData["role"] = result;
            ViewData["type"] = type;

            return View();
        }

        //GET: Create
        public IActionResult Create()
        {
            ViewData["type"] = type;
            return View();
        }

        //POST: Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RolesModel obj)
        {
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Name FROM AspNetRoles WHERE Name=@Name";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = Request.Form["Name"].ToString();

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    ModelState.AddModelError("Name", "Role already exist");
                    TempData["error"] = "Access Role cannot be created";
                }
                else
                {
                    con.Close();

                    using (SqlConnection con2 = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                    {

                        string sql4 = "DECLARE @ID UNIQUEIDENTIFIER\r\nSET @ID = NEWID()\r\n\r\nDECLARE @STAMP UNIQUEIDENTIFIER\r\nSET @STAMP = NEWID()\r\n\r\nINSERT INTO AspNetRoles (Id,Name,NormalizedName,ConcurrencyStamp) VALUES (@ID,@Name,@NormalizedName,@STAMP)";

                        SqlCommand cmd4 = new SqlCommand(sql4, con2);

                        cmd4.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = Request.Form["Name"].ToString();
                        cmd4.Parameters.Add("@NormalizedName", System.Data.SqlDbType.NVarChar).Value = Request.Form["Name"].ToString().ToUpper();
                       

                        con2.Open();
                        cmd4.ExecuteNonQuery();
                        TempData["success"] = "Access Role created successfully";
                        ViewData["type"] = "Access Users";
                        return RedirectToAction(nameof(Index));
                    }

                }

            }

            ViewData["type"] = type;

            return View(obj);
        }

        //GET: Edit
        public IActionResult Edit(string id)
        {

            RolesModel role = new RolesModel();

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Id,Name FROM AspNetRoles WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = id;

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        role.Id = dr.GetString(0);
                        role.Name = dr.GetString(1);
                 
                    }
                }
            }

            ViewData["role"] = role;
            ViewData["type"] = type;
       
            return View();
            
        }

        //POST: Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit()
        {
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Name FROM AspNetRoles WHERE Name=@Name";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = Request.Form["Name"].ToString();

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    ModelState.AddModelError("Name", "Role already exist");
                    TempData["error"] = "Access Role cannot be created";
                }
                else
                {
                    con.Close();

                    using (SqlConnection con2 = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                    {
                        string sql2 = "UPDATE AspNetRoles SET Name=@Name,NormalizedName=@NormalizedName WHERE Id=@Id";

                        con2.Open();

                        SqlCommand cmd2 = new SqlCommand(sql2, con2);

                        cmd2.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = Request.Form["Name"].ToString();
                        cmd2.Parameters.Add("@NormalizedName", System.Data.SqlDbType.NVarChar).Value = Request.Form["Name"].ToString().ToUpper();
                        cmd2.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = Request.Form["Id"].ToString();

                        cmd2.ExecuteNonQuery();
                        TempData["success"] = "Access Role updated successfully";
                        return RedirectToAction(nameof(Index));

                    }
                }
            }

            /*return View();*/
            ViewData["type"] = type;

            return View();
        }

        //GET: Delete
        public IActionResult Delete(string id)
        {
            RolesModel role = new RolesModel();

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Id,Name FROM AspNetRoles WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = id;

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        role.Id = dr.GetString(0);
                        role.Name = dr.GetString(1);

                    }
                }
            }

            ViewData["role"] = role;
            ViewData["type"] = type;

            return View();
        }
        //POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete()
        {
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "DELETE FROM AspNetRoles WHERE Id=@Id";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = Request.Form["Id"].ToString();

                con.Open();
                cmd.ExecuteNonQuery();

            }
            TempData["success"] = "Access Role deleted successfully";
            ViewData["type"] = type;

            return RedirectToAction(nameof(Index));

        }
    }
}
