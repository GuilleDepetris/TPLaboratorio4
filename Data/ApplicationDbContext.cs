using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrabajoFinal_Laboratorio4.Models;

namespace TrabajoFinal_Laboratorio4.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Afiliado> afiliado { get; set; }
        public DbSet<Estado> estado { get; set; }
        public DbSet<Ticket> ticket { get; set; }
        public DbSet<TicketDetalle> ticketDetalle { get; set; }
    }
}
