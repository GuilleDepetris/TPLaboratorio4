using TrabajoFinal_Laboratorio4.Models;

namespace TrabajoFinal_Laboratorio4.ViewsModels
{
    public class TicketDetallesViewModel
    {
        public List<TicketDetalle>? TicketDetalles { get; set; }
        public int TotalDetallesTickets { get; set; }
        public int PaginaActual { get; set; }
        public int ElementosPorPagina { get; set; } = 5;
        public int TamanoPagina => (int)Math.Ceiling((double)TotalDetallesTickets / ElementosPorPagina);
    }
}
