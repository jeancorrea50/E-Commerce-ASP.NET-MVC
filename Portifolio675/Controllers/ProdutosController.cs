using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portifolio675.Data;
using Portifolio675.Models;

namespace Portifolio675.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly PortDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProdutosController(PortDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

       [HttpGet]
        public async Task<IActionResult> Index()
        {

            // .Include(x => x.Categoria) => Ordena a lista passada para index por ordem alfabetica
            // AsNoTracking()  => não precisa ser rastreado ( não precisa verificar se foi alterado ou recuperado) cada compilação o metdo sera executado
            return View(await _context.Produtos.OrderBy(x => x.Nome).Include(x => x.Categoria).AsNoTracking().ToListAsync());
        }



          [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            var categorias = _context.Categorias.OrderBy(x => x.Nome).AsNoTracking().ToList();
            var categoriasSelectList = new SelectList(categorias,
                nameof(CategoriaModel.IdCategoria), nameof(CategoriaModel.Nome));
            ViewBag.Categorias = categoriasSelectList;

            if (id.HasValue)
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                {
                    return NotFound();
                }
                return View(produto);
            }
            return View(new ProdutoModel());
        }

        private bool ProdutoExiste(int id)
        {
            return _context.Produtos.Any(x => x.IdProduto == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, [FromForm] ProdutoModel produto)
        {
            string wwwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(produto.ImageFile.FileName);
            string extension = Path.GetExtension(produto.ImageFile.FileName);
            produto.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwwRootPath + "/Image/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))

                if (ModelState.IsValid)
            {
                  // upload save image to wwwroot/image
                
                {
                    await produto.ImageFile.CopyToAsync(fileStream);

                }
                if (id.HasValue)
                {
                    if (ProdutoExiste(id.Value))
                    {
                        _context.Produtos.Update(produto);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Produto alterado com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar produto.", TipoMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    _context.Produtos.Add(produto);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Produto cadastrado com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar produto.", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(produto);
            }
        }



        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Valor,Memoria,Cor,ImageName,Dados,ContentType")] ProdutoModel produto)
        {
            if (id != produto.IdProduto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.IdProduto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }
        // --------------->
       [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não informado.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.IdProduto == id);
            if (produto == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            //delete image from wwwroot/image
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", produto.ImageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Exists(imagePath);

            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Produto excluído com sucesso.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o produto.", TipoMensagem.Erro);
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TipoMensagem.Erro);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.IdProduto == id);
        }

        /// -------------------------------------------->
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.IdProduto == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }
    }
}
