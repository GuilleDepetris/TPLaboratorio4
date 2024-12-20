using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;

namespace TrabajoFinal_Laboratorio4.Models
{
    public class Afiliado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string nombres { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        public int dni { get; set; }

        [Required(ErrorMessage = "La necha de nacimiento es obligatoria.")]
        public DateTime fechaNacimiento { get; set; }

        public string foto { get; set; }

        public List<Ticket>? tickets { get; set; }

    }
}
