using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class MYBDbContext : IdentityDbContext
    {
        public MYBDbContext(DbContextOptions<MYBDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Savings> Savings { get; set; }

        public DbSet<Income> Incomes { get; set; }

        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This ensures Identity configs are initialized

            // Custom configurations
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Savings>().ToTable("Savings");
            modelBuilder.Entity<Income>().ToTable("Income");
            modelBuilder.Entity<ExpenseCategory>().ToTable("ExpenseCategory");
            modelBuilder.Entity<Expense>().ToTable("Expense");
        }
    }
}
