using GeekBurger.LabelLoader.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.LabelLoader.Migrations 
{
    public class LabelContext : DbContext
    {
        

        public LabelContext(DbContextOptions options) : base(options)
        {
        }

        protected LabelContext()
        {
        }



        public DbSet<TabelaNutrientes> TabelaNutrientes { get; set; }


    }
}
