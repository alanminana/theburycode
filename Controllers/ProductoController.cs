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
                    (p.Descripcion != null && p.Descripcion.Contains(filtro.Busqueda)));
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
            _logger.LogInformation($"Create POST: {viewModel.Nombre}, Categoria: {viewModel.CategoriaId}");

            // Generar código alfanumérico antes de validar
            if (string.IsNullOrEmpty(viewModel.CodigoAlfaNum))
            {
                viewModel.CodigoAlfaNum = await GenerarCodigoAlfanumerico(viewModel);
                _logger.LogInformation($"Código alfanumérico generado: {viewModel.CodigoAlfaNum}");
            }

            // Remover errores de campos autogenerados
            ModelState.Remove("CodigoAlfaNum");

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar código único
                    if (await _context.Productos.AnyAsync(p => p.CodigoNum == viewModel.CodigoNum))
                    {
                        _logger.LogWarning($"Código numérico duplicado: {viewModel.CodigoNum}");
                        ModelState.AddModelError("CodigoNum", "El código ya existe");
                        LoadViewData();
                        return View(viewModel);
                    }

                    var producto = MapToEntity(viewModel);
                    producto.UsuarioAlta = User.Identity?.Name ?? "SYSTEM";
                    producto.EstadoProducto = "A";
                    producto.FechaAlta = DateTime.Now;

                    _logger.LogInformation($"Guardando producto: Código={producto.CodigoAlfaNum}, Nombre={producto.Nombre}");

                    // Subir imagen si existe
                    if (viewModel.ImagenFile != null)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        if (_fileUploadService.ValidateFile(viewModel.ImagenFile, allowedExtensions, 5 * 1024 * 1024))
                        {
                            var fileName = await _fileUploadService.UploadFileAsync(viewModel.ImagenFile, "productos");
                            producto.OrigenStock = fileName;
                            _logger.LogInformation($"Imagen subida: {fileName}");
                        }
                    }

                    await _productoService.CreateAsync(producto);

                    _logger.LogInformation($"Producto creado exitosamente con ID: {producto.Id}");
                    TempData["Success"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al crear producto: {viewModel.Nombre}");
                    ModelState.AddModelError("", $"Error al guardar el producto: {ex.Message}");
                }
            }
            else
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning($"ModelState inválido. Errores: {string.Join(", ", errores)}");
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
                    TempData["Success"] = "Producto eliminado/desactivado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el producto";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                TempData["Error"] = "Error al eliminar el producto";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Producto/RollbackPrecio
        [HttpPost]
        public async Task<IActionResult> RollbackPrecio(int productoId, int precioLogId)
        {
            _logger.LogInformation($"RollbackPrecio: ProductoId={productoId}, PrecioLogId={precioLogId}");

            try
            {
                var precioLog = await _context.PrecioLogs.FindAsync(precioLogId);
                if (precioLog == null)
                {
                    _logger.LogWarning($"PrecioLog {precioLogId} no encontrado");
                    return Json(new { success = false, error = "Registro no encontrado" });
                }

                var producto = await _context.Productos.FindAsync(productoId);
                if (producto == null)
                {
                    _logger.LogWarning($"Producto {productoId} no encontrado");
                    return Json(new { success = false, error = "Producto no encontrado" });
                }

                // Guardar precio actual antes de revertir
                var nuevoPrecioLog = new PrecioLog
                {
                    ProductoId = productoId,
                    FechaCambio = DateTime.Now,
                    PrecioAnterior = producto.PrecioCosto,
                    PrecioNuevo = precioLog.PrecioAnterior,
                    Usuario = User.Identity?.Name ?? "SYSTEM"
                };

                producto.PrecioCosto = precioLog.PrecioAnterior;
                producto.FechaModificacion = DateTime.Now;
                producto.UsuarioModificacion = User.Identity?.Name ?? "SYSTEM";

                _context.PrecioLogs.Add(nuevoPrecioLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Precio revertido para producto {productoId} a ${precioLog.PrecioAnterior}");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revertir precio");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Producto/AjustePrecioMasivo
        [HttpPost]
        public async Task<IActionResult> AjustePrecioMasivo(int[] productosIds, decimal porcentaje, string tipoAjuste)
        {
            _logger.LogInformation($"AjustePrecioMasivo iniciado: {productosIds?.Length ?? 0} productos, {porcentaje}% {tipoAjuste}");

            try
            {
                if (productosIds == null || productosIds.Length == 0)
                {
                    _logger.LogWarning("No se recibieron IDs de productos");
                    return Json(new { success = false, error = "No se recibieron productos", actualizados = 0 });
                }

                var usuario = User.Identity?.Name ?? "SYSTEM";
                _logger.LogInformation($"IDs recibidos: {string.Join(",", productosIds)}");

                var actualizados = await _precioCalculator.AplicarAjusteMasivo(
                    productosIds.ToList(),
                    porcentaje,
                    tipoAjuste,
                    usuario);

                _logger.LogInformation($"Ajuste completado: {actualizados} productos actualizados");
                return Json(new { success = true, actualizados });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ajuste masivo");
                return Json(new { success = false, error = ex.Message, actualizados = 0 });
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

        // AJAX Methods
        [HttpGet]
        public async Task<IActionResult> GetRubros()
        {
            _logger.LogInformation("GetRubros llamado");
            var rubros = await _context.Categoria
                .Where(c => c.Tipo == "R")
                .Select(r => new { value = r.Id, text = r.Nombre })
                .ToListAsync();

            _logger.LogInformation($"Rubros encontrados: {rubros.Count}");
            return Json(rubros);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubrubros(int rubroId)
        {
            _logger.LogInformation($"GetSubrubros llamado con rubroId: {rubroId}");
            var subrubros = await _context.Categoria
                .Where(c => c.ParentId == rubroId && c.Tipo == "S")
                .Select(s => new { value = s.Id, text = s.Nombre })
                .ToListAsync();

            _logger.LogInformation($"Subrubros encontrados: {subrubros.Count}");
            return Json(subrubros);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubmarcas(int marcaId)
        {
            _logger.LogInformation($"GetSubmarcas llamado con marcaId: {marcaId}");
            var submarcas = await _context.Marcas
                .Where(m => m.ParentId == marcaId)
                .Select(m => new { value = m.Id, text = m.Nombre })
                .ToListAsync();

            _logger.LogInformation($"Submarcas encontradas: {submarcas.Count}");
            return Json(submarcas);
        }

        // Métodos auxiliares
        private async Task<bool> ProductoExists(int id)
        {
            return await _context.Productos.AnyAsync(e => e.Id == id);
        }

        private void LoadViewData()
        {
            // Cargar rubros para el dropdown
            ViewData["Rubros"] = new SelectList(
                _context.Categoria.Where(c => c.Tipo == "R").OrderBy(c => c.Nombre),
                "Id",
                "Nombre");

            // Mostrar categorías (subrubros) con jerarquía
            var categorias = _context.Categoria
                .Where(c => c.Tipo == "S")
                .Include(c => c.Parent)
                .OrderBy(c => c.Parent.Nombre)
                .ThenBy(c => c.Nombre)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Parent != null ? $"{c.Parent.Nombre} > {c.Nombre}" : c.Nombre
                })
                .ToList();

            ViewData["Categorias"] = new SelectList(categorias, "Value", "Text");

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

            var nuevo = ultimo + 1;
            _logger.LogInformation($"Código numérico generado: {nuevo}");
            return nuevo;
        }

        private async Task<string> GenerarCodigoAlfanumerico(ProductoViewModel viewModel)
        {
            var categoria = await _context.Categoria.FindAsync(viewModel.CategoriaId);
            var marca = await _context.Marcas.FindAsync(viewModel.MarcaId);

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
                CodigoAlfaNum = viewModel.CodigoAlfaNum ?? string.Empty,
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
    }
}