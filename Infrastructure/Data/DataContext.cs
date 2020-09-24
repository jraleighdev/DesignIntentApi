using Domain.Models.CouldStorage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<CloudFile> CloudFiles { get; set; }

        public DbSet<BillItem> Bills { get; set; }
    }
}
