using TrabajoFinal_Laboratorio4.Models;

namespace TrabajoFinal_Laboratorio4.ViewsModels
{
    public class EstadosViewModel
    {
        public List<Estado> Estados { get; set; }
        public int TotalEstados { get; set; }
        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; } = 3;
    }
}
