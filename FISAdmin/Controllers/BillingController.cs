using FISAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FISAdmin.Controllers
{
    public class BillingController : Controller
    {
        private readonly IConfiguration config;
        List<BillingPaymentModel> result = new List<BillingPaymentModel>();
        public string sql = "", sql2 = "";
        public BillingPaymentModel bp = new BillingPaymentModel();

        public BillingController(IConfiguration configuration)
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
                case "BCB":
                    sql = "SELECT * FROM billing_category_bill";
                    break;
                case "BCC":
                    sql = "SELECT * FROM billing_customer_category";
                    break;
                case "BDT":
                    sql = "SELECT * FROM billing_doc_type";
                    break;
                case "BFD":
                    sql = "SELECT * FROM billing_fis_destination";
                    break;
                case "BP":
                    sql = "SELECT * FROM billing_priority";
                    break;
                case "BSCB":
                    sql = "SELECT * FROM billing_source_bill";
                    break;
                case "BSTB":
                    sql = "SELECT * FROM billing_status_bill";
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
                case "BCB":
                    sql = "SELECT id FROM billing_category_bill WHERE id=@id";
                    sql2 = "INSERT INTO billing_category_bill (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "BCC":
                    sql = "SELECT id FROM billing_customer_category WHERE id=@id";
                    sql2 = "INSERT INTO billing_customer_category (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "BDT":
                    sql = "SELECT id FROM billing_doc_type WHERE id=@id";
                    sql2 = "INSERT INTO billing_doc_type (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";  
                    break;
                case "BFD":
                    sql = "SELECT id FROM billing_fis_destination WHERE id=@id";
                    sql2 = "INSERT INTO billing_fis_destination (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "BP":
                    sql = "SELECT id FROM billing_priority WHERE id=@id";
                    sql2 = "INSERT INTO billing_priority (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "BSCB":
                    sql = "SELECT id FROM billing_source_bill WHERE id=@id";
                    sql2 = "INSERT INTO billing_source_bill (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
                    break;
                case "BSTB":
                    sql = "SELECT id FROM billing_status_bill WHERE id=@id";
                    sql2 = "INSERT INTO billing_status_bill (id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate) VALUES (@id,@description,@CreatedBy,@CreatedDate,@LastModifiedBy,@LastModifiedDate)";
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
                case "BCB":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM billing_category_bill WHERE id=@id";
                    break;
                case "BCC":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM billing_customer_category WHERE id=@id";
                    break;
                case "BDT":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM billing_doc_type WHERE id=@id";
                    break;
                case "BFD":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM billing_fis_destination WHERE id=@id";
                    break;
                case "BP":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM billing_priority WHERE id=@id";
                    break;
                case "BSCB":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM billing_source_bill WHERE id=@id";
                    break;
                case "BSTB":
                    sql = "SELECT id,description,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate FROM billing_status_bill WHERE id=@id";
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
                case "BCB":
                    sql = "UPDATE billing_category_bill SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "BCC":
                    sql = "UPDATE billing_customer_category SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "BDT":
                    sql = "UPDATE billing_doc_type SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "BFD":
                    sql = "UPDATE billing_fis_destination SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "BP":
                    sql = "UPDATE billing_priority SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "BSCB":
                    sql = "UPDATE billing_source_bill SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
                    break;
                case "BSTB":
                    sql = "UPDATE billing_status_bill SET description=@description,LastModifiedBy=@LastModifiedBy,LastModifiedDate=@LastModifiedDate WHERE id=@id";
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
                case "BCB":
                    sql = "DELETE FROM billing_category_bill WHERE id=@id";
                    break;
                case "BCC":
                    sql = "DELETE FROM billing_customer_category WHERE id=@id";
                    break;
                case "BDT":
                    sql = "DELETE FROM billing_doc_type WHERE id=@id";
                    break;
                case "BFD":
                    sql = "DELETE FROM billing_fis_destination WHERE id=@id";
                    break;
                case "BP":
                    sql = "DELETE FROM billing_priority WHERE id=@id";
                    break;
                case "BSCB":
                    sql = "DELETE FROM billing_source_bill WHERE id=@id";
                    break;
                case "BSTB":
                    sql = "DELETE FROM billing_status_bill WHERE id=@id";
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
