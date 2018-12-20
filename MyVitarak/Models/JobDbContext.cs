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
       
    }
}