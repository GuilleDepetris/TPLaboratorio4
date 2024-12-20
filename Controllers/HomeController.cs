using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TrabajoFinal_Laboratorio4.Data;
using TrabajoFinal_Laboratorio4.Models;

namespace TrabajoFinal_Laboratorio4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Cambia el ID del estado pendiente según tu implementación
            int estadoPendienteId = 1; // Por ejemplo, el ID del estado "Pendiente"

            var ticketsPendientes = await _context.ticket
                .Include(t => t.afiliado) // Incluye la información del afiliado
                .Include(t => t.TicketDetalle) // Incluye los detalles del ticket
                .Where(t => t.TicketDetalle.estadoId == estadoPendienteId)
                .OrderBy(t => t.fechaSolicitud) // Ordena por fecha de solicitud
                .ToListAsync();

            return View(ticketsPendientes);
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
