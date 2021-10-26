using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Portifolio675.Models
{
    public class ProdutoModel
    {
        [Key]
        public int IdProduto { get; set; }
        public string Nome { get; set; }

        public Decimal Valor { get; set; }
        public string Memoria { get; set; }
        public string Cor { get; set; }

        public int Quantidade { get; set; }


        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [DisplayName("Nome Imagem")]

        public string ImageName { get; set; }

     [NotMapped]
     [DisplayName("Upload File")]
     public IFormFile ImageFile { get; set; }


        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }

        public CategoriaModel Categoria { get; set; }

    }


}
