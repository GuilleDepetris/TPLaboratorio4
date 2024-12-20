using System.ComponentModel.DataAnnotations;

namespace TrabajoFinal_Laboratorio4.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public int afiliadoId { get; set; }

        public Afiliado afiliado { get; set; }

        public DateTime fechaSolicitud { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? observacion { get; set; }

        public TicketDetalle TicketDetalle { get; set; }

    }
}
