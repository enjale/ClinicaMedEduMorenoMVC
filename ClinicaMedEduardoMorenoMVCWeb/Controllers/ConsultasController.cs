using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Models.ViewModels;
using ClinicaMedEduardoMorenoMVCWeb.Data;
using System.Security.Claims;

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

        private bool EsEnfermera()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol")
                      ?? User.FindFirst(ClaimTypes.Role)?.Value;
            return string.Equals(rol, "Enfermera", StringComparison.OrdinalIgnoreCase);
        }

        private int? ObtenerUsuarioIdActual()
        {
            var id = HttpContext.Session.GetInt32("UsuarioId");
            if (id.HasValue && id.Value > 0) return id.Value;

            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claimId, out int uId)) return uId;

            return null;
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
            ViewBag.EsEnfermera = EsEnfermera();

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

        private void CargarListas(Consultas? consulta = null)
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
            ViewBag.EsEnfermera = EsEnfermera();
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
            var usuarioActualId = ObtenerUsuarioIdActual();
            var consulta = new Consultas
            {
                Codigo = GenerarSiguienteCodigo(),
                Fecha = DateTime.Today,
                Estado = "pendiente",
                UsuarioId = usuarioActualId ?? 0
            };
            CargarListas(consulta);
            return View(consulta);
        }

        [HttpPost]
        public IActionResult Create(Consultas consulta)
        {
            // Asignar el código automáticamente para garantizar consistencia y correlatividad
            consulta.Codigo = GenerarSiguienteCodigo();
            ModelState.Remove(nameof(consulta.Codigo));

            // Si es enfermera, el estado siempre es "pendiente" y no puede crearla como atendida ni agregar medicamentos
            if (EsEnfermera())
            {
                consulta.Estado = "pendiente";
                ModelState.Remove(nameof(consulta.Estado));

                var usuarioActualId = ObtenerUsuarioIdActual();
                if ((consulta.UsuarioId == 0) && usuarioActualId.HasValue)
                {
                    consulta.UsuarioId = usuarioActualId.Value;
                    ModelState.Remove(nameof(consulta.UsuarioId));
                }
            }

            ValidarConsulta(consulta);

            if (ModelState.IsValid)
            {
                consulta.CreadoEn = DateTime.Now;
                _context.Consultas.Add(consulta);
                _context.SaveChanges();

                if (EsEnfermera())
                {
                    TempData["Mensaje"] = "Consulta registrada correctamente. Queda pendiente para la atención del médico.";
                    return RedirectToAction(nameof(Details), new { id = consulta.ConsultaId });
                }

                // Después de registrar por doctor, se abre la atención para asignar diagnósticos y recetas
                return RedirectToAction(nameof(Details), new { id = consulta.ConsultaId });
            }

            CargarListas(consulta);
            return View(consulta);
        }

        public IActionResult Details(int id)
        {
            var modelo = ObtenerDetalle(id);

            if (modelo == null)
            {
                return NotFound();
            }

            CargarDatosAtencion(modelo.Consulta);
            return View(modelo);
        }

        // Listas de la pantalla de atención: pacientes, enfermedades y medicamentos
        private void CargarDatosAtencion(Consultas consulta)
        {
            CargarListas(consulta);

            // Sugerencias para el buscador de enfermedades (nombre y tipo).
            // Las crónicas se registran en el expediente, no como diagnóstico de la consulta.
            ViewBag.Enfermedades = _context.Enfermedades
                .Where(e => e.TipoEnfermedad != "Crónica")
                .OrderBy(e => e.Nombre)
                .Select(e => new SelectListItem
                {
                    Value = e.Nombre,
                    Text = e.TipoEnfermedad
                })
                .ToList();

            // Sugerencias para el buscador de medicamentos (nombre y código)
            ViewBag.Medicamentos = _context.Medicamentos
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem
                {
                    Value = m.Nombre,
                    Text = m.Codigo
                })
                .ToList();
        }

        [HttpPost]
        public IActionResult Atender(int id, [Bind(Prefix = "Consulta")] Consultas datos, bool marcarAtendida)
        {
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso denegado: Las enfermeras no tienen permisos para atender consultas.";
                return RedirectToAction(nameof(Index));
            }

            var consulta = _context.Consultas.Find(id);
            if (consulta == null)
            {
                return NotFound();
            }

            // Solo se validan los campos que se editan en la pantalla de atención
            foreach (var clave in ModelState.Keys.Where(k => !k.StartsWith("Consulta.")).ToList())
            {
                ModelState.Remove(clave);
            }
            ModelState.Remove("Consulta.Codigo");
            ModelState.Remove("Consulta.UsuarioId");
            ModelState.Remove("Consulta.Estado");

            if (!_context.Expedientes.Any(e => e.ExpedienteId == datos.ExpedienteId))
            {
                ModelState.AddModelError("Consulta.ExpedienteId", "Seleccione un paciente válido");
            }

            if (!ModelState.IsValid)
            {
                var modelo = ObtenerDetalle(id)!;
                modelo.Consulta.ExpedienteId = datos.ExpedienteId;
                modelo.Consulta.Fecha = datos.Fecha;
                modelo.Consulta.Motivo = datos.Motivo;
                modelo.Consulta.Medico = datos.Medico;
                modelo.Consulta.VitalPeso = datos.VitalPeso;
                modelo.Consulta.VitalTemperatura = datos.VitalTemperatura;
                modelo.Consulta.VitalPresion = datos.VitalPresion;

                ViewBag.EsEnfermera = false;
                CargarDatosAtencion(modelo.Consulta);
                return View(nameof(Details), modelo);
            }

            // El código, el usuario que registró y la fecha de creación no cambian
            consulta.ExpedienteId = datos.ExpedienteId;
            consulta.Fecha = datos.Fecha;
            consulta.Motivo = datos.Motivo.Trim();
            consulta.Medico = string.IsNullOrWhiteSpace(datos.Medico) ? null : datos.Medico.Trim();
            consulta.VitalPeso = datos.VitalPeso;
            consulta.VitalTemperatura = datos.VitalTemperatura;
            consulta.VitalPresion = string.IsNullOrWhiteSpace(datos.VitalPresion) ? null : datos.VitalPresion.Trim();

            if (marcarAtendida)
            {
                consulta.Estado = "atendida";
            }

            _context.SaveChanges();

            TempData["Mensaje"] = $"La consulta médica '{consulta.Codigo}' ha sido actualizada exitosamente.";
            return RedirectToAction(nameof(Index));
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
                                    EnfermedadId = ce.EnfermedadId,
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
                       MedicamentoId = d.MedicamentoId,
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
        public IActionResult AgregarMedicamento(int consultaId, string? medicamento, string? prescripcion, string? cantidad)
        {
            if (EsEnfermera())
            {
                TempData["ErrorReceta"] = "Acceso restringido: Las enfermeras no pueden agregar ni recetar medicamentos.";
                return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#receta");
            }

            var consulta = _context.Consultas.Find(consultaId);
            if (consulta == null)
            {
                return NotFound();
            }

            prescripcion = prescripcion?.Trim();
            cantidad = cantidad?.Trim();

            var error = ValidarMedicamentoRecetado(medicamento, prescripcion, cantidad);
            if (error != null)
            {
                TempData["ErrorReceta"] = error;
                return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#receta");
            }

            var existente = BuscarMedicamentoPorNombre(medicamento);

            // Una receta por consulta: se crea con el primer medicamento (código REC-<código de consulta>)
            var receta = _context.Recetas.Where(r => r.ConsultaId == consultaId).OrderBy(r => r.RecetaId).FirstOrDefault();

            if (receta != null && existente != null &&
                _context.RecetaDetalles.Any(d => d.RecetaId == receta.RecetaId && d.MedicamentoId == existente.MedicamentoId))
            {
                TempData["ErrorReceta"] = "Ese medicamento ya está en la receta.";
                return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#receta");
            }

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

            var ultimoOrden = _context.RecetaDetalles
                .Where(d => d.RecetaId == receta.RecetaId)
                .Select(d => (int?)d.Orden)
                .Max() ?? 0;

            _context.RecetaDetalles.Add(new RecetaDetalle
            {
                RecetaId = receta.RecetaId,
                MedicamentoId = (existente ?? AgregarMedicamentoAlCatalogo(medicamento!)).MedicamentoId,
                Orden = (byte)Math.Min(ultimoOrden + 1, byte.MaxValue),
                Prescripcion = prescripcion!,
                Cantidad = cantidad!
            });
            _context.SaveChanges();

            return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#receta");
        }

        [HttpPost]
        public IActionResult EditarMedicamento(int id, string? medicamento, string? prescripcion, string? cantidad)
        {
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso restringido: Las enfermeras no pueden modificar recetas médicas.";
                return RedirectToAction(nameof(Index));
            }

            var detalle = _context.RecetaDetalles.Find(id);
            if (detalle == null)
            {
                return NotFound();
            }

            var consultaId = _context.Recetas
                .Where(r => r.RecetaId == detalle.RecetaId)
                .Select(r => r.ConsultaId)
                .FirstOrDefault();

            prescripcion = prescripcion?.Trim();
            cantidad = cantidad?.Trim();

            var existente = BuscarMedicamentoPorNombre(medicamento);
            var error = ValidarMedicamentoRecetado(medicamento, prescripcion, cantidad);

            if (error != null)
            {
                TempData["ErrorReceta"] = error;
            }
            else if (existente != null && _context.RecetaDetalles.Any(d => d.RecetaId == detalle.RecetaId && d.MedicamentoId == existente.MedicamentoId && d.RecetaDetalleId != id))
            {
                TempData["ErrorReceta"] = "Ese medicamento ya está en la receta.";
            }
            else
            {
                detalle.MedicamentoId = (existente ?? AgregarMedicamentoAlCatalogo(medicamento!)).MedicamentoId;
                detalle.Prescripcion = prescripcion!;
                detalle.Cantidad = cantidad!;
                _context.SaveChanges();
            }

            return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#receta");
        }

        private static string? ValidarMedicamentoRecetado(string? medicamento, string? prescripcion, string? cantidad)
        {
            if (string.IsNullOrWhiteSpace(medicamento) || medicamento.Trim().Length > 200)
                return "Debe indicar el medicamento (máximo 200 caracteres).";

            if (string.IsNullOrEmpty(prescripcion) || prescripcion.Length > 200)
                return "La prescripción es obligatoria (máximo 200 caracteres).";

            if (string.IsNullOrEmpty(cantidad) || cantidad.Length > 100)
                return "La cantidad es obligatoria (máximo 100 caracteres).";

            return null;
        }

        private Medicamentos? BuscarMedicamentoPorNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;

            nombre = nombre.Trim();
            return _context.Medicamentos.FirstOrDefault(m => m.Nombre == nombre);
        }

        // Si el medicamento escrito no existe en el catálogo, se registra con el siguiente código MED-### (igual que en el prototipo)
        private Medicamentos AgregarMedicamentoAlCatalogo(string nombre)
        {
            var codigos = _context.Medicamentos.Select(m => m.Codigo).ToList();
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

            var nuevo = new Medicamentos
            {
                Codigo = $"MED-{maxNumero + 1:D3}",
                Nombre = nombre.Trim()
            };

            _context.Medicamentos.Add(nuevo);
            _context.SaveChanges();
            return nuevo;
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
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso restringido: Las enfermeras no pueden modificar recetas médicas.";
                return RedirectToAction(nameof(Index));
            }

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
            var expediente = _context.Expedientes.AsNoTracking().FirstOrDefault(e => e.ExpedienteId == id);

            if (expediente == null)
            {
                return NotFound();
            }

            var paciente = _context.Pacientes.AsNoTracking().FirstOrDefault(p => p.PacienteId == expediente.PacienteId) ?? new Pacientes();

            var alergias = (from ea in _context.ExpedienteAlergias
                            join a in _context.Alergias on ea.AlergiaId equals a.AlergiaId
                            where ea.ExpedienteId == id
                            orderby a.Nombre
                            select new ExpedienteAlergiaItem
                            {
                                Alergia = a.Nombre,
                                Nivel = ea.Nivel,
                                Observaciones = ea.Observaciones
                            }).ToList();

            var enfermedades = (from ee in _context.ExpedienteEnfermedades
                                join e in _context.Enfermedades on ee.EnfermedadId equals e.EnfermedadId
                                where ee.ExpedienteId == id
                                orderby e.Nombre
                                select new ExpedienteEnfermedadItem
                                {
                                    Enfermedad = e.Nombre,
                                    TipoEnfermedad = e.TipoEnfermedad,
                                    FechaDeteccion = ee.FechaDeteccion,
                                    Observaciones = ee.Observaciones
                                }).ToList();

            var antecedentes = _context.Antecedentes
                .Where(an => an.ExpedienteId == id)
                .OrderByDescending(an => an.Fecha)
                .Select(an => new AntecedenteItem
                {
                    Tipo = an.Tipo,
                    Descripcion = an.Descripcion,
                    Fecha = an.Fecha,
                    Observaciones = an.Observaciones
                })
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

            ViewBag.EsEnfermera = EsEnfermera();
            return View(modelo);
        }

        public IActionResult Imprimir(int id)
        {
            var modelo = ObtenerDetalle(id);

            if (modelo == null)
            {
                return NotFound();
            }

            ViewBag.EsEnfermera = EsEnfermera();
            return View(modelo);
        }

        public IActionResult Edit(int id)
        {
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso denegado: Las enfermeras no tienen permisos para editar consultas.";
                return RedirectToAction(nameof(Index));
            }

            var consulta = _context.Consultas.Find(id);

            if (consulta == null)
            {
                return NotFound();
            }

            CargarListas(consulta);
            return View(consulta);
        }

        [HttpPost]
        public IActionResult Edit(int id, Consultas consulta)
        {
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso denegado: Las enfermeras no tienen permisos para editar consultas.";
                return RedirectToAction(nameof(Index));
            }

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

            CargarListas(consulta);
            return View(consulta);
        }

        [HttpPost]
        public IActionResult AsignarEnfermedad(int consultaId, string? enfermedad, string? observaciones)
        {
            if (EsEnfermera())
            {
                TempData["ErrorEnfermedad"] = "Acceso restringido: Solo los médicos pueden asignar diagnósticos.";
                return RedirectToAction(nameof(Details), new { id = consultaId });
            }

            if (!_context.Consultas.Any(c => c.ConsultaId == consultaId))
            {
                return NotFound();
            }

            var existente = BuscarEnfermedadPorNombre(enfermedad);

            if (!ValidarNombreEnfermedad(enfermedad))
            {
                TempData["ErrorEnfermedad"] = "Debe indicar la enfermedad (máximo 100 caracteres).";
            }
            // La BD no permite repetir la misma enfermedad en una consulta (UQ_ConsultaEnfermedades)
            else if (existente != null && _context.ConsultaEnfermedades.Any(ce => ce.ConsultaId == consultaId && ce.EnfermedadId == existente.EnfermedadId))
            {
                TempData["ErrorEnfermedad"] = "Esa enfermedad ya está asignada a la consulta.";
            }
            else
            {
                _context.ConsultaEnfermedades.Add(new ConsultaEnfermedades
                {
                    ConsultaId = consultaId,
                    EnfermedadId = (existente ?? AgregarEnfermedadAlCatalogo(enfermedad!)).EnfermedadId,
                    Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
                });
                _context.SaveChanges();
            }

            return Redirect(Url.Action(nameof(Details), new { id = consultaId }) + "#diagnostico");
        }

        [HttpPost]
        public IActionResult EditarEnfermedad(int id, string? enfermedad, string? observaciones)
        {
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso restringido: Las enfermeras no pueden modificar diagnósticos.";
                return RedirectToAction(nameof(Index));
            }

            var registro = _context.ConsultaEnfermedades.Find(id);
            if (registro == null)
            {
                return NotFound();
            }

            var existente = BuscarEnfermedadPorNombre(enfermedad);

            if (!ValidarNombreEnfermedad(enfermedad))
            {
                TempData["ErrorEnfermedad"] = "Debe indicar la enfermedad (máximo 100 caracteres).";
            }
            else if (existente != null && _context.ConsultaEnfermedades.Any(ce => ce.ConsultaId == registro.ConsultaId && ce.EnfermedadId == existente.EnfermedadId && ce.ConsultaEnfermedadId != id))
            {
                TempData["ErrorEnfermedad"] = "Esa enfermedad ya está asignada a la consulta.";
            }
            else
            {
                registro.EnfermedadId = (existente ?? AgregarEnfermedadAlCatalogo(enfermedad!)).EnfermedadId;
                registro.Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim();
                _context.SaveChanges();
            }

            return Redirect(Url.Action(nameof(Details), new { id = registro.ConsultaId }) + "#diagnostico");
        }

        private static bool ValidarNombreEnfermedad(string? nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre) && nombre.Trim().Length <= 100;
        }

        private Enfermedades? BuscarEnfermedadPorNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;

            nombre = nombre.Trim();
            return _context.Enfermedades.FirstOrDefault(e => e.Nombre == nombre);
        }

        // Si la enfermedad escrita no existe en el catálogo, se registra como tipo "Otra" (igual que en el prototipo)
        private Enfermedades AgregarEnfermedadAlCatalogo(string nombre)
        {
            var codigos = _context.Enfermedades.Select(e => e.Codigo).ToList();
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

            var nueva = new Enfermedades
            {
                Codigo = $"ENF-{maxNumero + 1:D3}",
                Nombre = nombre.Trim(),
                TipoEnfermedad = "Otra"
            };

            _context.Enfermedades.Add(nueva);
            _context.SaveChanges();
            return nueva;
        }

        [HttpPost]
        public IActionResult QuitarEnfermedad(int id)
        {
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso restringido: Las enfermeras no pueden modificar diagnósticos.";
                return RedirectToAction(nameof(Index));
            }

            var registro = _context.ConsultaEnfermedades.Find(id);

            if (registro == null)
            {
                return NotFound();
            }

            _context.ConsultaEnfermedades.Remove(registro);
            _context.SaveChanges();

            return Redirect(Url.Action(nameof(Details), new { id = registro.ConsultaId }) + "#diagnostico");
        }

        public IActionResult Delete(int id)
        {
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso denegado: Las enfermeras no tienen permisos para eliminar consultas.";
                return RedirectToAction(nameof(Index));
            }

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
            if (EsEnfermera())
            {
                TempData["Error"] = "Acceso denegado: Las enfermeras no tienen permisos para eliminar consultas.";
                return RedirectToAction(nameof(Index));
            }

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
