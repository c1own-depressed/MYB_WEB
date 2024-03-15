using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Persistence.Data
{
    public class MYBDbContext : DbContext
    {
        public MYBDbContext(DbContextOptions<MYBDbContext> options) : base(options) { }
        public MYBDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "server=localhost;port=3306;database=mybdatabase;user=root;password=1q2w3e4r";
                optionsBuilder.UseMySQL(connectionString);
            }
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Savings> Savings { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public string ConnectionString { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Savings>().ToTable("Savings");
            modelBuilder.Entity<Income>().ToTable("Income");
            modelBuilder.Entity<ExpenseCategory>().ToTable("ExpenseCategory");
            modelBuilder.Entity<Expense>().ToTable("Expense");
        }

    }
}
