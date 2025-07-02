using System.IO;
using Microsoft.EntityFrameworkCore;
using RateMe.Models.LocalDbModels;

namespace RateMe.Repositories
{
    /// <summary>
    /// Context for local SqlLite db.
    /// Holds 2 tables: for subjects and for control elements.
    /// </summary>
    internal class SubjectsContext : DbContext
    {
        public DbSet<SubjectLocal> Subjects { get; set; }
        public DbSet<ElementLocal> Elements { get; set; }

        public string DbPath { get; }

        public SubjectsContext()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            DbPath = Path.Combine(path, "subjects.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    }
}
