using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

public class PacientesController : Controller
{
    private readonly AppDbContext _context;

    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: PACIENTESS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Pacientes.ToListAsync());
    }

    // GET: PACIENTESS/Details/5
    public async Task<IActionResult> Details(int? pacienteid)
    {
        if (pacienteid == null)
        {
            return NotFound();
        }

        var pacientes = await _context.Pacientes
            .FirstOrDefaultAsync(m => m.PacienteId == pacienteid);
        if (pacientes == null)
        {
            return NotFound();
        }

        return View(pacientes);
    }

    // GET: PACIENTESS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PACIENTESS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PacienteId,Codigo,Nombre,Sexo,FechaNac,DUI,Altura,Peso,Telefono,Direccion,CreadoEn")] Pacientes pacientes)
    {
        if (ModelState.IsValid)
        {
            _context.Add(pacientes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(pacientes);
    }

    // GET: PACIENTESS/Edit/5
    public async Task<IActionResult> Edit(int? pacienteid)
    {
        if (pacienteid == null)
        {
            return NotFound();
        }

        var pacientes = await _context.Pacientes.FindAsync(pacienteid);
        if (pacientes == null)
        {
            return NotFound();
        }
        return View(pacientes);
    }

    // POST: PACIENTESS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? pacienteid, [Bind("PacienteId,Codigo,Nombre,Sexo,FechaNac,DUI,Altura,Peso,Telefono,Direccion,CreadoEn")] Pacientes pacientes)
    {
        if (pacienteid != pacientes.PacienteId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(pacientes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacientesExists(pacientes.PacienteId))
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
        return View(pacientes);
    }

    // GET: PACIENTESS/Delete/5
    public async Task<IActionResult> Delete(int? pacienteid)
    {
        if (pacienteid == null)
        {
            return NotFound();
        }

        var pacientes = await _context.Pacientes
            .FirstOrDefaultAsync(m => m.PacienteId == pacienteid);
        if (pacientes == null)
        {
            return NotFound();
        }

        return View(pacientes);
    }

    // POST: PACIENTESS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? pacienteid)
    {
        var pacientes = await _context.Pacientes.FindAsync(pacienteid);
        if (pacientes != null)
        {
            _context.Pacientes.Remove(pacientes);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PacientesExists(int? pacienteid)
    {
        return _context.Pacientes.Any(e => e.PacienteId == pacienteid);
    }
}
