using System;
using System.Data.Entity;
using ForumDb.Models;
using System.Linq;
using ForumDb.Data.Migrations;

namespace ForumDb.Data
{
    public class ForumDbContext : DbContext
    {
        public ForumDbContext()
            : base("ForumDb")
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Comment> Commentaries { get; set; }
        public DbSet<User> Users { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    Database.SetInitializer(new MigrateDatabaseToLatestVersion<ForumDbContext, Configuration>());

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
