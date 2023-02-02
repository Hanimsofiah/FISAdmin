using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using FISAdmin.Models;
using Microsoft.Data.SqlClient;

namespace FISAdmin.Controllers
{
    public class SubaccController : Controller
    {
        private readonly IConfiguration config;
        public List<SubaccModel> result = new List<SubaccModel>();
        public string sql = "", sql2 = "";
        public SubaccModel subacc = new SubaccModel();

        public SubaccController(IConfiguration configuration)
        {
            this.config = configuration;
        }

        //GET: Index
        public IActionResult Index(string type, string shortform)
        {
            int no = 0;
            string subacc_no= "subacc";

            switch (shortform)
            {
                case "SA":
                    sql = "SELECT kod,tajuk,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM subaccount_aktiviti";
                    break;
                case "SKP":
                    sql = "SELECT kod,tajuk,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM subaccount_kump_wang";
                    break;
                case "SP":
                    sql = "SELECT kod,tajuk,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM subaccount_penyumbang";
                    break;
                case "SS":
                    sql = "SELECT kod,tajuk,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM subaccount_sumber";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SubaccModel subacc = new SubaccModel();
                        no++;
                        subacc.subacc_no = subacc_no + no.ToString();
                        subacc.Kod = dr.GetString(0);
                        subacc.Tajuk = dr.GetString(1);
                        subacc.CreatedBy = dr.IsDBNull(2) ? null : dr.GetString(2);
                        subacc.LastModifiedBy = dr.IsDBNull(3) ? null : dr.GetString(3);
                        subacc.CreatedDate = dr.IsDBNull(4) ? null : dr.GetDateTime(4);
                        subacc.LastModifiedDate = dr.IsDBNull(5) ? null : dr.GetDateTime(5);
                        result.Add(subacc);
                    }
                }
            }

            ViewData["subacc"] = result;
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/Subacc/Index.cshtml");
        }

        //GET: Create
        public IActionResult Create(string type, string shortform)
        {
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/Subacc/Create.cshtml");
        }

        //POST: Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubaccModel obj, string type, string shortform)
        {
            switch (shortform)
            {
                case "SA":
                    sql = "SELECT kod FROM subaccount_aktiviti WHERE kod=@kod";
                    sql2 = "INSERT INTO subaccount_aktiviti (kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@kod,@tajuk,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "SKP":
                    sql = "SELECT kod FROM subaccount_kump_wang WHERE kod=@kod";
                    sql2 = "INSERT INTO subaccount_kump_wang (kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@kod,@tajuk,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "SP":
                    sql = "SELECT kod FROM subaccount_penyumbang WHERE kod=@kod";
                    sql2 = "INSERT INTO subaccount_penyumbang (kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@kod,@tajuk,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "SS":
                    sql = "SELECT kod FROM subaccount_sumber WHERE kod=@kod";
                    sql2 = "INSERT INTO subaccount_sumber (kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@kod,@tajuk,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                default:
                    break;
            }

            if (obj.Kod.ToString() == obj.Tajuk.ToString())
            {
                ModelState.AddModelError("Kod", "Kod cannot exactly match Tajuk");
                ModelState.AddModelError("Tajuk", "Tajuk cannot exactly match Kod");
                TempData["error"] = type + " cannot be created";
            }
            else
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        ModelState.AddModelError("Kod", "Kod already exist");
                        TempData["error"] = type + " cannot be created";
                    }
                    else
                    {
                        con.Close();
                        using (SqlConnection con2 = new SqlConnection(config.GetConnectionString("dbfis")))
                        {
                            SqlCommand cmd2 = new SqlCommand(sql2, con2);
                            cmd2.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();
                            cmd2.Parameters.Add("@tajuk", System.Data.SqlDbType.VarChar).Value = Request.Form["Tajuk"].ToString();
                            cmd2.Parameters.Add("@CreatedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["CreatedBy"].ToString();
                            cmd2.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                            cmd2.Parameters.Add("@LastModifiedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["LastModifiedBy"].ToString();
                            cmd2.Parameters.Add("@LastModifiedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                            con2.Open();
                            cmd2.ExecuteNonQuery();
                            TempData["success"] = type + " created successfully";
                            return RedirectToAction(nameof(Index), new {type = type, shortform = shortform});
                        }
                    }
                }
            }

            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/Subacc/Create.cshtml", obj);
            /*return PartialView("~/Views/Subacc/_ViewCreate.cshtml",obj);*/
        }

        //GET: Edit
        public IActionResult Edit(string id, string type, string shortform)
        {
            switch (shortform)
            {
                case "SA":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_aktiviti WHERE kod=@kod";
                    break;
                case "SKP":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_kump_wang WHERE kod=@kod";
                    break;
                case "SP":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_penyumbang WHERE kod=@kod";
                    break;
                case "SS":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_sumber WHERE kod=@kod";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = id;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        subacc.Kod = dr.GetString(0);
                        subacc.Tajuk = dr.GetString(1);
                        subacc.CreatedBy = dr.IsDBNull(2) ? null : dr.GetString(2);
                        subacc.CreatedDate = dr.IsDBNull(3) ? null : dr.GetDateTime(3);
                        subacc.LastModifiedBy = dr.IsDBNull(4) ? null : dr.GetString(4);
                        subacc.LastModifiedDate = dr.IsDBNull(5) ? null : dr.GetDateTime(5);
                    }
                }
            }

            ViewData["subacc"] = subacc;
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/Subacc/Edit.cshtml");
            /*return PartialView("~/Views/Subacc/_ViewEdit.cshtml");*/
        }

        //POST: Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string type, string shortform)
        {
            switch (shortform)
            {
                case "SA":
                    sql = "UPDATE subaccount_aktiviti SET tajuk=@tajuk,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE kod=@kod";
                    break;
                case "SKP":
                    sql = "UPDATE subaccount_kump_wang SET tajuk=@tajuk,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE kod=@kod";
                    break;
                case "SP":
                    sql = "UPDATE subaccount_penyumbang SET tajuk=@tajuk,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE kod=@kod";
                    break;
                case "SS":
                    sql = "UPDATE subaccount_sumber SET tajuk=@tajuk,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE kod=@kod";
                    break;
                default:
                    break;
            }

            if (Request.Form["Kod"].ToString() == Request.Form["Tajuk"].ToString())
            {
                ModelState.AddModelError("Kod", "Kod cannot exactly match Tajuk");
                ModelState.AddModelError("Tajuk", "Tajuk cannot exactly match Kod");
                TempData["error"] = type + " cannot be updated";
            }
            else
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();
                    cmd.Parameters.Add("@tajuk", System.Data.SqlDbType.VarChar).Value = Request.Form["Tajuk"].ToString();
                    cmd.Parameters.Add("@LastModifiedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["LastModifiedBy"].ToString();
                    cmd.Parameters.Add("@LastModifiedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TempData["success"] = type + " updated successfully";
                    return RedirectToAction(nameof(Index), new { type = type, shortform = shortform });
                }
            }
      
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/Subacc/Edit.cshtml");
            /*return PartialView("~/Views/Subacc/_ViewEdit.cshtml");*/
        }

        //GET: Delete
        public IActionResult Delete(string id, string type, string shortform)
        {
            switch (shortform)
            {
                case "SA":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_aktiviti WHERE kod=@kod";
                    break;
                case "SKP":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_kump_wang WHERE kod=@kod";
                    break;
                case "SP":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_penyumbang WHERE kod=@kod";
                    break;
                case "SS":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_sumber WHERE kod=@kod";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = id;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        subacc.Kod = dr.GetString(0);
                        subacc.Tajuk = dr.GetString(1);
                        subacc.CreatedBy = dr.IsDBNull(2) ? null : dr.GetString(2);
                        subacc.CreatedDate = dr.IsDBNull(3) ? null : dr.GetDateTime(3);
                        subacc.LastModifiedBy = dr.IsDBNull(4) ? null : dr.GetString(4);
                        subacc.LastModifiedDate = dr.IsDBNull(5) ? null : dr.GetDateTime(5);
                    }
                }
            }

            ViewData["subacc"] = subacc;
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/Subacc/Delete.cshtml");
            /*return PartialView("~/Views/Subacc/_ViewDelete.cshtml");*/
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string type, string shortform)
        {
            switch (shortform)
            {
                case "SA":
                    sql = "DELETE FROM subaccount_aktiviti WHERE kod=@kod";
                    break;
                case "SKP":
                    sql = "DELETE FROM subaccount_kump_wang WHERE kod=@kod";
                    break;
                case "SP":
                    sql = "DELETE FROM subaccount_penyumbang WHERE kod=@kod";
                    break;
                case "SS":
                    sql = "DELETE FROM subaccount_sumber WHERE kod=@kod";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();
                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["success"] = type + " deleted successfully";
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return RedirectToAction(nameof(Index), new { type = type, shortform = shortform });

        }

        //GET: Details
        public IActionResult Details(string id, string type, string shortform)
        {
            switch (shortform)
            {
                case "SA":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_aktiviti WHERE kod=@kod";
                    break;
                case "SKP":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_kump_wang WHERE kod=@kod";
                    break;
                case "SP":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_penyumbang WHERE kod=@kod";
                    break;
                case "SS":
                    sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_sumber WHERE kod=@kod";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = id;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        subacc.Kod = dr.GetString(0);
                        subacc.Tajuk = dr.GetString(1);
                        subacc.CreatedBy = dr.IsDBNull(2) ? null : dr.GetString(2);
                        subacc.CreatedDate = dr.IsDBNull(3) ? null : dr.GetDateTime(3);
                        subacc.LastModifiedBy = dr.IsDBNull(4) ? null : dr.GetString(4);
                        subacc.LastModifiedDate = dr.IsDBNull(5) ? null : dr.GetDateTime(5);
                    }
                }
            }

            ViewData["subacc"] = subacc;
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/Subacc/Details.cshtml");
            /* return PartialView("~/Views/Subacc/_ViewDetails.cshtml");*/
        }

    }
}
