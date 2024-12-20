using System.ComponentModel.DataAnnotations;

namespace TrabajoFinal_Laboratorio4.Models
{
    public class TicketDetalle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Describa lo que desea incluir en el pedido")]
        public string descripcionPedido { get; set; }

        public int estadoId { get; set; }

        public Estado estado { get; set; }

        public DateTime fechaEstado { get; set; }

        public int ticketId { get; set; }

        public Ticket ticket { get; set; }
    }
}
