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

    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Address { get; set; }
        public int AreaID { get; set; }
        public string Mobile { get; set; }
    }
    public class EmployeeList
    {
        [Key]
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Address { get; set; }
        public int AreaID { get; set; }
        public string Mobile { get; set; }
    }

    public class EmployeeDetails
    {
        [Key]
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        IEnumerable<EmployeeDetails> EmployeeDetail { get; set; }
        public string Address { get; set; }
        public int AreaID { get; set; }
        public string Area { get; set; }
        public string Mobile { get; set; }
        public int? TotalRows { get; set; }
    }

    public class Vehical
    {
        [Key]
        public int VechicleID { get; set; }
        public string Transport { get; set; }
        public string Owner { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string VechicleNo { get; set; }
        public string Marathi { get; set; }
        public Decimal RatePerTrip { get; set; }
        public int PrintOrder { get; set; }
    }

    public class VehicalDetails
    {
        [Key]
        public int VechicleID { get; set; }
        public string Transport { get; set; }
        public string Owner { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string VechicleNo { get; set; }
        public string Marathi { get; set; }
        public Decimal RatePerTrip { get; set; }
        public int PrintOrder { get; set; }
        public int? TotalRows { get; set; }
    }


    public class SupplierDetails
    {
        [Key]
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string PersonMobileNo { get; set; }
        public string ContactPerson { get; set; }
        public string EmailID { get; set; }
        public Boolean IsActive { get; set; }
        public int? TotalRows { get; set; }

    }

    public class SupplierMaster
    {
        [Key]
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string PersonMobileNo { get; set; }
        public string ContactPerson { get; set; }
        public string EmailID { get; set; }
        public Boolean IsActive { get; set; }
        public string OfficeNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Address { get; set; }

    }

    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int AreaID { get; set; }
        public string Area { get; set; }
        public int SalesPersonID { get; set; }
        public string EmployeeName { get; set; }
        public int VehicleID { get; set; }
        public string VechicleNo { get; set; }
        public int? CustomerTypeId { get; set; }
        public string CustomerType { get; set; }
        public string CustomerNameEnglish { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public Boolean isBillRequired { get; set; }
        public Boolean isActive { get; set; }
        public Decimal DeliveryCharges { get; set; }
    }



    public class CustomerList
    {
        [Key]
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int AreaID { get; set; }
        public string Area { get; set; }
        public int SalesPersonID { get; set; }
        public string EmployeeName { get; set; }
        public int VehicleID { get; set; }
        public string VechicleNo { get; set; }
        public int? CustomerTypeId { get; set; }
        public string CustomerType { get; set; }
        public Boolean isBillRequired { get; set; }
        public Boolean isActive { get; set; }
        public Decimal DeliveryCharges { get; set; }
    }


    public class CustomerDetails
    {

        [Key]
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int AreaID { get; set; }
        public int SalesPersonID { get; set; }
        public string CustomerNameEnglish { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string EmployeeName { get; set; }
        public int VehicleID { get; set; }
        public Boolean isBillRequired { get; set; }
        public Boolean isActive { get; set; }
        public Decimal DeliveryCharges { get; set; }
        public string Area { get; set; }
        public string VechicleNo { get; set; }
        public int? TotalRows { get; set; }
    }

    public class OpeningBalance
    {
        [Key]
        public int CustomerID { get; set; }
        public Decimal PreviousBalance { get; set; }
    }

    public class RouteDetails
    {

        [Key] public int AreaID { get; set; }
        public string Area { get; set; }
        IEnumerable<RouteDetails> RouteDetail { get; set; }
        public int? TotalRows { get; set; }

    }

}