using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyVitarak.Models;
using System.Data.SqlClient;
using PagedList;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Xml;
using System.Web.UI;

namespace MyVitarak.Controllers
{
    public class ActionController : Controller
    {
        // GET: Action
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchaseRates()
        {

            using (JobDbContext context = new JobDbContext())
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SP_EXECUTESQL123";

                        cmd.CommandType = CommandType.StoredProcedure;
                        // dataAdapter.Fill(ds);
                        //using (var reader = cmd.ExecuteReader())
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    var messege = ex.Message;
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }
                TempData["Data"] = dt;
                Download();

                return View(dt);
            }

        }


        [HttpGet]
        [ActionName("Download")]
        public void Download()
        {
            DataTable emps = TempData["Data"] as DataTable;
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = emps;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            string filePath = Server.MapPath("~/PurchaseRateClientXLSheet/" + 1 + "/generated/");

            bool isExists = System.IO.Directory.Exists(filePath);
            if (!isExists) { System.IO.Directory.CreateDirectory(filePath); }

            string fileName = "PurchaseRate" + ".xls";
            // Write the rendered content to a file.
            string renderedGridView = sw.ToString();
            System.IO.File.WriteAllText(filePath + fileName, renderedGridView);

        }



        [HttpGet]
        [ActionName("DownloadPurchaseExcel")]
        public void DownloadPurchaseExcel()
        {
            DataTable emps = TempData["Data"] as DataTable;
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = emps;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            string filePath = Server.MapPath("~/PurchaseClientXLSheet/" + 1 + "/generated/");

            bool isExists = System.IO.Directory.Exists(filePath);
            if (!isExists) { System.IO.Directory.CreateDirectory(filePath); }

            string fileName = "Purchase" + ".xls";
            // Write the rendered content to a file.
            string renderedGridView = sw.ToString();
            System.IO.File.WriteAllText(filePath + fileName, renderedGridView);

        }

        public ActionResult Purchase(DateTime? date)
        {

            using (JobDbContext context = new JobDbContext())
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                if (date == null)
                {
                    date = DateTime.Now;
                }

                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SP_LoadPurchase";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@orderdate", date));
                        // dataAdapter.Fill(ds);
                        //using (var reader = cmd.ExecuteReader())
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    var messege = ex.Message;
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }

                TempData["Data"] = dt;
                DownloadPurchaseExcel();
                return View(dt);
            }

        }

        public ActionResult PurchasePartial(DateTime? date)
        {

            using (JobDbContext context = new JobDbContext())
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                if (date == null)
                {
                    date = DateTime.Now;
                }

                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SP_LoadPurchase";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@orderdate", date));
                        // dataAdapter.Fill(ds);
                        //using (var reader = cmd.ExecuteReader())
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    var messege = ex.Message;
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }

                TempData["Data"] = dt;
                DownloadPurchaseExcel();

                return Request.IsAjaxRequest()
                     ? (ActionResult)PartialView("_partialPurchaseGrid", dt)
                     : View("_partialPurchaseGrid", dt);
            }

        }

    }
}