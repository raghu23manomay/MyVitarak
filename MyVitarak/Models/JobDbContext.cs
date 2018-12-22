using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MyVitarak.Models
{
    public class JobDbContext : DbContext
    {
        static JobDbContext()
        {
            Database.SetInitializer<JobDbContext>(null);

        }
        public JobDbContext() : base("Name=JobDbContext")
        {
        }

        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<ProductMaster> ProductMaster { get; set; }
        public DbSet<RouteDetails> RouteDetails { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<Vehical> Vehical { get; set; }
        public DbSet<VehicalDetails> VehicalDetails { get; set; }
        public DbSet<SupplierDetails> SupplierDetails { get; set; }
        public DbSet<SupplierMaster> SupplierMaster { get; set; }
        

        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerDetails> CustomerDetails { get; set; }
        public DbSet<CustomerList> CustomerList { get; set; }
        public DbSet<EmployeeList> EmployeeList { get; set; }
        public DbSet<OpeningBalance> OpeningBalance { get; set; }
        public DbSet<OpeningBalanceDeatils> OpeningBalanceDeatils { get; set; }
        

    }
}