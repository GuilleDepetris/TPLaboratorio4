using TrabajoFinal_Laboratorio4.Models;

namespace TrabajoFinal_Laboratorio4.ViewsModels
{
    public class TicketsViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public int TotalTickets { get; set; }
        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; } = 5;
        public int? AfiliadoId { get; set; }
        public string ObservacionTicket { get; set; }
    }
}
