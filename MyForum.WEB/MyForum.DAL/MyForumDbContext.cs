using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyForum.BL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.DAL
{
    public class MyForumDbContext : IdentityDbContext<User>
    {
        public DbSet<Blog>? Blogs { get; set; }
        public DbSet<Post>? Posts { get; set; }
        public override DbSet<User>? Users { get; set; }
        public DbSet<Comment>? Comments { get; set; }
        public DbSet<Theme>? Themes { get; set; }
        public DbSet<Repl>? Repls { get; set; }
        public MyForumDbContext(DbContextOptions<MyForumDbContext> options)
            : base(options)
        {
        }
    }
}
