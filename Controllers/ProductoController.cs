using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using theburycode.Models;
using theburycode.Services;
using theburycode.Services.Shared;
using theburycode.ViewModels;

namespace theburycode.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IPrecioCalculatorService _precioCalculator;
        private readonly IFileUploadService _fileUploadService;
        private readonly TheBuryCodeContext _context;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(
            IProductoService productoService,
            IPrecioCalculatorService precioCalculator,
            IFileUploadService fileUploadService,
            TheBuryCodeContext context,
            ILogger<ProductoController> logger)
        {
            _productoService = productoService;
            _precioCalculator = precioCalculator;
            _fileUploadService = fileUploadService;
            _context = context;
            _logger = logger;
        }

        // GET: Producto
        public async Task<IActionResult> Index(ProductoFiltroViewModel filtro)
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Submarca)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtro.Busqueda))
            {
                query = query.Where(p =>
                    p.CodigoAlfaNum.Contains(filtro.Busqueda) ||
                    p.Nombre.Contains(filtro.Busqueda) ||
                    p.Descripcion.Contains(filtro.Busqueda));
            }

            if (filtro.CategoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == filtro.CategoriaId);

            if (filtro.MarcaId.HasValue)
                query = query.Where(p => p.MarcaId == filtro.MarcaId);

            if (filtro.SoloActivos.HasValue && filtro.SoloActivos.Value)
                query = query.Where(p => p.Activo);

            if (filtro.SoloStockBajo.HasValue && filtro.SoloStockBajo.Value)
                query = query.Where(p => p.StockActual < p.StockMinimo);

            var productos = await query.OrderBy(p => p.Nombre).ToListAsync();

            // Calcular precios
            var viewModels = new List<ProductoViewModel>();
            foreach (var p in productos)
            {
                var precios = await _precioCalculator.GetPreciosProducto(p.Id);
                viewModels.Add(MapToViewModel(p, precios));
            }

            // Aplicar filtros de precio después del cálculo
            if (filtro.PrecioDesde.HasValue)
                viewModels = viewModels.Where(p => p.PrecioFinal >= filtro.PrecioDesde).ToList();

            if (filtro.PrecioHasta.HasValue)
                viewModels = viewModels.Where(p => p.PrecioFinal <= filtro.PrecioHasta).ToList();

            LoadViewData();
            ViewData["Filtro"] = filtro;
            return View(viewModels);
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);
            if (producto == null)
                return NotFound();

            var precios = await _precioCalculator.GetPreciosProducto(id.Value);
            var viewModel = MapToViewModel(producto, precios);

            // Cargar historial
            ViewData["HistorialPrecios"] = await _context.PrecioLogs
                .Where(p => p.ProductoId == id)
                .OrderByDescending(p => p.FechaCambio)
                .Take(10)
                .ToListAsync();

            ViewData["HistorialStock"] = await _context.StockLogs
                .Where(s => s.ProductoId == id)
                .OrderByDescending(s => s.FechaMovimiento)
                .Take(20)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            LoadViewData();
            var viewModel = new ProductoViewModel
            {
                Activo = true,
                IvaPct = 21,
                CodigoNum = GenerarCodigoNumerico()
            };
            return View(viewModel);
        }

        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar código único
                    if (await _context.Productos.AnyAsync(p => p.CodigoNum == viewModel.CodigoNum))
                    {
                        ModelState.AddModelError("CodigoNum", "El código ya existe");
                        LoadViewData();
                        return View(viewModel);
                    }

                    var producto = MapToEntity(viewModel);
                    producto.CodigoAlfaNum = GenerarCodigoAlfanumerico(viewModel);
                    producto.UsuarioAlta = User.Identity?.Name ?? "SYSTEM";
                    producto.EstadoProducto = "A";

                    // Subir imagen si existe
                    if (viewModel.ImagenFile != null)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        if (_fileUploadService.ValidateFile(viewModel.ImagenFile, allowedExtensions, 5 * 1024 * 1024))
                        {
                            var fileName = await _fileUploadService.UploadFileAsync(viewModel.ImagenFile, "productos");
                            producto.OrigenStock = fileName; // Usamos OrigenStock temporalmente para la imagen
                        }
                    }

                    await _productoService.CreateAsync(producto);
                    TempData["Success"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear producto");
                    ModelState.AddModelError("", "Error al guardar el producto");
                }
            }

            LoadViewData();
            return View(viewModel);
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);
            if (producto == null)
                return NotFound();

            var precios = await _precioCalculator.GetPreciosProducto(id.Value);
            var viewModel = MapToViewModel(producto, precios);

            LoadViewData();
            return View(viewModel);
        }

        // POST: Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var producto = await _productoService.GetByIdAsync(id);
                    if (producto == null)
                        return NotFound();

                    // Mapear cambios
                    producto.Nombre = viewModel.Nombre;
                    producto.Descripcion = viewModel.Descripcion;
                    producto.CategoriaId = viewModel.CategoriaId;
                    producto.MarcaId = viewModel.MarcaId;
                    producto.SubmarcaId = viewModel.SubmarcaId;
                    producto.PrecioCosto = viewModel.PrecioCosto;
                    producto.MargenVentaPct = viewModel.MargenVentaPct;
                    producto.DescuentoContadoPct = viewModel.DescuentoContadoPct;
                    producto.IvaPct = viewModel.IvaPct;
                    producto.StockActual = viewModel.StockActual;
                    producto.StockMinimo = viewModel.StockMinimo;
                    producto.Activo = viewModel.Activo;
                    producto.UsuarioModificacion = User.Identity?.Name ?? "SYSTEM";

                    // Actualizar imagen si se subió una nueva
                    if (viewModel.ImagenFile != null)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        if (_fileUploadService.ValidateFile(viewModel.ImagenFile, allowedExtensions, 5 * 1024 * 1024))
                        {
                            // Eliminar imagen anterior si existe
                            if (!string.IsNullOrEmpty(producto.OrigenStock))
                            {
                                await _fileUploadService.DeleteFileAsync($"uploads/productos/{producto.OrigenStock}");
                            }

                            var fileName = await _fileUploadService.UploadFileAsync(viewModel.ImagenFile, "productos");
                            producto.OrigenStock = fileName;
                        }
                    }

                    await _productoService.UpdateAsync(producto);
                    TempData["Success"] = "Producto actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductoExists(viewModel.Id))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar producto");
                    ModelState.AddModelError("", "Error al actualizar el producto");
                }
            }

            LoadViewData();
            return View(viewModel);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _productoService.GetByIdAsync(id.Value);
            if (producto == null)
                return NotFound();

            var precios = await _precioCalculator.GetPreciosProducto(id.Value);
            var viewModel = MapToViewModel(producto, precios);

            // Verificar si tiene movimientos
            ViewData["TieneMovimientos"] = await _context.VentaDetalles.AnyAsync(v => v.ProductoId == id) ||
                                          await _context.CompraDetalles.AnyAsync(c => c.ProductoId == id);

            return View(viewModel);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _productoService.DeleteAsync(id);
                if (result)
                {
                    TempData["Success"] = "Producto desactivado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo desactivar el producto";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                TempData["Error"] = "Error al eliminar el producto";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Producto/AjustePrecioMasivo
        [HttpPost]
        public async Task<IActionResult> AjustePrecioMasivo(int[] productosIds, decimal porcentaje, string tipoAjuste)
        {
            try
            {
                var usuario = User.Identity?.Name ?? "SYSTEM";
                var actualizados = await _precioCalculator.AplicarAjusteMasivo(
                    productosIds.ToList(),
                    porcentaje,
                    tipoAjuste,
                    usuario);

                return Json(new { success = true, actualizados });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ajuste masivo");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Producto/StockBajo
        public async Task<IActionResult> StockBajo()
        {
            var productos = await _productoService.GetStockBajoAsync();
            var viewModels = new List<ProductoViewModel>();

            foreach (var p in productos)
            {
                var precios = await _precioCalculator.GetPreciosProducto(p.Id);
                viewModels.Add(MapToViewModel(p, precios));
            }

            return View(viewModels);
        }

        // Métodos auxiliares
        private async Task<bool> ProductoExists(int id)
        {
            return await _context.Productos.AnyAsync(e => e.Id == id);
        }

        private void LoadViewData()
        {
            // Cargar solo subrubros (categorías tipo 'S')
            ViewData["Categorias"] = new SelectList(
                _context.Categoria.Where(c => c.Tipo == "S").OrderBy(c => c.Nombre),
                "Id",
                "Nombre");

            // Cargar solo marcas principales
            ViewData["Marcas"] = new SelectList(
                _context.Marcas.Where(m => m.ParentId == null).OrderBy(m => m.Nombre),
                "Id",
                "Nombre");
        }

        private int GenerarCodigoNumerico()
        {
            var ultimo = _context.Productos
                .OrderByDescending(p => p.CodigoNum)
                .Select(p => p.CodigoNum)
                .FirstOrDefault();

            return ultimo + 1;
        }

        private string GenerarCodigoAlfanumerico(ProductoViewModel viewModel)
        {
            var categoria = _context.Categoria.Find(viewModel.CategoriaId);
            var marca = _context.Marcas.Find(viewModel.MarcaId);

            var prefijoCat = categoria?.Nombre.Substring(0, Math.Min(3, categoria.Nombre.Length)).ToUpper() ?? "GEN";
            var prefijoMarca = marca?.Nombre.Substring(0, Math.Min(2, marca.Nombre.Length)).ToUpper() ?? "XX";

            return $"{prefijoCat}-{prefijoMarca}-{viewModel.CodigoNum:D5}";
        }

        private ProductoViewModel MapToViewModel(Producto producto, PrecioProductoDto? precios = null)
        {
            var vm = new ProductoViewModel
            {
                Id = producto.Id,
                CodigoNum = producto.CodigoNum,
                CodigoAlfaNum = producto.CodigoAlfaNum,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CategoriaId = producto.CategoriaId,
                MarcaId = producto.MarcaId,
                SubmarcaId = producto.SubmarcaId,
                PrecioCosto = producto.PrecioCosto,
                MargenVentaPct = producto.MargenVentaPct,
                DescuentoContadoPct = producto.DescuentoContadoPct,
                IvaPct = producto.IvaPct,
                StockActual = producto.StockActual,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo,
                CategoriaNombre = producto.Categoria?.Nombre,
                MarcaNombre = producto.Marca?.Nombre,
                SubmarcaNombre = producto.Submarca?.Nombre,
                ImagenUrl = !string.IsNullOrEmpty(producto.OrigenStock)
                    ? _fileUploadService.GetFileUrl(producto.OrigenStock, "productos")
                    : null
            };

            if (precios != null)
            {
                vm.PrecioLista = precios.PrecioLista;
                vm.PrecioContado = precios.PrecioContado;
                vm.PrecioFinal = precios.PrecioFinal;
            }

            return vm;
        }

        private Producto MapToEntity(ProductoViewModel viewModel)
        {
            return new Producto
            {
                Id = viewModel.Id,
                CodigoNum = viewModel.CodigoNum,
                CodigoAlfaNum = viewModel.CodigoAlfaNum,
                Nombre = viewModel.Nombre,
                Descripcion = viewModel.Descripcion,
                CategoriaId = viewModel.CategoriaId,
                MarcaId = viewModel.MarcaId,
                SubmarcaId = viewModel.SubmarcaId,
                PrecioCosto = viewModel.PrecioCosto,
                MargenVentaPct = viewModel.MargenVentaPct,
                DescuentoContadoPct = viewModel.DescuentoContadoPct,
                IvaPct = viewModel.IvaPct,
                StockActual = viewModel.StockActual,
                StockMinimo = viewModel.StockMinimo,
                Activo = viewModel.Activo
            };
        }

        // AJAX Methods
        [HttpGet]
        public async Task<IActionResult> GetSubmarcas(int marcaId)
        {
            var submarcas = await _context.Marcas
                .Where(m => m.ParentId == marcaId)
                .Select(m => new { value = m.Id, text = m.Nombre })
                .ToListAsync();

            return Json(submarcas);
        }
    }
}