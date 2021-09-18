using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portifolio.Data;
using Portifolio.Models;

namespace Portifolio675.Controllers
{
    public class CategsController : Controller
    {
        private readonly PortDbContext _context;

        public CategsController(PortDbContext context)
        {
            _context = context;
        }

        // GET: Categs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categs.ToListAsync());
        }

        // GET: Categs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categ = await _context.Categs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categ == null)
            {
                return NotFound();
            }

            return View(categ);
        }

        // GET: Categs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomeCategoria,TipoProduto,Modelo")] Categ categ)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categ);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categ);
        }

        // GET: Categs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categ = await _context.Categs.FindAsync(id);
            if (categ == null)
            {
                return NotFound();
            }
            return View(categ);
        }

        // POST: Categs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeCategoria,TipoProduto,Modelo")] Categ categ)
        {
            if (id != categ.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categ);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategExists(categ.Id))
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
            return View(categ);
        }

        // GET: Categs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categ = await _context.Categs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categ == null)
            {
                return NotFound();
            }

            return View(categ);
        }

        // POST: Categs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categ = await _context.Categs.FindAsync(id);
            _context.Categs.Remove(categ);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategExists(int id)
        {
            return _context.Categs.Any(e => e.Id == id);
        }
    }
}
