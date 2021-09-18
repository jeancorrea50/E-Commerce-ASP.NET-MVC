using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Portifolio.Models
{
    public class Prod
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string NomeProduto { get; set; }

        public string Cor { get; set; }

      
        [Display(Name = "Ano de Fabricação")]
        public DateTime? AnoFabricacao { get; set; }
        public decimal Valor { get; set; }
        public string ProfileImage { get; set; }

        public IEnumerable<Categ> Categs { get; set; }
         public Categ Categ { get; set; }

        [Display(Name = "Data lançamento")]
        public DateTime? DataLacamento { get; set; } = DateTime.Now;

    }
}
