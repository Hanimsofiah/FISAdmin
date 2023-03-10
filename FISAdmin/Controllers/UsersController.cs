using FISAdmin.Models;
using FISAdmin.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using Microsoft.AspNetCore.Identity;

namespace AdminFIS.Controllers
{
    public class UsersController : Controller
    {
        private readonly IConfiguration config;
        private readonly UserManager<ApplicationUser> _userManager;
        List<UsersModel> result = new List<UsersModel>();
        public string type = "Access Users";

        public UsersController(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            this.config = configuration;
            _userManager = userManager;
        }

         //GET: Index
        public IActionResult Index()
        {
            int no = 0;
            string user = "user";
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT AspNetUsers.Id, AspNetUsers.Nopeng, AspNetUsers.Nama, AspNetUsers.ActiveStatus, AspNetUsers.CreatedBy, AspNetUsers.CreatedDate, AspNetUsers.LastModifiedBy, AspNetUsers.LastModifiedDate, " +
                    "AspNetUsers.Email, AspNetUserRoles.RoleId, AspNetRoles.Name " +
                    "FROM AspNetUsers FULL JOIN AspNetUserRoles ON AspNetUsers.Id=AspNetUserRoles.UserId JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        UsersModel obj = new UsersModel();
                        no++;
                        obj.user_no = user + no.ToString();
                        obj.Id = sdr["Id"].ToString();
                        obj.Nopeng = sdr["Nopeng"].ToString();
                        obj.Nama = sdr["Nama"].ToString();
                        obj.ActiveStatus = (int)sdr["ActiveStatus"];
                        obj.CreatedBy = sdr["CreatedBy"].ToString();
                        obj.CreatedDate = (DateTime) sdr["CreatedDate"];
                        obj.LastModifiedBy = sdr["LastModifiedBy"].ToString();
                        obj.LastModifiedDate = (DateTime) sdr["LastModifiedDate"];
                        obj.Email = sdr["Email"].ToString();
                        obj.RoleId = sdr["RoleId"].ToString();
                        obj.RoleName = sdr["Name"].ToString();
                        result.Add(obj);
                    }
                }
                con.Close();
            }
            ViewData["user"] = result;
            ViewData["type"] = type;
            return View();
        }

        //GET: Create
        //Users/Create is replaced by Identity/Pages/Account/Register.cshtml
        public IActionResult Create()
        {
            UsersModel user = new UsersModel();
            user.Roles = PopulateRoles();
            user.Users = PopulateUsers();
            ViewData["type"] = type;
            ViewData["user"] = PopulateUsers();
            ViewData["role"] = PopulateRoles();
            /*return View(user);*/
            return new RedirectResult(url: "/Identity/Account/Register");
        }

        private List<SelectListItem> PopulateRoles()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Id,Name FROM AspNetRoles";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = sdr["Name"].ToString(),
                            Value = sdr["Id"].ToString()
                        });
                    }
                }
                con.Close();
            }
            return items;
        }

        private List<SelectListItem> PopulateUsers()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT nama,nopeng,emailrasmi FROM v_profailstaf";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = sdr["nama"].ToString() + " (" + sdr["nopeng"].ToString() + ") - " + sdr["emailrasmi"].ToString() ,
                            Value = sdr["nopeng"].ToString()
                        });
                    }
                }
                con.Close();
            }
            return items;
        }

        //POST: Create
        //Users/Create is replaced by Identity/Pages/Account/Register.cshtml.cs
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UsersModel obj)
        {
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Nopeng FROM AspNetUsers WHERE Nopeng=@Nopeng";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Nopeng", System.Data.SqlDbType.NVarChar).Value = Request.Form["Nopeng"].ToString();
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    ModelState.AddModelError("Nopeng", "User already exist");
                    TempData["error"] = "Access User cannot be created";
                }
                else
                {
                    con.Close();
                    using (SqlConnection con2 = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                    {
                        string sql2 = "DECLARE @STAMP UNIQUEIDENTIFIER\r\nSET @STAMP = NEWID()\r\n\r\n"+
                            "INSERT INTO AspNetUsers (Nopeng,Nama,ActiveStatus,EmailConfirmed,ConcurrencyStamp,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount) VALUES (@Nopeng,@Nama,@ActiveStatus,@STAMP,@EmailConfirmed,@PhoneNumberConfirmed,@TwoFactorEnabled,@LockoutEnabled,@AccessFailedCount)";

                        SqlCommand cmd2 = new SqlCommand(sql2, con2);
                        cmd2.Parameters.Add("@Nopeng", System.Data.SqlDbType.NVarChar).Value = Request.Form["Nopeng"].ToString();
                        cmd2.Parameters.Add("@Nama", System.Data.SqlDbType.NVarChar).Value = Request.Form["Nama"].ToString();
                        cmd2.Parameters.Add("@ActiveStatus", System.Data.SqlDbType.Int).Value = Int32.Parse(Request.Form["ActiveStatus"].ToString());
                        cmd2.Parameters.Add("@EmailConfirmed", System.Data.SqlDbType.Bit).Value = 0;
                        cmd2.Parameters.Add("@PhoneNumberConfirmed", System.Data.SqlDbType.Bit).Value = 0;
                        cmd2.Parameters.Add("@TwoFactorEnabled", System.Data.SqlDbType.Bit).Value = 0;
                        cmd2.Parameters.Add("@LockoutEnabled", System.Data.SqlDbType.Bit).Value = 1;
                        cmd2.Parameters.Add("@AccessFailedCount", System.Data.SqlDbType.Bit).Value = 0;
                        con.Open();
                        cmd2.ExecuteNonQuery();
                        TempData["success"] = "Access User created successfully";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            ViewData["type"] = type;
            return View(obj);
        }

        //GET: Edit
        public IActionResult Edit(string? id)
        {
            UsersModel um = new UsersModel();
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                 string sql = "SELECT Id,Nopeng,Nama,ActiveStatus,Email,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM AspNetUsers WHERE Id=@Id";
                 SqlCommand cmd = new SqlCommand(sql, con);
                 cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = id;
                 con.Open();
                 SqlDataReader dr = cmd.ExecuteReader();
                 if (dr.HasRows)
                 {
                     while (dr.Read())
                     {
                         um.Id = dr.GetString(0);
                         um.Nopeng = dr.GetString(1);
                         um.Nama = dr.GetString(2);
                         um.ActiveStatus = dr.GetInt32(3);
                         um.Email = dr.GetString(4);
                         um.CreatedBy = dr.GetString(5);
                         um.LastModifiedBy = dr.IsDBNull(6) ? null : dr.GetString(6);
                         um.CreatedDate = dr.GetDateTime(7);
                         um.LastModifiedDate = dr.IsDBNull(8) ? null : dr.GetDateTime(8);
                     }
                 }
                 con.Close();
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT RoleId FROM AspNetUserRoles WHERE UserId=@UserId";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = um.Id;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                      um.RoleId = dr.GetString(0);
                    }
                }
                con.Close();
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Name FROM AspNetRoles WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = um.RoleId;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {                     
                        um.RoleName = dr.GetString(0);
                    }
                }
                con.Close();
            }

            UsersModel roles_users = new UsersModel();
            roles_users.Roles = PopulateRoles();
            roles_users.Users = PopulateUsers();
            ViewData["user"] = um;
            ViewData["type"] = type;
            return View(roles_users);
        }
     
        //POST: Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit()
        {
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {              
                string sql = "UPDATE AspNetUsers SET ActiveStatus=@ActiveStatus,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE Id=@Id";
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@ActiveStatus", System.Data.SqlDbType.Int).Value = Int32.Parse(Request.Form["ActiveStatus"]); 
                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = Request.Form["Id"].ToString();
                cmd.Parameters.Add("@LastModifiedBy", System.Data.SqlDbType.NVarChar).Value = Request.Form["LastModifiedBy"].ToString();
                cmd.Parameters.Add("@LastModifiedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                cmd.ExecuteNonQuery();               
                con.Close();
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "UPDATE AspNetUserRoles SET RoleId=@RoleId WHERE UserId=@UserId";
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@RoleId", System.Data.SqlDbType.NVarChar).Value = Request.Form["RoleId"].ToString();
                cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = Request.Form["Id"].ToString();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            TempData["success"] = "Access User updated successfully";
            ViewData["type"] = type;
            return RedirectToAction(nameof(Index));          
        }

        //Users/Delete is replaced by Modal Delete Box in Index.cshtml
        //GET: Delete
        public IActionResult Delete(String? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            if (userId == id)
            {
                UsersModel um = new UsersModel();
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "SELECT Id,Nopeng,Nama,ActiveStatus,Email,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM AspNetUsers WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = id;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            um.Id = dr.GetString(0);
                            um.Nopeng = dr.GetString(1);
                            um.Nama = dr.GetString(2);
                            um.ActiveStatus = dr.GetInt32(3);
                            um.Email = dr.GetString(4);
                            um.CreatedBy = dr.GetString(5);
                            um.LastModifiedBy = dr.IsDBNull(6) ? null : dr.GetString(6);
                            um.CreatedDate = dr.GetDateTime(7);
                            um.LastModifiedDate = dr.IsDBNull(8) ? null : dr.GetDateTime(8);
                        }
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "SELECT RoleId FROM AspNetUserRoles WHERE UserId=@UserId";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = um.Id;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            um.RoleId = dr.GetString(0);
                        }
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "SELECT Name FROM AspNetRoles WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = um.RoleId;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            um.RoleName = dr.GetString(0);
                        }
                    }
                    con.Close();
                }
                UsersModel roles_users = new UsersModel();
                roles_users.Roles = PopulateRoles();
                roles_users.Users = PopulateUsers();
                ViewData["user"] = um;
                ViewData["type"] = type;
                TempData["error"] = "Access User cannot be deleted";
            }
            else
            {
                UsersModel um = new UsersModel();
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "SELECT Id,Nopeng,Nama,ActiveStatus,Email,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM AspNetUsers WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = id;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            um.Id = dr.GetString(0);
                            um.Nopeng = dr.GetString(1);
                            um.Nama = dr.GetString(2);
                            um.ActiveStatus = dr.GetInt32(3);
                            um.Email = dr.GetString(4);
                            um.CreatedBy = dr.GetString(5);
                            um.LastModifiedBy = dr.IsDBNull(6) ? null : dr.GetString(6);
                            um.CreatedDate = dr.GetDateTime(7);
                            um.LastModifiedDate = dr.IsDBNull(8) ? null : dr.GetDateTime(8);
                        }
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "SELECT RoleId FROM AspNetUserRoles WHERE UserId=@UserId";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = um.Id;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            um.RoleId = dr.GetString(0);
                        }
                    }
                    con.Close();
                }

                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "SELECT Name FROM AspNetRoles WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = um.RoleId;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            um.RoleName = dr.GetString(0);
                        }
                    }
                    con.Close();
                }

                UsersModel roles_users = new UsersModel();
                roles_users.Roles = PopulateRoles();
                roles_users.Users = PopulateUsers();
                ViewData["user"] = um;
                ViewData["type"] = type;
            }
            return View();
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            UsersModel um = new UsersModel();         
            //Check if Id is current User Id
            if (Request.Form["Id"].ToString() == userId)
            {
                TempData["error"] = "Cannot Delete Yourself!";
            }
            else
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "DELETE FROM AspNetUserRoles WHERE UserId=@UserId";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = Request.Form["Id"].ToString();
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
                {
                    string sql = "DELETE FROM AspNetUsers WHERE Id=@Id";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = Request.Form["Id"].ToString();
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["success"] = "Access User deleted successfully";
            }          
            ViewData["type"] = type;
            return RedirectToAction(nameof(Index));
        }


        //Users/Details/?id is replaced by Modal Details Box in Index.cshtml
        //GET: Details
        public IActionResult Details(String? id)
        {         
            UsersModel um = new UsersModel();
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Id,Nopeng,Nama,ActiveStatus,Email,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM AspNetUsers WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = id;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        um.Id = dr.GetString(0);
                        um.Nopeng = dr.GetString(1);
                        um.Nama = dr.GetString(2);
                        um.ActiveStatus = dr.GetInt32(3);
                        um.Email = dr.GetString(4);
                        um.CreatedBy = dr.GetString(5);
                        um.LastModifiedBy = dr.IsDBNull(6) ? null : dr.GetString(6);
                        um.CreatedDate = dr.GetDateTime(7);
                        um.LastModifiedDate = dr.IsDBNull(8) ? null : dr.GetDateTime(8);
                    }
                }
                con.Close();
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT RoleId FROM AspNetUserRoles WHERE UserId=@UserId";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = um.Id;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        um.RoleId = dr.GetString(0);
                    }
                }
                con.Close();
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                string sql = "SELECT Name FROM AspNetRoles WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar).Value = um.RoleId;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        um.RoleName = dr.GetString(0);
                    }
                }
                con.Close();
            }
            UsersModel roles_users = new UsersModel();
            roles_users.Roles = PopulateRoles();
            roles_users.Users = PopulateUsers();
            ViewData["user"] = um;
            ViewData["type"] = type;  
            return View();
        }
    }
}
