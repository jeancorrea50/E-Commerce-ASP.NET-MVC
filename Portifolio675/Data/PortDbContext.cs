using Microsoft.EntityFrameworkCore;
using Portifolio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Portifolio.Data
{
    public class PortDbContext : DbContext
    {
        public DbSet<Categ> Categs { get; set; }
        public DbSet<Prod> Prods { get; set; }
        


        // String de conexão com o Banco de dados (SQL)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Password=Bf18102907;Persist Security Info=True;User ID=jeancpcorrea;Initial Catalog=Portifolio495;Data Source=DESKTOP-43O4B71\\SQLEXPRESS");
         
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            base.OnModelCreating(builder);
            builder.Entity<Categ>().HasMany(categoria => categoria.Prods).WithOne(produtos => produtos.Categ);
            builder.Entity<Prod>().HasOne(produto => produto.Categ).WithMany(categoria => categoria.Prods);
        }

    }
   
    }