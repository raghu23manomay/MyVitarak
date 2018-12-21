using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyVitarak.Models;
using System.Data.SqlClient;
using PagedList;
using System.Data;

namespace MyVitarak.Controllers
{
    public class MasterController : Controller
    {
        // GET: Master
        [HttpGet]
        public ActionResult importexcel(string MasterName = "")
        {
            Session["MasterName"] = MasterName;
            return View();
        }

        [HttpPost]
        public ActionResult importexcel(HttpPostedFileBase file, Route L)
        {
            DataTable dt1 = new DataTable();
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/uploads/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.SetAttributes(fileLocation, FileAttributes.Normal);
                        //   System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);

                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    if (excelConnection.State == ConnectionState.Open)
                    {
                        excelConnection.Close();
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                    if (excelConnection1.State == ConnectionState.Open)
                    {
                        excelConnection1.Close();
                    }
                }
                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/uploads/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }
                dt1 = ds.Tables[0] as DataTable;
                Session.Add("dt1", dt1);
                L.dtTable = dt1;
            }
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("importexcel", L)
                : View(L);
        }

        public ActionResult Index(int? page)
        {
            StaticPagedList<ProductDetails> itemsAsIPagedList;
            itemsAsIPagedList = ProductGridList(page);

            Session["MasterName"] = "ProductMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("Index", itemsAsIPagedList)
                    : View("Index", itemsAsIPagedList);
        }

        //================================== Fill Product Grid Code ===========================================

        public StaticPagedList<ProductDetails> ProductGridList(int? page,string pname="")
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 5;
            int totalCount = 5;
            ProductDetails Ulist = new ProductDetails();

            IEnumerable<ProductDetails> result = _db.ProductDetails.SqlQuery(@"exec GetProductList
                   @pPageIndex, @pPageSize,@pname",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize),
               new SqlParameter("@pname",pname)

               ).ToList<ProductDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<ProductDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;
                       
        }

        public ActionResult LoadDataForProduct(int? page, string pname = "")
        {
            StaticPagedList<ProductDetails> itemsAsIPagedList;
            itemsAsIPagedList = ProductGridList(page,pname);

            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("_partialGridProductMaster", itemsAsIPagedList)
                    : View("_partialGridProductMaster", itemsAsIPagedList);
        }

        [HttpGet]
        public ActionResult Add_Product()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductMaster pm)
        {
            JobDbContext _db = new JobDbContext();
            try
            {

                var res = _db.Database.ExecuteSqlCommand(@"exec [UC_InsertProductMast] @Product,@ProductBrandID,@StockCount,@SalePrice,@CrateSize,@GST",
                    new SqlParameter("@Product", pm.Product),
                    new SqlParameter("@ProductBrandID", pm.ProductBrandID),
                    new SqlParameter("@StockCount", 1),
                    new SqlParameter("@SalePrice", pm.SalePrice == null ? (object)DBNull.Value : pm.SalePrice),
                    new SqlParameter("@CrateSize", pm.CrateSize),
                    new SqlParameter("@GST", pm.GST));

                if (res == 0)
                {
                    return Json("Product Already Exist");
                }
                else
                {
                    return Json("Data Added Sucessfully");
                }


            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }


        //========================================== Edit Product ================================================

        public ActionResult EditProduct()
        {
            return View();

        }

        public ActionResult FetchProductForUpdate(int? ProductID)
        {
            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.ProductMaster.SqlQuery(@"exec [UC_FetchDataForUpdate_ProductMaster] @ProductID",
                    new SqlParameter("@ProductID", ProductID)
                   ).ToList<ProductMaster>();

                ProductMaster rs = new ProductMaster();
                rs = res.FirstOrDefault();
                return View("EditProduct", rs);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }

        [HttpPost]
        public ActionResult Updateproduct(ProductMaster rm)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_UpdateProductMast @ProductID ,@Product ,@ProductBrandID ,@StockCount ,@SalePrice,@CrateSize ,@GST",
                    new SqlParameter("@ProductID", rm.ProductID),
                    new SqlParameter("@Product", rm.Product),
                    new SqlParameter("@ProductBrandID", rm.ProductBrandID),
                    new SqlParameter("@StockCount", rm.StockCount),
                    new SqlParameter("@SalePrice", rm.SalePrice == null ? (object)DBNull.Value : rm.SalePrice),
                    new SqlParameter("@CrateSize", rm.CrateSize),
                    new SqlParameter("@GST", rm.GST));

                return Json("Data Updated Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }


        [HttpPost]
        public ActionResult DeleteProduct(ProductMaster rm)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_DeleteProductMast @ProductID",
                    new SqlParameter("@ProductID", rm.ProductID));

                return Json("Data Deleted Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }



        public List<SelectListItem> binddropdown(string action, int val = 0)
        {
            JobDbContext _db = new JobDbContext();

            var res = _db.Database.SqlQuery<SelectListItem>("exec USP_BindDropDown @action , @val",
                   new SqlParameter("@action", action),
                    new SqlParameter("@val", val))
                   .ToList()
                   .AsEnumerable()
                   .Select(r => new SelectListItem
                   {
                       Text = r.Text.ToString(),
                       Value = r.Value.ToString(),
                       Selected = r.Value.Equals(Convert.ToString(val))
                   }).ToList();

            return res;
        }
        public JsonResult GetArea()
        {
            JobDbContext _db = new JobDbContext();
            var lstItem = binddropdown("Area", 0).Select(i => new { i.Value, i.Text }).ToList();
            return Json(lstItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployee()
        {
            JobDbContext _db = new JobDbContext();
            var lstItem = binddropdown("Employee", 0).Select(i => new { i.Value, i.Text }).ToList();
            return Json(lstItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVehicle()
        {
            JobDbContext _db = new JobDbContext();
            var lstItem = binddropdown("Vehicle", 0).Select(i => new { i.Value, i.Text }).ToList();
            return Json(lstItem, JsonRequestBehavior.AllowGet);
        }

        // GET: Master
        public ActionResult EmployeeIndex()
        {
            return View();
        }
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult LoadData(int? page, String Name)
        {
            StaticPagedList<EmployeeDetails> itemsAsIPagedList;
            itemsAsIPagedList = GridList(page, Name);

            Session["MasterName"] = "EmployeeMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("Partial_EmployeeGridList", itemsAsIPagedList)
                    : View("Partial_EmployeeGridList", itemsAsIPagedList);
        }

        public StaticPagedList<EmployeeDetails> GridList(int? page, String Name)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            EmployeeDetails Ulist = new EmployeeDetails();
            if (Name == null) Name = "";

            IEnumerable<EmployeeDetails> result = _db.EmployeeDetails.SqlQuery(@"exec GetEmployeeList
                   @pPageIndex, @pPageSize,@pName",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize),
               new SqlParameter("@pName", Name)

               ).ToList<EmployeeDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<EmployeeDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }


        /************************************************Add Employee************************************************************/
        [HttpGet]
        public ActionResult Add_Employee()
        {
            ViewData["Area"] = binddropdown("Area", 0);

            return View();
        }

        [HttpPost]
        public ActionResult AddEmployee(Employee pm)
        {
            JobDbContext _db = new JobDbContext();
            try
            {

                var res = _db.Database.ExecuteSqlCommand(@"exec uspInsertEmployee @EmployeeName,@Address,@AreaID,@Mobile",
                    new SqlParameter("@EmployeeName", pm.EmployeeName),
                    new SqlParameter("@Address", pm.Address),
                    new SqlParameter("@AreaID", pm.AreaID),
                    new SqlParameter("@Mobile", pm.Mobile));

                return Json("Data Added Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }
        public ActionResult IndexForEmployeeMaster(int? page, String Name)
        {
            StaticPagedList<EmployeeDetails> itemsAsIPagedList;
            itemsAsIPagedList = EmployeeGridList(page, Name);

            Session["MasterName"] = "EmployeeMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("IndexForEmployeeMaster", itemsAsIPagedList)
                    : View("IndexForEmployeeMaster", itemsAsIPagedList);
        }

        public StaticPagedList<EmployeeDetails> EmployeeGridList(int? page, String Name)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 20;
            int totalCount = 8;
            EmployeeDetails Ulist = new EmployeeDetails();
            if (Name == null) Name = "";

            IEnumerable<EmployeeDetails> result = _db.EmployeeDetails.SqlQuery(@"exec GetEmployeeList
                   @pPageIndex, @pPageSize,@pName",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize),
               new SqlParameter("@pName", Name)

               ).ToList<EmployeeDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<EmployeeDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveEmployeeExcelData(List<Employee> SaveEmployeeData)
        {
            try
            {
                JobDbContext _db = new JobDbContext();

                if (SaveEmployeeData.Count > 0)
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add("EmployeeID", typeof(int));
                    dt.Columns.Add("EmployeeName", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("AreaID", typeof(int));
                    dt.Columns.Add("Mobile", typeof(string));
                    dt.Columns.Add("UserId", typeof(int));

                    foreach (var item in SaveEmployeeData)
                    {
                        DataRow dr = dt.NewRow();
                        dr["EmployeeID"] = 1;
                        dr["EmployeeName"] = item.EmployeeName;
                        dr["Address"] = item.Address;
                        dr["AreaID"] = 2;
                        dr["Mobile"] = item.Mobile;
                        dr["UserId"] = 1;

                        if (item.EmployeeName == null)
                        {
                            return Json("Employee Name Missing");
                        }
                        if (item.Address == null)
                        {
                            return Json("Address missing");
                        }
                        if (item.AreaID == 0)
                        {
                            return Json("Area Id Missing");
                        }
                        if (item.Mobile == null)
                        {
                            return Json("Mobile number Missing");
                        }

                        if (item.EmployeeName != null)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    SqlParameter tvpParam = new SqlParameter();
                    tvpParam.ParameterName = "@EmployeeParameters";
                    tvpParam.SqlDbType = System.Data.SqlDbType.Structured;
                    tvpParam.Value = dt;
                    tvpParam.TypeName = "UT_EmployeeMasters";

                    var res = _db.Database.ExecuteSqlCommand(@"exec USP_InsertExcelData_EmployeeMaster @EmployeeParameters",
                     tvpParam);

                }
                // return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate")
                //: View();
                return Request.IsAjaxRequest() ? (ActionResult)Json("Excel Imported Sucessfully")
                : Json("Excel Imported Sucessfully");
            }
            catch (Exception e)

            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);
            }

        }

        /*******************************************EditEmployee*****************************************************/
        public ActionResult EditEmployee(int EmployeeID)
        {
            JobDbContext _db = new JobDbContext();
            EmployeeList md = new EmployeeList();
            ViewData["Area"] = binddropdown("Area", 0);
            var result = _db.EmployeeList.SqlQuery(@"exec uspSelectEmployeeMastByEmployeeID @EmployeeID
                ",
                new SqlParameter("@EmployeeID", EmployeeID)).ToList<EmployeeList>();
            md = result.FirstOrDefault();

            return Request.IsAjaxRequest()
               ? (ActionResult)PartialView("EditEmployee", md)
               : View("EditEmployee", md);
        }

        [HttpPost]
        public ActionResult UpdateEmployee(Employee up)
        {
            JobDbContext _db = new JobDbContext();

            try
            {
                var result = _db.Database.ExecuteSqlCommand(@"exec uspUpdateEmployee @EmployeeID,@EmployeeName,@Address,@AreaID,@Mobile",
                    new SqlParameter("@EmployeeID", up.EmployeeID),
                     new SqlParameter("@EmployeeName", up.EmployeeName),
                    new SqlParameter("@Address", up.Address),
                    new SqlParameter("@AreaID", up.AreaID),
                    new SqlParameter("@Mobile", up.Mobile));
                return Json("Data Updated Sucessfully");
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                return Json(up, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult DeleteEmployee(int? EmployeeID)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_DeleteEmployeeMast @EmployeeID",
                    new SqlParameter("@EmployeeID", EmployeeID));

                return Json("Data Deleted Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }

        }


        /************************************************Add Vehical************************************************************/

        public ActionResult Add_Vehical()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddVehical(Vehical pm)
        {
            JobDbContext _db = new JobDbContext();
            try
            {

                var res = _db.Database.ExecuteSqlCommand(@"exec UC_VehicleMast_Insert @Transport,@Owner,@Address,@Mobile,@VechicleNo,@RatePerTrip,@Marathi,@PrintOrder",
                    new SqlParameter("@Transport", pm.Transport),
                    new SqlParameter("@Owner", pm.Owner),
                    new SqlParameter("@Address", pm.Address),
                    new SqlParameter("@Mobile", pm.Mobile),
                    new SqlParameter("@VechicleNo", pm.VechicleNo),
                    new SqlParameter("@RatePerTrip", pm.RatePerTrip),
                    new SqlParameter("@Marathi", pm.Marathi),
                    new SqlParameter("@PrintOrder", pm.PrintOrder)
                    );

                return Json("Data Added Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }
        }

        public ActionResult IndexForVehicalMaster(int? page)
        {
            StaticPagedList<VehicalDetails> itemsAsIPagedList;
            itemsAsIPagedList = VehicalGridList(page);

            Session["MasterName"] = "VehicalMaster";
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("IndexForVehicalMaster", itemsAsIPagedList)
                    : View("IndexForVehicalMaster", itemsAsIPagedList);
        }

        public StaticPagedList<VehicalDetails> VehicalGridList(int? page)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            VehicalDetails Ulist = new VehicalDetails();

            IEnumerable<VehicalDetails> result = _db.VehicalDetails.SqlQuery(@"exec GetVehicalList
                   @pPageIndex, @pPageSize",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize)

               ).ToList<VehicalDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<VehicalDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveVehicalExcelData(List<Vehical> SaveVehicalData)
        {
            try
            {
                JobDbContext _db = new JobDbContext();

                if (SaveVehicalData.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VechicleID", typeof(int));
                    dt.Columns.Add("Transport", typeof(string));
                    dt.Columns.Add("Owner", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("Mobile", typeof(string));
                    dt.Columns.Add("VechicleNo", typeof(string));
                    dt.Columns.Add("RatePerTrip", typeof(decimal));
                    dt.Columns.Add("Marathi", typeof(string));
                    dt.Columns.Add("PrintOrder", typeof(int));
                    foreach (var item in SaveVehicalData)
                    {
                        DataRow dr = dt.NewRow();
                        dr["VechicleID"] = 1;
                        dr["Transport"] = item.Transport;
                        dr["Owner"] = item.Owner;
                        dr["Address"] = item.Address;
                        dr["Mobile"] = item.Mobile;
                        dr["VechicleNo"] = item.VechicleNo;
                        dr["RatePerTrip"] = item.RatePerTrip;
                        dr["Marathi"] = item.Marathi;
                        dr["PrintOrder"] = item.PrintOrder;
                        if (item.Transport != null)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    SqlParameter tvpParam = new SqlParameter();
                    tvpParam.ParameterName = "@VehicalParameters";
                    tvpParam.SqlDbType = System.Data.SqlDbType.Structured;
                    tvpParam.Value = dt;
                    tvpParam.TypeName = "UT_VehicalMaster";

                    var res = _db.Database.ExecuteSqlCommand(@"exec USP_InsertExcelData_VehicalMaster @VehicalParameters",
                     tvpParam);

                }
                // return Request.IsAjaxRequest() ? (ActionResult)PartialView("ImportLaneRate")
                //: View();
                return Request.IsAjaxRequest() ? (ActionResult)Json("Excel Imported Sucessfully")
                : Json("Excel Imported Sucessfully");
            }
            catch (Exception e)

            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);
            }

        }

        /*******************************************EditEmployee*****************************************************/

        public ActionResult EditVehical(int VechicleID)
        {
            JobDbContext _db = new JobDbContext();
            Vehical md = new Vehical();
            var result = _db.Vehical.SqlQuery(@"exec UC_VehicleMast_GetByPK @VechicleID
                ",
                new SqlParameter("@VechicleID", VechicleID)).ToList<Vehical>();
            md = result.FirstOrDefault();
            return Request.IsAjaxRequest()
               ? (ActionResult)PartialView("EditVehical", md)
               : View("EditVehical", md);
        }

        [HttpPost]
        public ActionResult UpdateVehical(Vehical up)
        {
            JobDbContext _db = new JobDbContext();

            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_VehicleMast_UpdateByPK @VechicleID, @Transport,@Owner,@Address,@Mobile,@VechicleNo,@RatePerTrip,@Marathi,@PrintOrder",
                     new SqlParameter("@VechicleID", up.VechicleID),
                    new SqlParameter("@Transport", up.Transport),
                    new SqlParameter("@Owner", up.Owner),
                    new SqlParameter("@Address", up.Address),
                    new SqlParameter("@Mobile", up.Mobile),
                    new SqlParameter("@VechicleNo", up.VechicleNo),
                    new SqlParameter("@RatePerTrip", up.RatePerTrip),
                    new SqlParameter("@Marathi", up.Marathi),
                    new SqlParameter("@PrintOrder", up.PrintOrder)
                    );

                return Json("Data Updated Sucessfully");

            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                return Json(up, JsonRequestBehavior.AllowGet);

            }



        }


        [HttpPost]
        public ActionResult DeleteVehicle(int? VechicleID)
        {

            JobDbContext _db = new JobDbContext();
            try
            {
                var res = _db.Database.ExecuteSqlCommand(@"exec UC_CustomerMast_DeleteByPK @VechicleID",
                    new SqlParameter("@VechicleID", VechicleID));

                return Json("Data Deleted Sucessfully");
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return Json(message);

            }




        public ActionResult OpeningBalance()
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
                        cmd.CommandText = "OpeningBalanceList";
                        cmd.CommandType = CommandType.StoredProcedure;
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

                return View(dt);
            }

        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveOpeningBalance(List<OpeningBalance> SaveLaneRate)
        {
            try
            {
                JobDbContext _db = new JobDbContext();

                if (SaveLaneRate.Count > 0)
                {
                    foreach (var item in SaveLaneRate)
                    {
                        _db.Database.ExecuteSqlCommand(@"exec uspUpdateOpeniningBalance @PreviousBalance,@CustomerId",
                              new SqlParameter("@PreviousBalance", item.PreviousBalance), new SqlParameter("@CustomerId", item.CustomerID));

                    }

                }
                return Json("Opening Balance Added Sucessfully");
            }
            catch (Exception e)



            {
                var messege = e.Message;
                return Request.IsAjaxRequest() ? (ActionResult)Json(messege)
               : Json(messege);

            }
        }

    }
}