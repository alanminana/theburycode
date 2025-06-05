using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using theburycode.Models;
using theburycode.ViewModels;

namespace theburycode.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly TheBuryCodeContext _context;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(TheBuryCodeContext context, ILogger<CategoriaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Categoria
        public async Task<IActionResult> Index()
        {
            var viewModel = new CategoriaIndexViewModel
            {
                Categorias = await _context.Categoria
                    .Include(c => c.Parent)
                    .Include(c => c.InverseParent)
                    .Include(c => c.Productos)
                    .OrderBy(c => c.Tipo)
                    .ThenBy(c => c.Parent != null ? c.Parent.Nombre : c.Nombre)
                    .ThenBy(c => c.Nombre)
                    .ToListAsync(),

                Marcas = await _context.Marcas
                    .Include(m => m.Parent)
                    .Include(m => m.InverseParent)
                    .Include(m => m.ProductoMarcas)
                    .OrderBy(m => m.Tipo)
                    .ThenBy(m => m.Parent != null ? m.Parent.Nombre : m.Nombre)
                    .ThenBy(m => m.Nombre)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: Categoria/Create
        public IActionResult Create()
        {
            ViewBag.Rubros = _context.Categoria
                .Where(c => c.Tipo == "R")
                .Select(c => new { c.Id, c.Nombre })
                .ToList();
            return View();
        }

        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            _logger.LogInformation($"Creando categoría: {categoria.Nombre}, Tipo: {categoria.Tipo}");

            if (ModelState.IsValid)
            {
                categoria.UsuarioAlta = User.Identity?.Name ?? "SYSTEM";
                categoria.FechaAlta = DateTime.Now;

                _context.Add(categoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Categoría creada con ID: {categoria.Id}");
                TempData["Success"] = $"{(categoria.Tipo == "R" ? "Rubro" : "Subrubro")} creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Rubros = _context.Categoria.Where(c => c.Tipo == "R").ToList();
            return View(categoria);
        }

        // GET: Categoria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null) return NotFound();

            ViewBag.Rubros = _context.Categoria
                .Where(c => c.Tipo == "R" && c.Id != id)
                .ToList();

            return View(categoria);
        }

        // POST: Categoria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();

            _logger.LogInformation($"Editando categoría ID: {id}");

            if (ModelState.IsValid)
            {
                try
                {
                    categoria.FechaModificacion = DateTime.Now;
                    categoria.UsuarioModificacion = User.Identity?.Name ?? "SYSTEM";

                    _context.Update(categoria);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Categoría {id} actualizada");
                    TempData["Success"] = "Categoría actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(categoria);
        }

        // POST: Categoria/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Intentando eliminar categoría ID: {id}");

            var categoria = await _context.Categoria
                .Include(c => c.InverseParent)
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                _logger.LogWarning($"Categoría {id} no encontrada");
                return Json(new { success = false, error = "Categoría no encontrada" });
            }

            if (categoria.InverseParent.Any())
            {
                _logger.LogWarning($"Categoría {id} tiene subcategorías");
                return Json(new { success = false, error = "No se puede eliminar porque tiene subcategorías" });
            }

            if (categoria.Productos.Any())
            {
                _logger.LogWarning($"Categoría {id} tiene productos");
                return Json(new { success = false, error = "No se puede eliminar porque tiene productos asociados" });
            }

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Categoría {id} eliminada");
            return Json(new { success = true });
        }

        // AJAX: Crear categoría desde modal
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Categoria categoria)
        {
            _logger.LogInformation($"CreateAjax - Categoría: {categoria.Nombre}");

            if (string.IsNullOrEmpty(categoria.Nombre))
                return Json(new { success = false, error = "El nombre es requerido" });

            categoria.UsuarioAlta = User.Identity?.Name ?? "SYSTEM";
            categoria.FechaAlta = DateTime.Now;

            _context.Add(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Categoría creada vía AJAX con ID: {categoria.Id}");

            return Json(new
            {
                success = true,
                id = categoria.Id,
                nombre = categoria.Nombre,
                tipo = categoria.Tipo
            });
        }

        // ==================== MARCAS ====================

        // GET: Categoria/Marcas
        public async Task<IActionResult> Marcas()
        {
            var marcas = await _context.Marcas
                .Include(m => m.Parent)
                .Include(m => m.InverseParent)
                .OrderBy(m => m.Tipo)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

            return View(marcas);
        }

        // POST: Categoria/DeleteMarca/5
        [HttpPost]
        public async Task<IActionResult> DeleteMarca(int id)
        {
            _logger.LogInformation($"Intentando eliminar marca ID: {id}");

            var marca = await _context.Marcas
                .Include(m => m.InverseParent)
                .Include(m => m.ProductoMarcas)
                .Include(m => m.ProductoSubmarcas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (marca == null)
            {
                _logger.LogWarning($"Marca {id} no encontrada");
                return Json(new { success = false, error = "Marca no encontrada" });
            }

            if (marca.InverseParent.Any())
            {
                _logger.LogWarning($"Marca {id} tiene submarcas");
                return Json(new { success = false, error = "No se puede eliminar porque tiene submarcas" });
            }

            if (marca.ProductoMarcas.Any() || marca.ProductoSubmarcas.Any())
            {
                _logger.LogWarning($"Marca {id} tiene productos");
                return Json(new { success = false, error = "No se puede eliminar porque tiene productos asociados" });
            }

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Marca {id} eliminada");
            return Json(new { success = true });
        }

        // AJAX: Crear marca desde modal
        [HttpPost]
        public async Task<IActionResult> CreateMarcaAjax([FromBody] Marca marca)
        {
            _logger.LogInformation($"CreateMarcaAjax - Marca: {marca.Nombre}");

            if (string.IsNullOrEmpty(marca.Nombre))
                return Json(new { success = false, error = "El nombre es requerido" });

            marca.UsuarioAlta = User.Identity?.Name ?? "SYSTEM";
            marca.FechaAlta = DateTime.Now;

            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Marca creada vía AJAX con ID: {marca.Id}");

            return Json(new
            {
                success = true,
                id = marca.Id,
                nombre = marca.Nombre,
                tipo = marca.Tipo
            });
        }

        // Métodos auxiliares
        private bool CategoriaExists(int id)
        {
            return _context.Categoria.Any(e => e.Id == id);
        }
    }
}