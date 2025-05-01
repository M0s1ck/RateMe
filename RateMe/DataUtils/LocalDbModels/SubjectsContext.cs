using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.LocalDbModels
{
    internal class SubjectsContext : DbContext
    {
        public DbSet<SubjectLocal> Subjects { get; set; }
        public DbSet<ControlElementLocal> Elements { get; set; }

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
