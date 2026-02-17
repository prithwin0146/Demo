using Microsoft.EntityFrameworkCore;
using EmployeeApi.DTOs;

namespace EmployeeApi.Models
{
    public partial class TaskDbContext : DbContext
    {
        public TaskDbContext()
        {
        }

        public TaskDbContext(DbContextOptions<TaskDbContext> options)
            : base(options)
        {
        }

        // Tables
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<EmployeeProject> EmployeeProjects { get; set; }
        public virtual DbSet<Department> Departments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TaskDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // EMPLOYEES
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.JobRole).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("Employee");
                entity.Property(e => e.DepartmentId);
            });

            // USERS 
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("Employee");
            });

            // PROJECTS
            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");

                entity.HasKey(e => e.ProjectId);

                entity.Property(e => e.ProjectName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate);
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            });

          
            // EMPLOYEE PROJECTS
            modelBuilder.Entity<EmployeeProject>(entity =>
            {
                entity.ToTable("EmployeeProjects");

                entity.HasKey(e => e.EmployeeProjectId);

                entity.Property(e => e.AssignedDate).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(100);
            });

            // DEPARTMENTS
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");

                entity.HasKey(e => e.DepartmentId);

                entity.Property(e => e.DepartmentName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ManagerId);
            });


            // Keyless entity for stored procedure results
            modelBuilder.Entity<EmployeeProjectDto>().HasNoKey();
            modelBuilder.Entity<ProjectPagedResult>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }
    }
}
