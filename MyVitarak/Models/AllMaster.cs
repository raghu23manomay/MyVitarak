using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyVitarak.Models
{
    public class AllMaster
    {
    }

    public class ProductDetails
    {

        [Key]
        public int ProductID { get; set; }
        public string Product { get; set; }
        public int ProductBrandID { get; set; }
        public int CrateSize { get; set; }
        public Decimal GST { get; set; }
        public int? TotalRows { get; set; }

    }

    public class ProductMaster
    {

        [Key]
        public int ProductID { get; set; }
        public string Product { get; set; }
        public int ProductBrandID { get; set; }
        public int? StockCount { get; set; }
        public Decimal? SalePrice { get; set; }
        public int CrateSize { get; set; }
        public Decimal GST { get; set; }



    }
}