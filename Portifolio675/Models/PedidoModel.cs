using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portifolio675.Models
{
    [Table("Pedido")]
    public class PedidoModel
    {
        [Key]
        public int IdPedido { get; set; }

        public DateTime? DataPedido { get; set; }

        public DateTime? DataEntrega { get; set; }

        public decimal? ValorTotal { get; set; }

        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public ClienteModel Cliente { get; set; }

        public EnderecoModel EnderecoEntrega { get; set; }

        public ICollection<ItemPedidoModel> ItensPedido { get; set; }
    }
}