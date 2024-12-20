using TrabajoFinal_Laboratorio4.Models;

namespace TrabajoFinal_Laboratorio4.ViewsModels
{
    public class AfiliadosViewModel
    {
        public List<Afiliado> Afiliados { get; set; }

        public string Nombres { get; set; }

        public string Apellido { get; set; }

        public int? Dni { get; set; }


        // Paginación
        public int TotalAfiliados { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalAfiliados / TamanoPagina);
        public int TamanoPagina { get; set; } = 5; 

    }
}
