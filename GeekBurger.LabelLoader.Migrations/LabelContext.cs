using GeekBurger.LabelLoader.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.LabelLoader.Migrations 
{
    public class LabelContext : DbContext
    {
        private readonly IConfiguration _Configuration;

        public LabelContext(DbContextOptions options) : base(options)
        {
        }

        public LabelContext(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_Configuration.GetConnectionString("LabelContextConnectionString"));
        }

        public DbSet<TabelaNutrientes> TabelaNutrientes { get; set; }


    }
}
