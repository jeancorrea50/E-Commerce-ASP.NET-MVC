using Microsoft.EntityFrameworkCore;
using Portifolio675.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Portifolio675.Data
{
    public class PortDbContext : DbContext
    {
        public DbSet<CategoriaModel> Categorias { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<ItemPedidoModel> ItensPedidos { get; set; }


        // String de conexão com o Banco de dados (SQL)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // string C com o banco de dados sql server
            optionsBuilder.UseSqlServer("Password=Bf18102907;Persist Security Info=True;User ID=jeancpcorrea;Initial Catalog=PortifolioJean;Data Source=DESKTOP-43O4B71\\SQLEXPRESS");
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // to passando que o novo das tabelas no banco de dados será "Categoria " e "Produto", caso nao passe este parametro, será criado no pural, ex "Produtos" 
            modelBuilder.Entity<CategoriaModel>().ToTable("Categoria");
            modelBuilder.Entity<ProdutoModel>().ToTable("Produto");


            modelBuilder.Entity<ClienteModel>()
                          .OwnsMany(c => c.Enderecos, e =>
                          {
                              e.WithOwner().HasForeignKey("IdUsuario");
                              e.HasKey("IdUsuario", "IdEndereco");
                          });

            modelBuilder.Entity<ProdutoModel>().Property(p => p.Quantidade)
                .HasDefaultValue(0);
            modelBuilder.Entity<PedidoModel>()
                .OwnsOne(p => p.EnderecoEntrega, e =>
                {
                    e.Ignore(e => e.IdEndereco);
                    e.Ignore(e => e.Selecionado);
                    e.ToTable("Pedido");
                });
            modelBuilder.Entity<ItemPedidoModel>()
                .HasKey(ip => new { ip.IdPedido, ip.IdProduto });


            modelBuilder.Entity<UsuarioModel>().Property(u => u.DataCadastro)
                .HasDefaultValueSql("getdate()");



        }

    }
}