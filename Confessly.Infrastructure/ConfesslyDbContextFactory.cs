//using Confessly.Configuration;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Confessly.Infrastructure
//{
//    public class ConfesslyDbContextFactory : IDesignTimeDbContextFactory<ConfesslyDbContext>
//    {
//        public ConfesslyDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<ConfesslyDbContext>();
//            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=confessly;Username=postgres;Password=Changeme1234$");
//            return new ConfesslyDbContext(optionsBuilder.Options);
//        }
//    }
//}
