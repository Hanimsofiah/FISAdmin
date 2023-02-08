using FISAdmin.Models;
using FISAdmin.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using Microsoft.AspNetCore.Identity;

namespace FISAdmin.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration config;
        List<BillingPaymentModel> result = new List<BillingPaymentModel>();
        public string sql = "", sql2 = "";
        public BillingPaymentModel bp = new BillingPaymentModel();

        public PaymentController(IConfiguration configuration)
        {
            this.config = configuration;
        }

        //GET: Index
        public IActionResult Index(string type, string shortform)
        {
            int no = 0;
            string bp = "bp";

            switch (shortform)
            {
                case "PDT":
                    sql = "SELECT * FROM payment_doc_type";
                    break;
                case "PSC":
                    sql = "SELECT * FROM payment_source";
                    break;
                case "PST":
                    sql = "SELECT * FROM payment_status";
                    break;
                case "PT":
                    sql = "SELECT * FROM payment_type";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("ApplicationDbContextConnection")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        BillingPaymentModel obj = new BillingPaymentModel();
                        no++;
                        obj.bp_no = bp + no.ToString();
                        obj.Id = sdr["id"].ToString();
                        obj.Description = sdr["description"].ToString();
                        obj.CreatedBy = sdr.IsDBNull(2) ? null : sdr.GetString(2);
                        obj.CreatedDate = sdr.IsDBNull(3) ? null : sdr.GetDateTime(3);
                        obj.LastModifiedBy = sdr.IsDBNull(4) ? null : sdr.GetString(4);
                        obj.LastModifiedDate = sdr.IsDBNull(5) ? null : sdr.GetDateTime(5);
                        result.Add(obj);
                    }
                }
                con.Close();
            }
            ViewData["bp"] = result;
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/BillingPayment/Index.cshtml");
        }

        //GET: Create
        public IActionResult Create(string type, string shortform)
        {
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/BillingPayment/Create.cshtml");
        }

        //POST: Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BillingPaymentModel obj, string type, string shortform)
        {
            switch (shortform)
            {
                case "PDT":
                    sql = "SELECT id FROM payment_doc_type WHERE id=@id";
                    sql2 = "INSERT INTO payment_doc_type (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";             
                    break;
                case "PSC":
                    sql = "SELECT id FROM payment_source WHERE id=@id";
                    sql2 = "INSERT INTO payment_source (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "PST":
                    sql = "SELECT id FROM payment_status WHERE id=@id";
                    sql2 = "INSERT INTO payment_status (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "PT":
                    sql = "SELECT id FROM payment_type WHERE id=@id";
                    sql2 = "INSERT INTO payment_type (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                default:
                    break;
            }

            if (obj.Id.ToString() == obj.Description.ToString())
            {
                ModelState.AddModelError("Id", "Id cannot exactly match Description");
                ModelState.AddModelError("Description", "Description cannot exactly match Id");
                TempData["error"] = type + " cannot be created";
            }
            else
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.VarChar).Value = Request.Form["Id"].ToString();
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        ModelState.AddModelError("Id", "Id already exist");
                        TempData["error"] = type + " cannot be created";
                    }
                    else
                    {
                        con.Close();
                        using (SqlConnection con2 = new SqlConnection(config.GetConnectionString("dbfis")))
                        {
                            SqlCommand cmd2 = new SqlCommand(sql2, con2);
                            cmd2.Parameters.Add("@id", System.Data.SqlDbType.VarChar).Value = Request.Form["Id"].ToString();
                            cmd2.Parameters.Add("@description", System.Data.SqlDbType.VarChar).Value = Request.Form["Description"].ToString();
                            cmd2.Parameters.Add("@CreatedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["CreatedBy"].ToString();
                            cmd2.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                            cmd2.Parameters.Add("@LastModifiedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["LastModifiedBy"].ToString();
                            cmd2.Parameters.Add("@LastModifiedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                            con2.Open();
                            cmd2.ExecuteNonQuery();
                            TempData["success"] = type + " created successfully";
                            return RedirectToAction(nameof(Index), new { type = type, shortform = shortform });
                        }
                    }
                }
            }

            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/BillingPayment/Create.cshtml", obj);
        }

        //GET: Edit
        public IActionResult Edit(string id, string type, string shortform)
        {
            switch (shortform)
            {
                case "PDT":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM payment_doc_type WHERE id=@id";
                    break;
                case "PSC":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM payment_source WHERE id=@id";
                    break;
                case "PST":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM payment_status WHERE id=@id";
                    break;
                case "PT":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM payment_type WHERE id=@id";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@id", System.Data.SqlDbType.VarChar).Value = id;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        bp.Id = dr.GetString(0);
                        bp.Description = dr.GetString(1);
                        bp.CreatedBy = dr.IsDBNull(2) ? null : dr.GetString(2);
                        bp.CreatedDate = dr.IsDBNull(3) ? null : dr.GetDateTime(3);
                        bp.LastModifiedBy = dr.IsDBNull(4) ? null : dr.GetString(4);
                        bp.LastModifiedDate = dr.IsDBNull(5) ? null : dr.GetDateTime(5);
                    }
                }
            }

            ViewData["bp"] = bp;
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/BillingPayment/Edit.cshtml");
            /*return PartialView("~/Views/bp/_ViewEdit.cshtml");*/
        }

        //POST: Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string type, string shortform)
        {
            switch (shortform)
            {
                case "PDT":
                    sql = "UPDATE payment_doc_type SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "PSC":
                    sql = "UPDATE payment_source SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "PST":
                    sql = "UPDATE payment_status SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "PT":
                    sql = "UPDATE payment_type SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                default:
                    break;
            }

            if (Request.Form["Id"].ToString() == Request.Form["Description"].ToString())
            {
                ModelState.AddModelError("Id", "Id cannot exactly match Description");
                ModelState.AddModelError("Description", "Description cannot exactly match Id");
                TempData["error"] = type + " cannot be updated";
            }
            else
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.VarChar).Value = Request.Form["Id"].ToString();
                    cmd.Parameters.Add("@description", System.Data.SqlDbType.VarChar).Value = Request.Form["Description"].ToString();
                    cmd.Parameters.Add("@LastModifiedBy", System.Data.SqlDbType.VarChar).Value = Request.Form["LastModifiedBy"].ToString();
                    cmd.Parameters.Add("@LastModifiedDate", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.ExecuteNonQuery();
                    TempData["success"] = type + " updated successfully";
                    return RedirectToAction(nameof(Index), new { type = type, shortform = shortform });
                }
            }

            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return View("~/Views/BillingPayment/Edit.cshtml");
        }

        //POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string type, string shortform)
        {
            switch (shortform)
            {
                case "PDT":
                    sql = "DELETE FROM payment_doc_type WHERE id=@id";
                    break;
                case "PSC":
                    sql = "DELETE FROM payment_source WHERE id=@id";
                    break;
                case "PST":
                    sql = "DELETE FROM payment_status WHERE id=@id";
                    break;
                case "PT":
                    sql = "DELETE FROM payment_type WHERE id=@id";
                    break;
                default:
                    break;
            }

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@id", System.Data.SqlDbType.VarChar).Value = Request.Form["Id"].ToString();
                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["success"] = type + " deleted successfully";
            ViewData["type"] = type;
            ViewData["shortform"] = shortform;

            return RedirectToAction(nameof(Index), new { type = type, shortform = shortform });

        }
    }
}
