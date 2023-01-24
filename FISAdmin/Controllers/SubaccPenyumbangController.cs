using FISAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace FISAdmin.Controllers
{
    public class SubaccPenyumbangController : Controller
    {
        private readonly IConfiguration config;
        public List<SubaccModel> result = new List<SubaccModel>();
        public string type = "Subaccount Penyumbang";

        public SubaccPenyumbangController(IConfiguration configuration)
        {
            this.config = configuration;
        }

        //GET: Index
        public IActionResult Index()
        {

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                string sql = "SELECT kod,tajuk,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM subaccount_penyumbang";
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SubaccModel subacc = new SubaccModel();
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

            return View("~/Views/Subacc/Index.cshtml");
            /*return View("~/Views/Subaccount/Aktiviti.cshtml");*/
            /* return PartialView("~/Views/Subacc/_ViewIndex.cshtml");*/

        }


        //GET: Create
        public IActionResult Create()
        {
            ViewData["type"] = type;
                
            return View("~/Views/Subacc/Create.cshtml");
            /* return View();*/
            /*return PartialView("~/Views/Subacc/_ViewCreate.cshtml");*/
        }

        //POST: Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubaccModel obj)
        {

            if (obj.Kod.ToString() == obj.Tajuk.ToString())
            {
                ModelState.AddModelError("Kod", "Kod cannot exactly match Tajuk");
                ModelState.AddModelError("Tajuk", "Tajuk cannot exactly match Kod");
                TempData["error"] = "Subaccount Penyumbang cannot be created";
            }
            else
            {

                using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
                {
                    string sql = "SELECT kod FROM subaccount_penyumbang WHERE kod=@kod";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();

                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        ModelState.AddModelError("Kod", "Kod already exist");
                        TempData["error"] = "Subaccount Penyumbang cannot be created";
                    }
                    else
                    {
                        con.Close();

                        using (SqlConnection con2 = new SqlConnection(config.GetConnectionString("dbfis")))
                        {

                            string sql2 = "INSERT INTO subaccount_penyumbang (kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@kod,@tajuk,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";

                            SqlCommand cmd2 = new SqlCommand(sql2, con2);

                            cmd2.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();
                            cmd2.Parameters.Add("@tajuk", System.Data.SqlDbType.VarChar).Value = Request.Form["Tajuk"].ToString();
                            cmd2.Parameters.Add("@CreatedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["CreatedBy"].ToString();
                            cmd2.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                            cmd2.Parameters.Add("@LastModifiedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["LastModifiedBy"].ToString();
                            cmd2.Parameters.Add("@LastModifiedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;

                            con2.Open();
                            cmd2.ExecuteNonQuery();
                            TempData["success"] = "Subaccount Penyumbang created successfully";
                            return RedirectToAction(nameof(Index));
                        }

                    }

                }
            }


            /*return View(obj);*/
            ViewData["type"] = type;

            return View("~/Views/Subacc/Create.cshtml", obj);
            /*return PartialView("~/Views/Subacc/_ViewCreate.cshtml",obj);*/
        }


        //GET: Edit
        public IActionResult Edit(string id)
        {

            SubaccModel subacc = new SubaccModel();

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                string sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_penyumbang WHERE kod=@kod";
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
            /* return View(ViewData["subacc"]);*/

            return View("~/Views/Subacc/Edit.cshtml");
            /*return PartialView("~/Views/Subacc/_ViewEdit.cshtml");*/
        }



        //POST: Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit()
        {

            if (Request.Form["Kod"].ToString() == Request.Form["Tajuk"].ToString())
            {
                ModelState.AddModelError("Kod", "Kod cannot exactly match Tajuk");
                ModelState.AddModelError("Tajuk", "Tajuk cannot exactly match Kod");
            }
            else
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
                {
                    string sql = "UPDATE subaccount_penyumbang SET tajuk=@tajuk,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE kod=@kod";

                    con.Open();

                    SqlCommand cmd = new SqlCommand(sql, con);

                    cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();
                    cmd.Parameters.Add("@tajuk", System.Data.SqlDbType.VarChar).Value = Request.Form["Tajuk"].ToString();
                    cmd.Parameters.Add("@LastModifiedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["LastModifiedBy"].ToString();
                    cmd.Parameters.Add("@LastModifiedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;


                    cmd.ExecuteNonQuery();
                    TempData["success"] = "Subaccount Penyumbang updated successfully";
                    return RedirectToAction(nameof(Index));

                }
            }
            /*return View();*/
            ViewData["type"] = type;
                
            return View("~/Views/Subacc/Edit.cshtml");
            /*return PartialView("~/Views/Subacc/_ViewEdit.cshtml");*/
        }



        //GET: Delete
        public IActionResult Delete(string id)
        {
            SubaccModel subacc = new SubaccModel();

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                string sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_penyumbang WHERE kod=@kod";
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

            /* return View();*/
            return View("~/Views/Subacc/Delete.cshtml");
            /*return PartialView("~/Views/Subacc/_ViewDelete.cshtml");*/
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete()
        {
            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                string sql = "DELETE FROM subaccount_penyumbang WHERE kod=@kod";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.Add("@kod", System.Data.SqlDbType.VarChar).Value = Request.Form["Kod"].ToString();

                con.Open();
                cmd.ExecuteNonQuery();

            }
            TempData["success"] = "Subaccount Penyumbang deleted successfully";
            ViewData["type"] = type;

            return RedirectToAction(nameof(Index));

        }

        //GET: Details
        public IActionResult Details(string id)
        {
            SubaccModel subacc = new SubaccModel();

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                string sql = "SELECT kod,tajuk,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM subaccount_penyumbang WHERE kod=@kod";
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

            return View("~/Views/Subacc/Details.cshtml");
            /*return View();*/
            /* return PartialView("~/Views/Subacc/_ViewDetails.cshtml");*/
        }
    }
}
