﻿// <auto-generated />
using GeekBurger.LabelLoader.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeekBurger.LabelLoader.Migrations.Migrations
{
    [DbContext(typeof(LabelContext))]
    [Migration("20191105001304_Iniciando banco")]
    partial class Iniciandobanco
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GeekBurger.LabelLoader.Models.TabelaNutrientes", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnName("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Descricao")
                        .HasColumnName("Descricao")
                        .HasColumnType("varchar(60)");

                    b.HasKey("Id");

                    b.ToTable("Cad_Ingredientes");
                });
#pragma warning restore 612, 618
        }
    }
}
