using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MyForum.DAL
{
    public class DataContextFactory : IDesignTimeDbContextFactory<MyForumDbContext>
    {
        public MyForumDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyForumDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=aspnet-MyForumDB;Trusted_Connection = True; MultipleActiveResultSets = true");
        return new MyForumDbContext(optionsBuilder.Options);
        }
    }
}
