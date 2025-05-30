using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using theburycode.Models;
using System.Text;

namespace theburycode.Controllers
{
    public class DatabaseTestController : Controller
    {
        private readonly TheBuryCodeContext _context;
        private readonly ILogger<DatabaseTestController> _logger;

        public DatabaseTestController(TheBuryCodeContext context, ILogger<DatabaseTestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Vista principal de prueba
        public IActionResult Index()
        {
            return View();
        }

        // Test de conexión básica
        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return Json(new { success = canConnect, message = canConnect ? "Conexión exitosa" : "No se pudo conectar" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar conexión");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Test completo de todas las tablas
        [HttpGet]
        public async Task<IActionResult> TestAllTables()
        {
            var results = new Dictionary<string, object>();
            var errors = new List<string>();

            try
            {
                // Test tablas de dominio
                results["Generos"] = await _context.Generos.CountAsync();
                results["EstadosCiviles"] = await _context.EstadoCivils.CountAsync();
                results["EstadosEntrega"] = await _context.EstadoEntregas.CountAsync();
                results["EstadosCompra"] = await _context.EstadoCompras.CountAsync();
                results["EstadosProducto"] = await _context.EstadoProductos.CountAsync();
                results["EstadosUsuario"] = await _context.EstadoUsuarios.CountAsync();
                results["EstadosCredito"] = await _context.EstadoSolicitudCreditos.CountAsync();
                results["EstadosRMA"] = await _context.EstadoRmas.CountAsync();

                // Test tablas maestras
                results["Provincias"] = await _context.Provincia.CountAsync();
                results["Ciudades"] = await _context.Ciudads.CountAsync();
                results["Bancos"] = await _context.Bancos.CountAsync();
                results["FormasPago"] = await _context.FormaPagos.CountAsync();
                results["TiposTarjeta"] = await _context.TipoTarjeta.CountAsync();
                results["Modulos"] = await _context.SysModulos.CountAsync();

                // Test módulos principales
                results["Clientes"] = await _context.Clientes.CountAsync();
                results["Productos"] = await _context.Productos.CountAsync();
                results["Categorias"] = await _context.Categoria.CountAsync();
                results["Marcas"] = await _context.Marcas.CountAsync();
                results["Proveedores"] = await _context.Proveedors.CountAsync();
                results["Ventas"] = await _context.Venta.CountAsync();
                results["Compras"] = await _context.Compras.CountAsync();
                results["Cotizaciones"] = await _context.Cotizacions.CountAsync();
                results["SolicitudesCredito"] = await _context.SolicitudCreditos.CountAsync();
                results["Usuarios"] = await _context.Usuarios.CountAsync();
                results["Roles"] = await _context.Rols.CountAsync();

                return Json(new
                {
                    success = true,
                    totalTables = results.Count,
                    results = results,
                    message = "Todas las tablas verificadas correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar tablas");
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    results = results
                });
            }
        }

        // Test de integridad referencial
        [HttpGet]
        public async Task<IActionResult> TestRelationships()
        {
            var tests = new List<object>();

            try
            {
                // Test Cliente -> Ciudad
                var clienteConCiudad = await _context.Clientes
                    .Include(c => c.Ciudad)
                    .FirstOrDefaultAsync();
                tests.Add(new
                {
                    test = "Cliente-Ciudad",
                    success = clienteConCiudad?.Ciudad != null,
                    message = clienteConCiudad?.Ciudad != null ? "OK" : "Sin datos"
                });

                // Test Producto -> Categoria/Marca
                var producto = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Marca)
                    .FirstOrDefaultAsync();
                tests.Add(new
                {
                    test = "Producto-Categoria-Marca",
                    success = producto != null,
                    hasCategoria = producto?.Categoria != null,
                    hasMarca = producto?.Marca != null
                });

                // Test Venta -> Cliente
                var venta = await _context.Venta
                    .Include(v => v.Cliente)
                    .Include(v => v.FormaPago)
                    .FirstOrDefaultAsync();
                tests.Add(new
                {
                    test = "Venta-Cliente",
                    success = venta != null,
                    hasCliente = venta?.Cliente != null,
                    hasFormaPago = venta?.FormaPago != null
                });

                return Json(new { success = true, tests = tests });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message, tests = tests });
            }
        }

        // Insertar datos de prueba
        [HttpPost]
        public async Task<IActionResult> InsertTestData()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verificar si ya hay datos
                if (await _context.Clientes.AnyAsync())
                {
                    return Json(new { success = false, message = "Ya existen datos en la BD" });
                }

                // Insertar categorías de prueba
                var rubro = new Categoria
                {
                    Nombre = "Electrodomésticos",
                    Tipo = "R",
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Categoria.Add(rubro);
                await _context.SaveChangesAsync();

                var subrubro = new Categoria
                {
                    Nombre = "Refrigeración",
                    Tipo = "S",
                    ParentId = rubro.Id,
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Categoria.Add(subrubro);

                // Insertar marca
                var marca = new Marca
                {
                    Nombre = "Samsung",
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Marcas.Add(marca);
                await _context.SaveChangesAsync();

                // Insertar cliente de prueba
                var cliente = new Cliente
                {
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Dni = "12345678",
                    Genero = "M",
                    EstadoCivil = "S",
                    Email = "juan@test.com",
                    Celular = "1122334455",
                    CiudadId = 1, // Asume que existe La Plata
                    Scoring = 8,
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Clientes.Add(cliente);

                // Insertar proveedor
                var proveedor = new Proveedor
                {
                    Nombre = "Distribuidora Test SA",
                    Contacto = "ventas@test.com",
                    Activo = true,
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Proveedors.Add(proveedor);

                // Insertar producto
                var producto = new Producto
                {
                    CodigoNum = 1001,
                    CodigoAlfaNum = "PROD-1001",
                    Nombre = "Heladera Samsung 360L",
                    CategoriaId = subrubro.Id,
                    MarcaId = marca.Id,
                    PrecioCosto = 150000,
                    MargenVentaPct = 30,
                    DescuentoContadoPct = 10,
                    IvaPct = 21,
                    StockActual = 10,
                    StockMinimo = 2,
                    Activo = true,
                    EstadoProducto = "A",
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Productos.Add(producto);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Datos de prueba insertados correctamente",
                    clienteId = cliente.Id,
                    productoId = producto.Id
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al insertar datos de prueba");
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // Consulta de prueba con LINQ
        [HttpGet]
        public async Task<IActionResult> TestQueries()
        {
            try
            {
                var queries = new Dictionary<string, object>();

                // Clientes con scoring alto
                queries["ClientesScoringAlto"] = await _context.Clientes
                    .Where(c => c.Scoring >= 7)
                    .Select(c => new { c.Id, c.Nombre, c.Apellido, c.Scoring })
                    .ToListAsync();

                // Productos con stock bajo
                queries["ProductosStockBajo"] = await _context.Productos
                    .Where(p => p.StockActual < p.StockMinimo)
                    .Select(p => new { p.Id, p.Nombre, p.StockActual, p.StockMinimo })
                    .ToListAsync();

                // Ventas del mes
                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                queries["VentasDelMes"] = await _context.Venta
                    .Where(v => v.Fecha >= inicioMes)
                    .CountAsync();

                // Top 5 productos más vendidos
                queries["TopProductos"] = await _context.VentaDetalles
                    .GroupBy(vd => new { vd.ProductoId, vd.Producto.Nombre })
                    .Select(g => new {
                        ProductoId = g.Key.ProductoId,
                        Nombre = g.Key.Nombre,
                        CantidadVendida = g.Sum(x => x.Cantidad)
                    })
                    .OrderByDescending(x => x.CantidadVendida)
                    .Take(5)
                    .ToListAsync();

                return Json(new { success = true, queries = queries });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}