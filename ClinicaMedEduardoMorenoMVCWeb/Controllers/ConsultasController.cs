using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels;
using ClinicaMedEduardoMorenoMVCWeb.Data;

namespace ClinicaMedEduardoMorenoMVCWeb.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly AppDbContext _context;

        // Valores permitidos por el CHECK constraint CK_Consultas_Estado de la base de datos
        private static readonly string[] EstadosConsulta = { "pendiente", "atendida", "cancelada" };

        public ConsultasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pagina = 1, string? buscar = null, string? filtro = null)
        {
            if (pagina < 1) pagina = 1;
            int tamañoPagina = 10;

            // Consultas con el nombre del paciente para poder buscar por él
            var query = from c in _context.Consultas
                        join e in _context.Expedientes on c.ExpedienteId equals e.ExpedienteId
                        join p in _context.Pacientes on e.PacienteId equals p.PacienteId
                        select new { Consulta = c, Paciente = p.Nombre };

            // Filtro por estado del día: "pendientes-hoy", "atendidas-hoy" o historial completo
            var hoy = DateTime.Today;
            if (filtro == "pendientes-hoy")
            {
                query = query.Where(x => x.Consulta.Fecha == hoy && x.Consulta.Estado == "pendiente");
            }
            else if (filtro == "atendidas-hoy")
            {
                query = query.Where(x => x.Consulta.Fecha == hoy && x.Consulta.Estado == "atendida");
            }
            else
            {
                filtro = "todas";
            }

            // Búsqueda por código, paciente, motivo o médico
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();
                query = query.Where(x => x.Consulta.Codigo.Contains(buscar)
                                      || x.Paciente.Contains(buscar)
                                      || x.Consulta.Motivo.Contains(buscar)
                                      || (x.Consulta.Medico != null && x.Consulta.Medico.Contains(buscar)));
            }

            var totalRegistros = query.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamañoPagina);
            if (totalPaginas == 0) totalPaginas = 1;

            if (pagina > totalPaginas) pagina = totalPaginas;

            var resultados = query
                .OrderByDescending(x => x.Consulta.Fecha)
                .ThenByDescending(x => x.Consulta.ConsultaId)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            var consultas = resultados.Select(x => x.Consulta).ToList();

            // Nombre del paciente por expediente, solo de las consultas de esta página
            ViewBag.Pacientes = resultados
                .GroupBy(x => x.Consulta.ExpedienteId)
                .ToDictionary(g => g.Key, g => g.First().Paciente);

            // Al buscar un paciente, se cargan sus consultas anteriores a cada consulta de la página
            var anteriores = new Dictionary<int, List<Consultas>>();
            if (!string.IsNullOrWhiteSpace(buscar) && consultas.Any())
            {
                var expedienteIds = consultas.Select(c => c.ExpedienteId).Distinct().ToList();
                var historial = _context.Consultas
                    .Where(c => expedienteIds.Contains(c.ExpedienteId))
                    .OrderByDescending(c => c.Fecha)
                    .ThenByDescending(c => c.ConsultaId)
                    .ToList();

                foreach (var consulta in consultas)
                {
                    anteriores[consulta.ConsultaId] = historial
                        .Where(h => h.ExpedienteId == consulta.ExpedienteId
                                 && (h.Fecha < consulta.Fecha || (h.Fecha == consulta.Fecha && h.ConsultaId < consulta.ConsultaId)))
                        .ToList();
                }
            }
            ViewBag.Anteriores = anteriores;

            ViewBag.Buscar = buscar;
            ViewBag.Filtro = filtro;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TamañoPagina = tamañoPagina;

            return View(consultas);
        }

        private string GenerarSiguienteCodigo()
        {
            var codigos = _context.Consultas.Select(c => c.Codigo).ToList();
            int maxNumero = 0;

            foreach (var cod in codigos)
            {
                if (string.IsNullOrWhiteSpace(cod)) continue;
                var match = System.Text.RegularExpressions.Regex.Match(cod, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int num))
                {
                    if (num > maxNumero) maxNumero = num;
                }
            }

            int siguienteNumero = maxNumero + 1;
            return $"CON-{siguienteNumero:D3}";
        }

        private void CargarListas()
        {
            ViewBag.Expedientes = (from e in _context.Expedientes
                                   join p in _context.Pacientes on e.PacienteId equals p.PacienteId
                                   orderby p.Nombre
                                   select new SelectListItem
                                   {
                                       Value = e.ExpedienteId.ToString(),
                                       Text = p.Nombre + " (" + e.Codigo + ")"
                                   }).ToList();

            ViewBag.Usuarios = _context.Usuarios
                .Where(u => u.Activo)
                .OrderBy(u => u.Nombre)
                .Select(u => new SelectListItem
                {
                    Value = u.UsuarioId.ToString(),
                    Text = u.Nombre + " - " + u.Rol
                })
                .ToList();

            ViewBag.Medicos = _context.Usuarios
                .Where(u => u.Activo && u.Rol.ToLower() == "doctor")
                .OrderBy(u => u.Nombre)
                .Select(u => u.Nombre)
                .ToList();

            ViewBag.Estados = EstadosConsulta;
        }

        private void ValidarConsulta(Consultas consulta)
        {
            if (!EstadosConsulta.Contains(consulta.Estado))
            {
                ModelState.AddModelError(nameof(consulta.Estado), "Seleccione un estado válido");
            }

            if (!_context.Expedientes.Any(e => e.ExpedienteId == consulta.ExpedienteId))
            {
                ModelState.AddModelError(nameof(consulta.ExpedienteId), "Seleccione un paciente válido");
            }

            if (!_context.Usuarios.Any(u => u.UsuarioId == consulta.UsuarioId))
            {
                ModelState.AddModelError(nameof(consulta.UsuarioId), "Seleccione un usuario válido");
            }
        }

        public IActionResult Create()
        {
            var consulta = new Consultas
            {
                Codigo = GenerarSiguienteCodigo(),
                Fecha = DateTime.Today,
                Estado = "pendiente"
            };
            CargarListas();
            return View(consulta);
        }

        [HttpPost]
        public IActionResult Create(Consultas consulta)
        {
            // Asignar el código automáticamente para garantizar consistencia y correlatividad
            consulta.Codigo = GenerarSiguienteCodigo();
            ModelState.Remove(nameof(consulta.Codigo));
            ValidarConsulta(consulta);

            if (ModelState.IsValid)
            {
                consulta.CreadoEn = DateTime.Now;
                _context.Consultas.Add(consulta);
                _context.SaveChanges();

                // Después de registrar, se abre la atención para asignar diagnósticos
                return RedirectToAction(nameof(Details), new { id = consulta.ConsultaId });
            }

            CargarListas();
            return View(consulta);
        }

        public IActionResult Details(int id)
        {
            var modelo = ObtenerDetalle(id);

            if (modelo == null)
            {
                return NotFound();
            }

            // Solo se ofrecen las enfermedades que aún no están asignadas a la consulta
            var asignadas = _context.ConsultaEnfermedades
                .Where(ce => ce.ConsultaId == id)
                .Select(ce => ce.EnfermedadId)
                .ToList();

            ViewBag.Enfermedades = _context.Enfermedades
                .Where(e => !asignadas.Contains(e.EnfermedadId))
                .OrderBy(e => e.Nombre)
                .Select(e => new SelectListItem
                {
                    Value = e.EnfermedadId.ToString(),
                    Text = e.Nombre + " (" + e.TipoEnfermedad + ")"
                })
                .ToList();

            // Solo se ofrecen los medicamentos que aún no están en la receta
            var recetados = modelo.Receta == null
                ? new List<int>()
                : _context.RecetaDetalles.Where(d => d.RecetaId == modelo.Receta.RecetaId).Select(d => d.MedicamentoId).ToList();

            ViewBag.Medicamentos = _context.Medicamentos
                .Where(m => !recetados.Contains(m.MedicamentoId))
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem
                {
                    Value = m.MedicamentoId.ToString(),
                    Text = m.Nombre + " (" + m.Codigo + ")"
                })
                .ToList();

            return View(modelo);
        }

        private static int? CalcularEdad(DateTime? fechaNac, DateTime alFecha)
        {
            if (fechaNac is not DateTime nacimiento) return null;

            int edad = alFecha.Year - nacimiento.Year;
            if (nacimiento.Date > alFecha.AddYears(-edad)) edad--;
            return edad;
        }

        private ConsultaDetalleViewModel? ObtenerDetalle(int id)
        {
            var consulta = _context.Consultas.FirstOrDefault(c => c.ConsultaId == id);

            if (consulta == null)
            {
                return null;
            }

            var paciente = (from e in _context.Expedientes
                            join p in _context.Pacientes on e.PacienteId equals p.PacienteId
                            where e.ExpedienteId == consulta.ExpedienteId
                            select new { ExpedienteCodigo = e.Codigo, p.Codigo, p.Nombre, p.FechaNac, p.Sexo })
                           .FirstOrDefault();

            // Edad del paciente a la fecha de la consulta
            var edad = CalcularEdad(paciente?.FechaNac, consulta.Fecha);

            var usuario = _context.Usuarios
                .Where(u => u.UsuarioId == consulta.UsuarioId)
                .Select(u => u.Nombre)
                .FirstOrDefault();

            var enfermedades = (from ce in _context.ConsultaEnfermedades
                                join e in _context.Enfermedades on ce.EnfermedadId equals e.EnfermedadId
                                where ce.ConsultaId == id
                                orderby ce.ConsultaEnfermedadId
                                select new ConsultaEnfermedadItem
                                {
                                    ConsultaEnfermedadId = ce.ConsultaEnfermedadId,
                                    CodigoEnfermedad = e.Codigo,
                                    NombreEnfermedad = e.Nombre,
                                    TipoEnfermedad = e.TipoEnfermedad,
                                    Observaciones = ce.Observaciones
                                }).ToList();

            // Receta de la consulta con sus medicamentos
            var receta = _context.Recetas
                .Where(r => r.ConsultaId == id)
                .OrderBy(r => r.RecetaId)
                .FirstOrDefault();

            var medicamentos = receta == null
                ? new List<RecetaMedicamentoItem>()
                : (from d in _context.RecetaDetalles
                   join m in _context.Medicamentos on d.MedicamentoId equals m.MedicamentoId
                   where d.RecetaId == receta.RecetaId
                   orderby d.Orden, d.RecetaDetalleId
                   select new RecetaMedicamentoItem
                   {
                       RecetaDetalleId = d.RecetaDetalleId,
                       Orden = d.Orden,
                       CodigoMedicamento = m.Codigo,
                       NombreMedicamento = m.Nombre,
                       Prescripcion = d.Prescripcion,
                       Cantidad = d.Cantidad
                   }).ToList();

            return new ConsultaDetalleViewModel
            {
                Consulta = consulta,
                NombrePaciente = paciente?.Nombre ?? "—",
                CodigoPaciente = paciente?.Codigo ?? string.Empty,
                CodigoExpediente = paciente?.ExpedienteCodigo ?? string.Empty,
                EdadPaciente = edad,
                SexoPaciente = paciente?.Sexo switch { "M" => "Masculino", "F" => "Femenino", _ => paciente?.Sexo ?? string.Empty },
                RegistradoPor = usuario ?? "—",
                Enfermedades = enfermedades,
                Receta = receta,
                Medicamentos = medicamentos
            };
        }

        [HttpPost]
        public IActionResult AgregarMedicamento(int consultaId, int medicamentoId, string? prescripcion, string? cantidad)
        {
            var consulta = _context.Consultas.Find(consultaId);
            if (consulta == null)
            {
                return NotFound();
            }

            prescripcion = prescripcion?.Trim();
            cantidad = cantidad?.Trim();

            if (!_context.Medicamentos.Any(m => m.MedicamentoId == medicamentoId))
            {
                TempData["ErrorReceta"] = "Seleccione un medicamento válido.";
            }
            else if (string.IsNullOrEmpty(prescripcion) || prescripcion.Length > 200)
            {
                TempData["ErrorReceta"] = "La prescripción es obligatoria (máximo 200 caracteres).";
            }
            else if (string.IsNullOrEmpty(cantidad) || cantidad.Length > 100)
            {
                TempData["ErrorReceta"] = "La cantidad es obligatoria (máximo 100 caracteres).";
            }
            else
            {
                // Una receta por consulta: se crea con el primer medicamento (código REC-<código de consulta>)
                var receta = _context.Recetas.Where(r => r.ConsultaId == consultaId).OrderBy(r => r.RecetaId).FirstOrDefault();
                if (receta == null)
                {
                    receta = new Recetas
                    {
                        Codigo = GenerarCodigoReceta(consulta.Codigo),
                        ConsultaId = consultaId,
                        Fecha = DateTime.Today,
                        Medico = consulta.Medico
                    };
                    _context.Recetas.Add(receta);
                    _context.SaveChanges();
                }

                if (_context.RecetaDetalles.Any(d => d.RecetaId == receta.RecetaId && d.MedicamentoId == medicamentoId))
                {
                    TempData["ErrorReceta"] = "Ese medicamento ya está en la receta.";
                }
                else
                {
                    var ultimoOrden = _context.RecetaDetalles
                        .Where(d => d.RecetaId == receta.RecetaId)
                        .Select(d => (int?)d.Orden)
                        .Max() ?? 0;

                    _context.RecetaDetalles.Add(new RecetaDetalle
                    {
                        RecetaId = receta.RecetaId,
                        MedicamentoId = medicamentoId,
                        Orden = (byte)Math.Min(ultimoOrden + 1, byte.MaxValue),
                        Prescripcion = prescripcion,
                        Cantidad = cantidad
                    });
                    _context.SaveChanges();
                }
            }

            return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#receta");
        }

        private string GenerarCodigoReceta(string codigoConsulta)
        {
            var codigo = $"REC-{codigoConsulta}";
            int sufijo = 2;
            while (_context.Recetas.Any(r => r.Codigo == codigo))
            {
                codigo = $"REC-{codigoConsulta}-{sufijo++}";
            }
            return codigo;
        }

        [HttpPost]
        public IActionResult QuitarMedicamento(int id)
        {
            var detalle = _context.RecetaDetalles.Find(id);
            if (detalle == null)
            {
                return NotFound();
            }

            var consultaId = _context.Recetas
                .Where(r => r.RecetaId == detalle.RecetaId)
                .Select(r => r.ConsultaId)
                .FirstOrDefault();

            _context.RecetaDetalles.Remove(detalle);
            _context.SaveChanges();

            return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#receta");
        }

        public IActionResult Expediente(int id, int? consultaId)
        {
            // Algunas columnas del expediente aceptan NULL en la BD, por eso se leen con SQL de solo lectura
            var expediente = _context.Database
                .SqlQuery<ExpedienteDatos>($"SELECT Codigo, PacienteId, Historial, CAST(FechaCreacion AS datetime2) AS FechaCreacion FROM Expedientes WHERE ExpedienteId = {id}")
                .AsEnumerable()
                .FirstOrDefault();

            if (expediente == null)
            {
                return NotFound();
            }

            var paciente = _context.Pacientes.AsNoTracking().FirstOrDefault(p => p.PacienteId == expediente.PacienteId) ?? new Pacientes();

            var alergias = _context.Database.SqlQuery<ExpedienteAlergiaItem>(
                $@"SELECT a.Nombre AS Alergia, ea.Nivel, ea.Observaciones
                   FROM ExpedienteAlergias ea JOIN Alergias a ON a.AlergiaId = ea.AlergiaId
                   WHERE ea.ExpedienteId = {id}").ToList();

            var enfermedades = _context.Database.SqlQuery<ExpedienteEnfermedadItem>(
                $@"SELECT e.Nombre AS Enfermedad, e.TipoEnfermedad, CAST(ee.FechaDeteccion AS datetime2) AS FechaDeteccion, ee.Observaciones
                   FROM ExpedienteEnfermedades ee JOIN Enfermedades e ON e.EnfermedadId = ee.EnfermedadId
                   WHERE ee.ExpedienteId = {id}").ToList();

            var antecedentes = _context.Database.SqlQuery<AntecedenteItem>(
                $@"SELECT Tipo, Descripcion, CAST(Fecha AS datetime2) AS Fecha, Observaciones
                   FROM Antecedentes WHERE ExpedienteId = {id}").ToList()
                .OrderByDescending(a => a.Fecha)
                .ToList();

            // Historial de consultas con sus diagnósticos y medicamentos recetados
            var consultas = _context.Consultas
                .Where(c => c.ExpedienteId == id)
                .OrderByDescending(c => c.Fecha)
                .ThenByDescending(c => c.ConsultaId)
                .ToList();
            var consultaIds = consultas.Select(c => c.ConsultaId).ToList();

            var diagnosticos = (from ce in _context.ConsultaEnfermedades
                                join e in _context.Enfermedades on ce.EnfermedadId equals e.EnfermedadId
                                where consultaIds.Contains(ce.ConsultaId)
                                select new { ce.ConsultaId, e.Nombre }).ToList();

            var recetados = (from r in _context.Recetas
                             join d in _context.RecetaDetalles on r.RecetaId equals d.RecetaId
                             join m in _context.Medicamentos on d.MedicamentoId equals m.MedicamentoId
                             where consultaIds.Contains(r.ConsultaId)
                             orderby d.Orden
                             select new { r.ConsultaId, m.Nombre }).ToList();

            var modelo = new ExpedienteCompletoViewModel
            {
                ExpedienteId = id,
                CodigoExpediente = expediente.Codigo,
                FechaCreacion = expediente.FechaCreacion,
                Historial = expediente.Historial,
                ConsultaOrigenId = consultaId,
                Paciente = paciente,
                Edad = CalcularEdad(paciente.FechaNac, DateTime.Today),
                Alergias = alergias,
                Enfermedades = enfermedades,
                Antecedentes = antecedentes,
                Consultas = consultas.Select(c => new HistorialConsultaItem
                {
                    ConsultaId = c.ConsultaId,
                    Codigo = c.Codigo,
                    Fecha = c.Fecha,
                    Motivo = c.Motivo,
                    Estado = c.Estado,
                    Medico = c.Medico,
                    Diagnosticos = diagnosticos.Where(d => d.ConsultaId == c.ConsultaId).Select(d => d.Nombre).ToList(),
                    Medicamentos = recetados.Where(r => r.ConsultaId == c.ConsultaId).Select(r => r.Nombre).ToList()
                }).ToList()
            };

            return View(modelo);
        }

        public IActionResult Imprimir(int id)
        {
            var modelo = ObtenerDetalle(id);

            if (modelo == null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        public IActionResult Edit(int id)
        {
            var consulta = _context.Consultas.Find(id);

            if (consulta == null)
            {
                return NotFound();
            }

            CargarListas();
            return View(consulta);
        }

        [HttpPost]
        public IActionResult Edit(int id, Consultas consulta)
        {
            if (id != consulta.ConsultaId)
            {
                return NotFound();
            }

            var consultaExistente = _context.Consultas.AsNoTracking().FirstOrDefault(c => c.ConsultaId == id);
            if (consultaExistente == null)
            {
                return NotFound();
            }

            // El código y la fecha de creación son fijos y no deben modificarse
            consulta.Codigo = consultaExistente.Codigo;
            consulta.CreadoEn = consultaExistente.CreadoEn;
            ModelState.Remove(nameof(consulta.Codigo));
            ValidarConsulta(consulta);

            if (ModelState.IsValid)
            {
                _context.Update(consulta);
                _context.SaveChanges();

                return RedirectToAction(nameof(Details), new { id = consulta.ConsultaId });
            }

            CargarListas();
            return View(consulta);
        }

        [HttpPost]
        public IActionResult AsignarEnfermedad(int consultaId, int enfermedadId, string? observaciones)
        {
            if (!_context.Consultas.Any(c => c.ConsultaId == consultaId))
            {
                return NotFound();
            }

            if (!_context.Enfermedades.Any(e => e.EnfermedadId == enfermedadId))
            {
                TempData["ErrorEnfermedad"] = "Seleccione una enfermedad válida.";
            }
            // La BD no permite repetir la misma enfermedad en una consulta (UQ_ConsultaEnfermedades)
            else if (_context.ConsultaEnfermedades.Any(ce => ce.ConsultaId == consultaId && ce.EnfermedadId == enfermedadId))
            {
                TempData["ErrorEnfermedad"] = "Esa enfermedad ya está asignada a la consulta.";
            }
            else
            {
                _context.ConsultaEnfermedades.Add(new ConsultaEnfermedades
                {
                    ConsultaId = consultaId,
                    EnfermedadId = enfermedadId,
                    Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
                });
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Details), new { id = consultaId });
        }

        [HttpPost]
        public IActionResult QuitarEnfermedad(int id)
        {
            var registro = _context.ConsultaEnfermedades.Find(id);

            if (registro == null)
            {
                return NotFound();
            }

            _context.ConsultaEnfermedades.Remove(registro);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = registro.ConsultaId });
        }

        public IActionResult Delete(int id)
        {
            var modelo = ObtenerDetalle(id);

            if (modelo == null)
            {
                return NotFound();
            }

            ViewBag.TieneRecetas = _context.Recetas.Any(r => r.ConsultaId == id);
            return View(modelo);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var consulta = _context.Consultas.Find(id);

            if (consulta != null)
            {
                // Las recetas no se borran en cascada (FK_Recetas_Consultas), así que se bloquea la eliminación
                if (_context.Recetas.Any(r => r.ConsultaId == id))
                {
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Los diagnósticos asignados se eliminan en cascada (FK_ConsultaEnf_Consultas)
                _context.Consultas.Remove(consulta);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
