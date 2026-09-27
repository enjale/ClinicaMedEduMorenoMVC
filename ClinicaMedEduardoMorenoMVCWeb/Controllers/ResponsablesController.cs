using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;
using ClinicaMedEduardoMorenoMVCWeb.Data;

public class ResponsablesController : Controller
{
    private readonly AppDbContext _context;

    public ResponsablesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: RESPONSABLESS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Responsables.ToListAsync());
    }

    // GET: RESPONSABLESS/Details/5
    public async Task<IActionResult> Details(int? responsableid)
    {
        if (responsableid == null)
        {
            return NotFound();
        }

        var responsables = await _context.Responsables
            .FirstOrDefaultAsync(m => m.ResponsableId == responsableid);
        if (responsables == null)
        {
            return NotFound();
        }

        return View(responsables);
    }

    // GET: RESPONSABLESS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: RESPONSABLESS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ResponsableId,PacienteId,Nombre,DUI,Parentesco,Telefono,Direccion")] Responsables responsables)
    {
        if (ModelState.IsValid)
        {
            _context.Add(responsables);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(responsables);
    }

    // GET: RESPONSABLESS/Edit/5
    public async Task<IActionResult> Edit(int? responsableid)
    {
        if (responsableid == null)
        {
            return NotFound();
        }

        var responsables = await _context.Responsables.FindAsync(responsableid);
        if (responsables == null)
        {
            return NotFound();
        }
        return View(responsables);
    }

    // POST: RESPONSABLESS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? responsableid, [Bind("ResponsableId,PacienteId,Nombre,DUI,Parentesco,Telefono,Direccion")] Responsables responsables)
    {
        if (responsableid != responsables.ResponsableId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(responsables);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResponsablesExists(responsables.ResponsableId))
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
        return View(responsables);
    }

    // GET: RESPONSABLESS/Delete/5
    public async Task<IActionResult> Delete(int? responsableid)
    {
        if (responsableid == null)
        {
            return NotFound();
        }

        var responsables = await _context.Responsables
            .FirstOrDefaultAsync(m => m.ResponsableId == responsableid);
        if (responsables == null)
        {
            return NotFound();
        }

        return View(responsables);
    }

    // POST: RESPONSABLESS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? responsableid)
    {
        var responsables = await _context.Responsables.FindAsync(responsableid);
        if (responsables != null)
        {
            _context.Responsables.Remove(responsables);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ResponsablesExists(int? responsableid)
    {
        return _context.Responsables.Any(e => e.ResponsableId == responsableid);
    }
}
