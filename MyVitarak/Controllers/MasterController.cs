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
            const int pageSize = 20;
            int totalCount = 20;
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
    }
}