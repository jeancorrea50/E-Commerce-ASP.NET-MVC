using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portifolio.Data;
using Portifolio.Models;
using Portifolio.Models.ViewModels;

namespace Portifolio675.Controllers
{
    public class ProdsController : Controller
    {

                                                                  // CONEXÃO COM O BANCO DE DADOS
        private readonly PortDbContext _context;

        public ProdsController(PortDbContext context)
        {
            _context = context;
        } 
                       
        
        
                                                       // INDEX (HOME) 
        
        public async Task<IActionResult> Index()
        {
            var items = _context.Prods.ToList();
            return View(await _context.Prods.Include(x => x.Categ).ToListAsync());
        }


                                            // (DETALHES)

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prod = await _context.Prods
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prod == null)
            {
                return NotFound();
            }

            return View(prod);
        }



       
                                                                                      // GET & SET (CREATE)
        public IActionResult Create()
        {
                                                       // VIEWMODEL ( 2 TABELAS DE BANCO DE DADOS EXIBINDO NA VIEW )
            var lista = _context.Categs.Select(categorias => new SelectListItem { Value = categorias.Id.ToString(), Text = categorias.NomeCategoria }).ToList();


            return View(new ProdViewModel { Prod = new Prod(), CategSelect = lista });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomeProduto,Cor,AnoFabricacao,Valor,Imagem")] Prod prod, int CategId)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prod);
                prod.Categ = _context.Categs.Find(CategId);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prod);
        }


                                                                               // GET & SET (EDIT)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prod = await _context.Prods.FindAsync(id);
            if (prod == null)
            {
                return NotFound();
            }
            return View(prod);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeProduto,Cor,AnoFabricacao,Valor,Imagem")] Prod prod)
        {
            if (id != prod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdExists(prod.Id))
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
            return View(prod);
        }



                                                                                       // GET E SET (DELETE)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prod = await _context.Prods
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prod == null)
            {
                return NotFound();
            }

            return View(prod);
        }

  
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prod = await _context.Prods.FindAsync(id);
            _context.Prods.Remove(prod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdExists(int id)
        {
            return _context.Prods.Any(e => e.Id == id);
        }
    }
}
