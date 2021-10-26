using System.Linq;
using System.Threading.Tasks;
using Portifolio675.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portifolio675.Data;

namespace Portifolio675.Controllers
{
    public class ItemPedidosController : Controller
    {
        private readonly PortDbContext _context;

        public ItemPedidosController(PortDbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index(int? ped)
        {
            if (ped.HasValue)
            {
                if (_context.Pedidos.Any(p => p.IdPedido == ped))
                {
                    var pedido = await _context.Pedidos
                        .Include(p => p.Cliente)
                        .Include(p => p.ItensPedido.OrderBy(i => i.Produto.Nome))
                        .ThenInclude(i => i.Produto)
                        .FirstOrDefaultAsync(p => p.IdPedido == ped);

                    ViewBag.Pedido = pedido;
                    return View(pedido.ItensPedido);
                }
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? ped, int? prod)
        {
            if (ped.HasValue)
            {
                if (_context.Pedidos.Any(p => p.IdPedido == ped))
                {
                    var produtos = _context.Produtos
                        .OrderBy(x => x.Nome)
                        .Select(p => new { p.IdProduto, NomePreco = $"{p.Nome} ({p.Valor:C})" })
                        .AsNoTracking().ToList();
                    var produtosSelectList = new SelectList(produtos, "IdProduto", "NomePreco");
                    ViewBag.Produtos = produtosSelectList;

                    if (prod.HasValue && ItemPedidoExiste(ped.Value, prod.Value))
                    {
                        var itemPedido = await _context.ItensPedidos
                            .Include(i => i.Produto)
                            .FirstOrDefaultAsync(i => i.IdPedido == ped && i.IdProduto == prod);
                        return View(itemPedido);
                    }
                    else
                    {
                        return View(new ItemPedidoModel()
                        { IdPedido = ped.Value, ValorUnitario = 0, Quantidade = 1 });
                    }
                }
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        private bool ItemPedidoExiste(int ped, int prod)
        {
            return _context.ItensPedidos.Any(x => x.IdPedido == ped && x.IdProduto == prod);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ItemPedidoModel itemPedido)
        {
            if (ModelState.IsValid)
            {
                if (itemPedido.IdPedido > 0)
                {
                    var produto = await _context.Produtos.FindAsync(itemPedido.IdProduto);
                    itemPedido.ValorUnitario = produto.Valor;
                    if (ItemPedidoExiste(itemPedido.IdPedido, itemPedido.IdProduto))
                    {
                        _context.ItensPedidos.Update(itemPedido);
                        if (await _context.SaveChangesAsync() > 0)
                            TempData["mensagem"] = MensagemModel.Serializar("Item de pedido alterado com sucesso.");
                        else
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar item de pedido.", TipoMensagem.Erro);
                    }
                    else
                    {
                        _context.ItensPedidos.Add(itemPedido);
                        if (await _context.SaveChangesAsync() > 0)
                            TempData["mensagem"] = MensagemModel.Serializar("Item de pedido cadastrado com sucesso.");
                        else
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar item de pedido.", TipoMensagem.Erro);
                    }
                    var pedido = await _context.Pedidos.FindAsync(itemPedido.IdPedido);
                    pedido.ValorTotal = _context.ItensPedidos
                        .Where(i => i.IdPedido == itemPedido.IdPedido)
                        .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { ped = itemPedido.IdPedido });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }
            }
            else
            {
                var produtos = _context.Produtos
                        .OrderBy(x => x.Nome)
                        .Select(p => new { p.IdProduto, NomePreco = $"{p.Nome} ({p.Valor:C})" })
                        .AsNoTracking().ToList();
                var produtosSelectList = new SelectList(produtos, "IdProduto", "NomePreco");
                ViewBag.Produtos = produtosSelectList;

                return View(itemPedido);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? ped, int? prod)
        {
            if (!ped.HasValue || !prod.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de pedido não informado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            if (!ItemPedidoExiste(ped.Value, prod.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var itemPedido = await _context.ItensPedidos.FindAsync(ped, prod);
            _context.Entry(itemPedido).Reference(i => i.Produto).Load();
            return View(itemPedido);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int idPedido, int idProduto)
        {
            var itemPedido = await _context.ItensPedidos.FindAsync(idPedido, idProduto);
            if (itemPedido != null)
            {
                _context.ItensPedidos.Remove(itemPedido);
                if (await _context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Item de pedido excluído com sucesso.");
                    var pedido = await _context.Pedidos.FindAsync(itemPedido.IdPedido);
                    pedido.ValorTotal = _context.ItensPedidos
                        .Where(i => i.IdPedido == itemPedido.IdPedido)
                        .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _context.SaveChangesAsync();
                }
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o item de pedido.", TipoMensagem.Erro);
                return RedirectToAction("Index", new { ped = idPedido });
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", new { ped = idPedido });
            }
        }
    }
}