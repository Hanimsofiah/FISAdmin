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
        public string SKP_type = "Subaccount Kumpulan Wang";
        public string SA_type = "Subaccount Aktiviti";


        public SubaccController(IConfiguration configuration)
        {
            this.config = configuration;
        }

        //GET: SKP_Index
        public IActionResult SKP_Index()
        {

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                string sql = "SELECT kod,tajuk,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM subaccount_kump_wang";
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
            ViewData["type"] = SKP_type;

            return View("~/Views/Subacc/Index.cshtml");
            /*return View("~/Views/Subaccount/Aktiviti.cshtml");*/
            /* return PartialView("~/Views/Subacc/_ViewIndex.cshtml");*/

        }

        //GET: SA_Index
        public IActionResult SA_Index()
        {

            using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbfis")))
            {
                string sql = "SELECT kod,tajuk,CreatedBy,LastModifiedBy,CreatedDate,LastModifiedDate FROM subaccount_aktiviti";
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
            ViewData["type"] = SA_type;

            return View("~/Views/Subacc/Index.cshtml");
            /*return View("~/Views/Subaccount/Aktiviti.cshtml");*/
            /* return PartialView("~/Views/Subacc/_ViewIndex.cshtml");*/

        }
    }
}
