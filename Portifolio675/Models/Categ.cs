using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portifolio.Models
{
    public class Categ
    {
        public int Id { get; set; }
        public string NomeCategoria { get; set; }
        public string TipoProduto { get; set; }
        public string Modelo { get; set; }

        public IEnumerable<Prod> Prods { get; set; }

    }
}
