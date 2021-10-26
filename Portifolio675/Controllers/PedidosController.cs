using System;
using System.Linq;
using System.Threading.Tasks;
using Portifolio675.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portifolio675.Data;

namespace Portifolio675.Controllers
{
    public class PedidosController : Controller
    {
        private readonly PortDbContext _context;

        public PedidosController(PortDbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index(int? cid)
        {
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                if (cliente != null)
                {
                    var pedidos = await _context.Pedidos
                        .Where(p => p.IdCliente == cid)
                        .OrderByDescending(x => x.IdPedido)
                        .AsNoTracking().ToListAsync();

                    ViewBag.Cliente = cliente;
                    return View(pedidos);
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado",
                        TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado",
                    TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? cid)
        {
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                if (cliente != null)
                {
                    _context.Entry(cliente).Collection(c => c.Pedidos).Load();
                    PedidoModel pedido = null;
                    if (_context.Pedidos.Any(p => p.IdCliente == cid && !p.DataPedido.HasValue))
                    {
                        pedido = await _context.Pedidos
                            .FirstOrDefaultAsync(p => p.IdCliente == cid && !p.DataPedido.HasValue);
                    }
                    else
                    {
                        pedido = new PedidoModel { IdCliente = cid.Value, ValorTotal = 0 };
                        cliente.Pedidos.Add(pedido);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "ItemPedido", new { ped = pedido.IdPedido });
                }
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        private bool PedidoExiste(int id)
        {
            return _context.Pedidos.Any(x => x.IdPedido == id);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(int? id, [FromForm] PedidoModel pedido)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (PedidoExiste(id.Value))
                    {
                        _context.Pedidos.Update(pedido);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Pedido alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar pedido.", TipoMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrada.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    _context.Pedidos.Add(pedido);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido cadastrada com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar pedido.", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(pedido);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
                return RedirectToAction("Index");
            }

            if (!PedidoExiste(id.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Pedido excluído com sucesso.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o pedido.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index), new { cid = pedido.IdCliente });
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Fechar(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
                return RedirectToAction("Index");
            }

            if (!PedidoExiste(id.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Fechar(int id)
        {
            if (PedidoExiste(id))
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.ItensPedido)
                    .ThenInclude(i => i.Produto)
                    .FirstOrDefaultAsync(p => p.IdPedido == id);

                if (pedido.ItensPedido.Count() > 0)
                {
                    pedido.DataPedido = DateTime.Now;
                    foreach (var item in pedido.ItensPedido)
                        item.Produto.Quantidade -= item.Quantidade;
                    if (await _context.SaveChangesAsync() > 0)
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido fechado com sucesso.");
                    else
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível fechar o pedido.", TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = pedido.IdCliente });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não é possível fechar um pedido sem itens.", TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = pedido.IdCliente });
                }
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Entregar(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
                return RedirectToAction("Index");
            }

            if (!PedidoExiste(id.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .ThenInclude(c => c.Enderecos)
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Entregar(int idPedido, int idEndereco)
        {
            if (PedidoExiste(idPedido))
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .ThenInclude(c => c.Enderecos)
                    .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

                var endereco = pedido.Cliente.Enderecos
                    .FirstOrDefault(e => e.IdEndereco == idEndereco);

                if (endereco != null)
                {
                    pedido.EnderecoEntrega = endereco;
                    pedido.DataEntrega = DateTime.Now;
                    if (await _context.SaveChangesAsync() > 0)
                        TempData["mensagem"] = MensagemModel.Serializar("Entrega registrada com sucesso.");
                    else
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível registrar a entrega do pedido.", TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = pedido.IdCliente });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado.", TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = pedido.IdCliente });
                }
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
        }
    }
}