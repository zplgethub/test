using Microsoft.EntityFrameworkCore;
using VSIMS.WebApi.Entity;

namespace VSIMS.WebApi.Data
{
    /// <summary>
    /// 创建数据库上下文类
    /// </summary>
    public class ImsDataContext:DbContext
    {
        public DbSet<UserEntity> Users { get; set; }

        public DbSet<MenuEntity> Menus { get; set; }

        public DbSet<RoleEntity> Roles { get; set; }

        public DbSet<UserRoleEntity> UserRoles { get; set; }

        public DbSet<RoleMenuEntity> RoleMenus { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public DbSet<StudentEntity> Students { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public DbSet<ClassesEntity> Classes { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public DbSet<CourseEntity> Courses { get; set; }

        /// <summary>
        /// 成绩
        /// </summary>
        public DbSet<ScoreEntity> Scores { get; set; }

        public ImsDataContext(DbContextOptions<ImsDataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserEntity>().ToTable("Users");
            modelBuilder.Entity<MenuEntity>().ToTable("Menus");
            modelBuilder.Entity<StudentEntity>().ToTable("Students");
            modelBuilder.Entity<RoleEntity>().ToTable("Roles");
            modelBuilder.Entity<UserRoleEntity>().ToTable("UserRoles");
            modelBuilder.Entity<RoleMenuEntity>().ToTable("RoleMenus");
        }
    }
}
