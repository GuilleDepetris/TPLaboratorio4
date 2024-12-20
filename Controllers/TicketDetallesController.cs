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
    public class TicketDetallesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketDetallesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketDetalles
        public async Task<IActionResult> Index(int paginaActual = 1)
        {
            //Aca obtuve el contexto
            var applicationDbContext = _context.ticketDetalle.Include(t => t.estado).Include(t => t.ticket).AsQueryable();
            //Aca obtuve la cantidad de Ticketsdetalles
            var totalTicketsDetalles = await applicationDbContext.CountAsync();
            var elementosPorPagina = 5;
            var ticketDetallesPorPagina = await applicationDbContext
               .Skip((paginaActual - 1) * elementosPorPagina) // Ajusta este valor según TamanoPagina si es necesario
               .Take(elementosPorPagina)
               .ToListAsync();
            // Crear el modelo
            TicketDetallesViewModel modelo = new TicketDetallesViewModel()
            {
                TicketDetalles = ticketDetallesPorPagina, 
                TotalDetallesTickets = totalTicketsDetalles,
                PaginaActual = paginaActual,
                ElementosPorPagina = elementosPorPagina
            };

            return View(modelo);
        }

        // GET: TicketDetalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var ticketDetalle = await _context.ticketDetalle
                .Include(td => td.ticket)
                .ThenInclude(t => t.afiliado) // Incluir el afiliado
                .Include(td => td.estado)
                .FirstOrDefaultAsync(td => td.Id == id);

                if (ticketDetalle == null)
                {
                    return NotFound();
                }

            // Obtener todos los tickets del afiliado
            var ticketsDelAfiliado = await _context.ticket
                .Include(t => t.afiliado) // Incluir el afiliado
                .Include(t => t.TicketDetalle) // Incluir el TicketDetalle
                .Where(t => t.afiliado.Id == ticketDetalle.ticket.afiliado.Id)
                .ToListAsync();

            ViewBag.TicketsDelAfiliado = ticketsDelAfiliado;

            return View(ticketDetalle);
        }

        // GET: TicketDetalles/Create
        public IActionResult Create()
        {
            var ticketIdsConDetalles = _context.ticketDetalle.Select(td => td.ticketId).ToList();

            // Filtra los tickets, para no mostrar los que ya tienen detalles
            var ticketsDisponibles = _context.ticket
                .Where(t => !ticketIdsConDetalles.Contains(t.Id))
                .ToList();

            // Verifica si hay tickets disponibles
            if (!ticketsDisponibles.Any())
            {
                // Si no hay tickets, añade una opción predeterminada
                var opciones = new List<SelectListItem>
{
                new SelectListItem { Value = "", Text = "No hay tickets disponibles" }};

                ViewData["ticketId"] = new SelectList(opciones, "Value", "Text");
            }
            else
            {
                ViewData["ticketId"] = new SelectList(ticketsDisponibles, "Id", "observacion");
            }
            ViewData["estadoId"] = new SelectList(_context.estado, "Id", "descripcion");
            return View();
        }

        // POST: TicketDetalles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,descripcionPedido,estadoId,fechaEstado,ticketId")] TicketDetalle ticketDetalle, int ticketId)
        {
            ModelState.Remove("fechaestado");
            ModelState.Remove("ticket");
            ModelState.Remove("estado");


            if (ModelState.IsValid)
            {
                ticketDetalle.fechaEstado = DateTime.Now;//Hora del momento de creacion

                _context.Add(ticketDetalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["estadoId"] = new SelectList(_context.estado, "Id", "descripcion", ticketDetalle.estadoId);
            ViewData["ticketId"] = new SelectList(_context.ticket, "Id", "observacion", ticketDetalle.ticketId);
            
            return View(ticketDetalle);
        }

        // GET: TicketDetalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketDetalle = await _context.ticketDetalle.FindAsync(id);
            if (ticketDetalle == null)
            {
                return NotFound();
            }

            // Obtiene los IDs de tickets que ya tienen detalles, excluyendo el ticket que se está editando
            var ticketIdsConDetalles = _context.ticketDetalle
                .Where(td => td.ticketId != ticketDetalle.ticketId)
                .Select(td => td.ticketId)
                .ToList();

            // Filtra los tickets disponibles para asegurarse de que el ticket que se está editando esté disponible
            var ticketsDisponibles = _context.ticket
                .Where(t => !ticketIdsConDetalles.Contains(t.Id) || t.Id == ticketDetalle.ticketId)
                .ToList();

            ViewData["estadoId"] = new SelectList(_context.estado, "Id", "descripcion", ticketDetalle.estadoId);
            ViewData["ticketId"] = new SelectList(ticketsDisponibles, "Id", "observacion", ticketDetalle.ticketId);

            return View(ticketDetalle);
        }

        // POST: TicketDetalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,descripcionPedido,estadoId,fechaEstado,ticketId")] TicketDetalle ticketDetalle)
        {
            if (id != ticketDetalle.Id)
            {
                return NotFound();
            }
            ModelState.Remove("estado");
            ModelState.Remove("ticket");
            ModelState.Remove("fechaEstado");


            if (ModelState.IsValid)
            {
                try
                {
                    ticketDetalle.fechaEstado = DateTime.Now;//Hora del momento de creacion
                    _context.Update(ticketDetalle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketDetalleExists(ticketDetalle.Id))
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

            ViewData["estadoId"] = new SelectList(_context.estado, "Id", "descripcion", ticketDetalle.estadoId);
            ViewData["ticketId"] = new SelectList(_context.ticket, "Id", "observacion", ticketDetalle.ticketId);

            return View(ticketDetalle);
        }

        // GET: TicketDetalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketDetalle = await _context.ticketDetalle
                .Include(t => t.estado)
                .Include(t => t.ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketDetalle == null)
            {
                return NotFound();
            }

            return View(ticketDetalle);
        }

        // POST: TicketDetalles/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketDetalle = await _context.ticketDetalle.FindAsync(id);
            if (ticketDetalle != null)
            {
                _context.ticketDetalle.Remove(ticketDetalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketDetalleExists(int id)
        {
            return _context.ticketDetalle.Any(e => e.Id == id);
        }
    }
}
