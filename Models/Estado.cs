namespace TrabajoFinal_Laboratorio4.Models
{
    public class Estado
    {
        public int Id { get; set; }

        public string descripcion { get; set; }

        public List<TicketDetalle> ticketDetalles { get; set; }
    }
}
