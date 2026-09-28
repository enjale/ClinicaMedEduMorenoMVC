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

        public IActionResult Index(int pagina = 1)
        {
            if (pagina < 1) pagina = 1;
            int tamañoPagina = 10;
            var totalRegistros = _context.Consultas.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamañoPagina);
            if (totalPaginas == 0) totalPaginas = 1;

            if (pagina > totalPaginas) pagina = totalPaginas;

            var consultas = _context.Consultas
                .OrderByDescending(c => c.Fecha)
                .ThenByDescending(c => c.ConsultaId)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            // Nombre del paciente por expediente, solo de las consultas de esta página
            var expedienteIds = consultas.Select(c => c.ExpedienteId).Distinct().ToList();
            ViewBag.Pacientes = (from e in _context.Expedientes
                                 join p in _context.Pacientes on e.PacienteId equals p.PacienteId
                                 where expedienteIds.Contains(e.ExpedienteId)
                                 select new { e.ExpedienteId, p.Nombre })
                                .ToDictionary(x => x.ExpedienteId, x => x.Nombre);

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

            return View(modelo);
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
                            select new { ExpedienteCodigo = e.Codigo, p.Codigo, p.Nombre })
                           .FirstOrDefault();

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

            return new ConsultaDetalleViewModel
            {
                Consulta = consulta,
                NombrePaciente = paciente?.Nombre ?? "—",
                CodigoPaciente = paciente?.Codigo ?? string.Empty,
                CodigoExpediente = paciente?.ExpedienteCodigo ?? string.Empty,
                RegistradoPor = usuario ?? "—",
                Enfermedades = enfermedades
            };
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
