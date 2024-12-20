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
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(int? afiliadoId, string observacionTicket, int paginaActual = 1)
        {
            // Obtén la lista de afiliados
            var afiliados = await _context.afiliado.ToListAsync();
            ViewBag.Afiliados = new SelectList(afiliados, "Id", "apellido");

            var appDBContextBase = _context.ticket.Include(a => a.afiliado).AsQueryable();

            // Filtrado por observación
            if (!string.IsNullOrEmpty(observacionTicket))
            {
                appDBContextBase = appDBContextBase.Where(a => a.observacion.Contains(observacionTicket));
            }
            // Filtrado por afiliado
            if (afiliadoId.HasValue)
            {
                appDBContextBase = appDBContextBase.Where(a => a.afiliadoId == afiliadoId.Value);
            }

            // Obtener el total de tickets para paginación
            var totalTickets = await appDBContextBase.CountAsync();

            // Obtener los tickets en la página actual
            var ticketsPorPagina = await appDBContextBase
                .Skip((paginaActual - 1) * 5) // Ajusta este valor según TamanoPagina si es necesario
                .Take(5)
                .ToListAsync();

            // Crear el modelo
            TicketsViewModel modelo = new TicketsViewModel()
            {
                Tickets = ticketsPorPagina,
                TotalTickets = totalTickets,
                PaginaActual = paginaActual,
                AfiliadoId = afiliadoId,
                ObservacionTicket = observacionTicket
            };

            return View(modelo);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.ticket
                .Include(t => t.afiliado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["afiliadoId"] = new SelectList(_context.afiliado, "Id", "apellido");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,afiliadoId,fechaSolicitud,observacion")] Ticket ticket)
        {
            ModelState.Remove("TicketDetalle");
            ModelState.Remove("fechaSolicitud");
            ModelState.Remove("afiliado");
            if (ModelState.IsValid)
            {
                ticket.fechaSolicitud = DateTime.Now;//Hora del momento de creacion
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ViewData["afiliadoId"] = new SelectList(_context.afiliado, "Id", "apellido", ticket.afiliadoId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["afiliadoId"] = new SelectList(_context.afiliado, "Id", "apellido", ticket.afiliadoId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,afiliadoId,fechaSolicitud,observacion")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            ModelState.Remove("TicketDetalle");
            ModelState.Remove("afiliado");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            ViewData["afiliadoId"] = new SelectList(_context.afiliado, "Id", "apellido", ticket.afiliadoId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.ticket
                .Include(t => t.afiliado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.ticket
                .Include(t => t.TicketDetalle)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (ticket != null)
            {
                if (ticket.TicketDetalle != null)
                {
                    _context.ticketDetalle.RemoveRange(ticket.TicketDetalle);
                }
                _context.ticket.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.ticket.Any(e => e.Id == id);
        }
    }
}
