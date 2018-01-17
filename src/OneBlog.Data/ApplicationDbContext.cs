using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using OneBlog.Helpers;
using OneBlog.Data.Mapping;

namespace OneBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IDbContextFactory _factory;

        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(IDbContextFactory factory, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _factory = factory;
        }
        /// <summary>
        /// 文章
        /// </summary>
        public DbSet<Posts> Posts { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public DbSet<Tags> Tags { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public DbSet<Categories> Categories { get; set; }
        /// <summary>
        /// 评论
        /// </summary>
        public DbSet<Comments> Comments { get; set; }
        /// <summary>
        /// 文章和分类关系
        /// </summary>
        public DbSet<PostsInCategories> PostsInCategories { get; set; }
        /// <summary>
        /// 分类和文章关系
        /// </summary>
        public DbSet<TagsInPosts> TagsInPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Override the name of the table because of a RC2 change
            builder.Entity<Posts>().ToTable("Posts");
            builder.Entity<Tags>().ToTable("Tags");
            builder.Entity<Categories>().ToTable("Categories");
            builder.Entity<PostsInCategories>().ToTable("PostsInCategories");
            builder.Entity<TagsInPosts>().ToTable("TagsInPosts");
            builder.Entity<Comments>().ToTable("Comments");

            var currentAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly;
            var typesToRegister = currentAssembly.GetTypes<BaseEntityMapping>();//获取所有数据提供类型
            foreach (var type in typesToRegister)
            {
                type.Execute(builder);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _factory.Configuring(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }
    }
}