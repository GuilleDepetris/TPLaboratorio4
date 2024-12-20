using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabajoFinal_Laboratorio4.Data;
using TrabajoFinal_Laboratorio4.Models;
using TrabajoFinal_Laboratorio4.ViewsModels;

namespace TrabajoFinal_Laboratorio4.Controllers
{
    public class EstadosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Estados
        public async Task<IActionResult> Index(int paginaActual = 1)
        {
            //Aca obtuve el contexto
            var appDBContextBase = _context.estado.AsQueryable();
            //Aca obtuve la cantidad de Estados
            var totalEstados = await appDBContextBase.CountAsync();

            var estadosPorPagina = await appDBContextBase
               .Skip((paginaActual - 1) * 3) // Ajusta este valor según TamanoPagina si es necesario
               .Take(3)
               .ToListAsync();
            // Crear el modelo
            EstadosViewModel modelo = new EstadosViewModel()
            {
                Estados = estadosPorPagina,
                TotalEstados = totalEstados,
                PaginaActual = paginaActual
            };

            return View(modelo);

            //return View(await _context.estado.ToListAsync());
        }

        // GET: Estados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = await _context.estado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estado == null)
            {
                return NotFound();
            }

            return View(estado);
        }

        // GET: Estados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,descripcion")] Estado estado)
        {
            ModelState.Remove("ticketDetalles");
            if (ModelState.IsValid)
            {
                _context.Add(estado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // GET: Estados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = await _context.estado.FindAsync(id);
            if (estado == null)
            {
                return NotFound();
            }
            return View(estado);
        }

        // POST: Estados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,descripcion")] Estado estado)
        {
            if (id != estado.Id)
            {
                return NotFound();
            }
            ModelState.Remove("ticketDetalles");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadoExists(estado.Id))
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
            return View(estado);
        }

        // GET: Estados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = await _context.estado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estado == null)
            {
                return NotFound();
            }

            return View(estado);
        }

        // POST: Estados/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estadoEnUso = _context.ticketDetalle.Any(td => td.estadoId == id);
            if (estadoEnUso)
            {
                ModelState.AddModelError("", "No se puede eliminar este estado porque está asociado a uno o más tickets.");
                return RedirectToAction(nameof(Index));
            }

            var estado = await _context.estado.FindAsync(id);
            if (estado != null)
            {
                _context.estado.Remove(estado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadoExists(int id)
        {
            return _context.estado.Any(e => e.Id == id);
        }
    }
}
