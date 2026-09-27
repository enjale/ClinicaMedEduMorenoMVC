using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

public class ContactoEmergenciasController : Controller
{
    private readonly AppDbContext _context;

    public ContactoEmergenciasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: CONTACTOEMERGENCIAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ContactoEmergencias.ToListAsync());
    }

    // GET: CONTACTOEMERGENCIAS/Details/5
    public async Task<IActionResult> Details(int? contactoemergenciaid)
    {
        if (contactoemergenciaid == null)
        {
            return NotFound();
        }

        var contactoemergencia = await _context.ContactoEmergencias
            .FirstOrDefaultAsync(m => m.ContactoEmergenciaId == contactoemergenciaid);
        if (contactoemergencia == null)
        {
            return NotFound();
        }

        return View(contactoemergencia);
    }

    // GET: CONTACTOEMERGENCIAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CONTACTOEMERGENCIAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ContactoEmergenciaId,PacienteId,Nombre,Relacion,Telefono,TelefonoAlterno,Direccion,Prioridad,Activo,CreadoEn")] ContactoEmergencia contactoemergencia)
    {
        if (ModelState.IsValid)
        {
            _context.Add(contactoemergencia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(contactoemergencia);
    }

    // GET: CONTACTOEMERGENCIAS/Edit/5
    public async Task<IActionResult> Edit(int? contactoemergenciaid)
    {
        if (contactoemergenciaid == null)
        {
            return NotFound();
        }

        var contactoemergencia = await _context.ContactoEmergencias.FindAsync(contactoemergenciaid);
        if (contactoemergencia == null)
        {
            return NotFound();
        }
        return View(contactoemergencia);
    }

    // POST: CONTACTOEMERGENCIAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? contactoemergenciaid, [Bind("ContactoEmergenciaId,PacienteId,Nombre,Relacion,Telefono,TelefonoAlterno,Direccion,Prioridad,Activo,CreadoEn")] ContactoEmergencia contactoemergencia)
    {
        if (contactoemergenciaid != contactoemergencia.ContactoEmergenciaId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(contactoemergencia);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactoEmergenciaExists(contactoemergencia.ContactoEmergenciaId))
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
        return View(contactoemergencia);
    }

    // GET: CONTACTOEMERGENCIAS/Delete/5
    public async Task<IActionResult> Delete(int? contactoemergenciaid)
    {
        if (contactoemergenciaid == null)
        {
            return NotFound();
        }

        var contactoemergencia = await _context.ContactoEmergencias
            .FirstOrDefaultAsync(m => m.ContactoEmergenciaId == contactoemergenciaid);
        if (contactoemergencia == null)
        {
            return NotFound();
        }

        return View(contactoemergencia);
    }

    // POST: CONTACTOEMERGENCIAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? contactoemergenciaid)
    {
        var contactoemergencia = await _context.ContactoEmergencias.FindAsync(contactoemergenciaid);
        if (contactoemergencia != null)
        {
            _context.ContactoEmergencias.Remove(contactoemergencia);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ContactoEmergenciaExists(int? contactoemergenciaid)
    {
        return _context.ContactoEmergencias.Any(e => e.ContactoEmergenciaId == contactoemergenciaid);
    }
}
