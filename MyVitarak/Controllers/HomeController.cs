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
    public class HomeController : Controller
    {
        
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

        public StaticPagedList<ProductDetails> ProductGridList(int? page)
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalCount = 8;
            ProductDetails Ulist = new ProductDetails();

            IEnumerable<ProductDetails> result = _db.ProductDetails.SqlQuery(@"exec GetProductList
                   @pPageIndex, @pPageSize",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize)

               ).ToList<ProductDetails>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<ProductDetails>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;



        }
    }
}