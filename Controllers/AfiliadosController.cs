using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabajoFinal_Laboratorio4.Data;
using TrabajoFinal_Laboratorio4.Models;
using TrabajoFinal_Laboratorio4.ViewsModels;

namespace TrabajoFinal_Laboratorio4.Controllers
{
    public class AfiliadosController : Controller
    {
        private readonly ApplicationDbContext _context;//contexto de la base de datos, actua como puente entre la Db y la app.
        private readonly IWebHostEnvironment env; //Permite manejar rutas y configuraciones del entorno web.


        public AfiliadosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        // GET: Afiliados
        public async Task<IActionResult> Index(string busquedaNombre, string? busquedaApellido, int? busquedaDni, int paginaActual = 1)
        {
            
            var appDBContextBase = _context.afiliado.AsQueryable();//Sirve para los filtros

            if (!string.IsNullOrEmpty(busquedaNombre))
            {
                appDBContextBase = appDBContextBase.Where(a => a.nombres.Contains(busquedaNombre));
            }
            if (!string.IsNullOrEmpty(busquedaApellido))//Si no es vacio, hace la busqueda de nombre
            {
                appDBContextBase = appDBContextBase.Where(a => a.apellido.ToString().Contains(busquedaApellido.ToString()));

            }
            if (busquedaDni.HasValue)//Verifica que tenga valor
            {
                appDBContextBase = appDBContextBase.Where(a => a.dni.ToString().Contains(busquedaDni.Value.ToString()));
            }

            var totalAfiliados = await appDBContextBase.CountAsync();

            // Crear el modelo
            AfiliadosViewModel modelo = new AfiliadosViewModel()
            {
                Nombres = busquedaNombre,
                Apellido = busquedaApellido,
                Dni = busquedaDni,
                TamanoPagina = 5,
                PaginaActual = paginaActual
            };

            var afiliadosPorPagina = await appDBContextBase
                .Skip((paginaActual - 1) * modelo.TamanoPagina)
                .Take(modelo.TamanoPagina)
                .ToListAsync();

            // Asignar los afiliados y el total a modelo
            modelo.Afiliados = afiliadosPorPagina;
            modelo.TotalAfiliados = totalAfiliados;

            return View(modelo);

            //return View(modelo);
            //return View(await appDBContextBase.ToListAsync());
        }

        public async Task<IActionResult> Importar()
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivo = archivos[0];
                if (archivo.Length > 0)
                {
                    var pathDestino = Path.Combine(env.WebRootPath, "importaciones");//Ruta donde se va a guardar la foto
                    var archivoDestino = Guid.NewGuid().ToString();//Crea un nombre unico para identificar la foto y lo convierte a string
                    archivoDestino = archivoDestino.Replace("-", "");//Reemplaza los guiones
                    archivoDestino += Path.GetExtension(archivo.FileName);//Extrae la extension
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino); //Combina el directorio de destino (pathDestino),
                                                                                 //con el nombre de archivo único (archivoDestino) para crear una ruta completa donde se guardará el archivo
                    using (var filestream = new FileStream(rutaDestino, FileMode.Create))//Abrirá (o creará si no existe) el archivo en la ruta especificada (rutaDestino).
                    {
                        archivo.CopyTo(filestream);//Guarda el archivo en la carpeta correspondiente
                       
                    }
                    using (var file = new FileStream(rutaDestino, FileMode.Open))
                    {
                        List<string> renglones = new List<string>();
                        List<Afiliado> AfiliadosArch = new List<Afiliado>();

                        StreamReader fileContent = new StreamReader(file, System.Text.Encoding.Default);
                        do
                        {
                            renglones.Add(fileContent.ReadLine());
                        }
                        while (!fileContent.EndOfStream);

                        if (renglones.Count > 0)
                        {
                            foreach (var renglon in renglones)
                            {
                                string[] data = renglon.Split(';');
                                if (data.Length == 4)
                                {
                                    Afiliado afiliado = new Afiliado();
                                    afiliado.nombres = data[0].Trim();
                                    afiliado.apellido = data[1].Trim();
                                    afiliado.dni = int.Parse(data[2].Trim());
                                    afiliado.fechaNacimiento = DateTime.Parse(data[3].Trim());
                                    afiliado.foto = "";
                                    AfiliadosArch.Add(afiliado);
                                }
                            }
                            if (AfiliadosArch.Count > 0)
                            {
                                _context.AddRange(AfiliadosArch);//Es mas de un afiliado para agregar, entonces usanmos AddRange
                                await _context.SaveChangesAsync();
                            }
                        }
                    }


                }
            }
            // Calcular la paginación
            int paginaActual = 1; // Siempre inicializas desde la página 1 después de importar
            int tamanoPagina = 5; // Tamaño de página predeterminado
            var totalAfiliados = await _context.afiliado.CountAsync();

            var afiliadosPaginados = await _context.afiliado
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            var afiliadosViewModel = new AfiliadosViewModel
            {
                Afiliados = afiliadosPaginados,
                TotalAfiliados = totalAfiliados,
                PaginaActual = paginaActual,
                TamanoPagina = tamanoPagina
            };

            return View("Index", afiliadosViewModel);
            //return View("Index",await _context.afiliado.ToListAsync());
        }

            // GET: Afiliados/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var afiliado = await _context.afiliado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (afiliado == null)
            {
                return NotFound();
            }

            return View(afiliado);
        }

        // GET: Afiliados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Afiliados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombres,apellido,dni,fechaNacimiento")] Afiliado afiliado)
        {
            ModelState.Remove("foto");
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if(archivos != null && archivos.Count > 0)
                {
                    var archivoFoto = archivos[0];
                    if(archivoFoto.Length > 0)
                    {
                        var pathDestino = Path.Combine(env.WebRootPath, "imagenes\\afiliados");//Ruta donde se va a guardar la foto
                        var archivoDestino = Guid.NewGuid().ToString();//Crea un nombre unico para identificar la foto y lo convierte a string
                        archivoDestino = archivoDestino.Replace("-", "");//Reemplaza los guiones
                        archivoDestino += Path.GetExtension(archivoFoto.FileName);//Extrae la extension
                        var rutaDestino = Path.Combine(pathDestino, archivoDestino); //Combina el directorio de destino (pathDestino),
                        //con el nombre de archivo único (archivoDestino) para crear una ruta completa donde se guardará el archivo
                        using (var filestream = new  FileStream(rutaDestino, FileMode.Create))//Abrirá (o creará si no existe) el archivo en la ruta especificada (rutaDestino).
                        {
                            archivoFoto.CopyTo(filestream);//Guarda la foto en la carpeta imagenes\\afiliados
                            afiliado.foto = archivoDestino;//Guarda el nombre del archivo en la propiedad foto de afiliado
                        }

                    }
                    else
                    {
                        afiliado.foto = ""; // Asigna un string vacío si no hay foto
                    }
                }
                else
                {
                    afiliado.foto = ""; // Asigna un string vacío si no hay archivos en la solicitud
                }
                _context.Add(afiliado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(afiliado);
        }

        // GET: Afiliados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var afiliado = await _context.afiliado.FindAsync(id);
            if (afiliado == null)
            {
                return NotFound();
            }
            return View(afiliado);
        }

        // POST: Afiliados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombres,apellido,dni,fechaNacimiento,foto")] Afiliado afiliado)
        {
            if (id != afiliado.Id)
            {
                return NotFound();
            }
            ModelState.Remove("foto");
            if (ModelState.IsValid)
            {

                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoFoto = archivos[0];
                    if (archivoFoto.Length > 0)
                    {
                        var pathDestino = Path.Combine(env.WebRootPath, "imagenes\\afiliados");
                        //genera el nombre aleatorio y le agrega la extension
                        var archivoDestino = Guid.NewGuid().ToString();
                        archivoDestino = archivoDestino.Replace("-", "");
                        archivoDestino += Path.GetExtension(archivoFoto.FileName);
                        var rutaDestino = Path.Combine(pathDestino, archivoDestino);
                        if (!string.IsNullOrEmpty(afiliado.foto))
                        {
                            string fotoAnterior = Path.Combine(pathDestino, afiliado.foto);
                            if (System.IO.File.Exists(fotoAnterior))
                            {
                                System.IO.File.Delete(fotoAnterior);
                            }
                        }
                        using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                        {
                            archivoFoto.CopyTo(filestream);
                            afiliado.foto = archivoDestino;
                        }
                    }
                }
                else
                {
                    var afiliadoExistente = await _context.afiliado.AsNoTracking()
                                   .FirstOrDefaultAsync(a => a.Id == id);
                    if (afiliadoExistente != null)
                    {
                        afiliado.foto = afiliadoExistente.foto;
                    }
                }


                try
                {
                    _context.Update(afiliado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AfiliadoExists(afiliado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(afiliado);
        }

        // GET: Afiliados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var afiliado = await _context.afiliado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (afiliado == null)
            {
                return NotFound();
            }

            return View(afiliado);
        }

        // POST: Afiliados/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var afiliado = await _context.afiliado.FindAsync(id);
            if (afiliado != null)
            {
                _context.afiliado.Remove(afiliado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AfiliadoExists(int id)
        {
            return _context.afiliado.Any(e => e.Id == id);
        }
    }
}
