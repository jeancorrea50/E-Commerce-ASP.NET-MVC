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

namespace Projeto1___Ecomerce.Controllers
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

        // GET: Produtos
        public async Task<IActionResult> Index()
        {

            // .Include(x => x.Categoria) => Ordena a lista passada para index por ordem alfabetica
            // AsNoTracking()  => não precisa ser rastreado ( não precisa verificar se foi alterado ou recuperado) cada compilação o metdo sera executado
            return View(await _context.Produtos.OrderBy(x => x.Nome).Include(x => x.Categoria).AsNoTracking().ToListAsync());
        }

        // GET: Produtos/Details/5
       

        // GET: Produtos/Create
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

    

    // POST: Produtos/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Valor,Memoria,Cor,Title,ImageFile")] ProdutoModel produto)
        {
            if (ModelState.IsValid)
            {
                // save image to wwwroot/image
                string wwwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(produto.ImageFile.FileName);
                string extension = Path.GetExtension(produto.ImageFile.FileName);
                produto.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwwRootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await produto.ImageFile.CopyToAsync(fileStream);

                } 

                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
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

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
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
