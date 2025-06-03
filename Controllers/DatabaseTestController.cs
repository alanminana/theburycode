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
        [HttpGet]
        public async Task<IActionResult> TestAllTables()
        {
            var results = new Dictionary<string, object>();
            var errors = new List<string>();
            var allTables = new List<string>();
            var missingTables = new List<string>();

            try
            {
                // Lista de todas las tablas esperadas
                var expectedTables = new[]
                {
            "Genero", "EstadoCivil", "EstadoEntrega", "EstadoCompra", "EstadoProducto",
            "EstadoUsuario", "EstadoSolicitudCredito", "EstadoRMA", "Provincia", "Ciudad",
            "Banco", "FormaPago", "TipoTarjeta", "SysModulo", "Cliente", "Producto",
            "Categoria", "Marca", "Proveedor", "Venta", "Compra", "Cotizacion",
            "SolicitudCredito", "Usuario", "Rol", "Permiso", "Conyuge", "Garante",
            "DocumentoCliente", "DomicilioParticular", "DomicilioLaboral", "VentaDetalle",
            "CompraDetalle", "CotizacionDetalle", "StockLog", "PrecioLog", "ScoringLog",
            "Configuracion", "Alerta", "AlertaLog", "ArchivoCredito", "LineaCredito",
            "Cuota", "ProveedorProducto", "UsuarioRol", "RolPermiso", "RMACliente",
            "RMAClienteDetalle", "RMAProveedor"
        };

                // Verificar cada tabla
                foreach (var tabla in expectedTables)
                {
                    try
                    {
                        var count = await _context.Database.ExecuteSqlAsync($"SELECT COUNT(*) FROM {tabla}");
                        allTables.Add(tabla);
                        results[tabla] = count;
                    }
                    catch
                    {
                        missingTables.Add(tabla);
                        results[tabla] = -1;
                    }
                }

                return Json(new
                {
                    success = true,
                    totalTables = allTables.Count,
                    allTables = allTables,
                    missingTables = missingTables,
                    results = results,
                    message = missingTables.Count == 0
                        ? "Todas las tablas verificadas correctamente"
                        : $"Faltan {missingTables.Count} tablas"
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
                    allTables = allTables,
                    missingTables = missingTables,
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

                // Insertar marcas
                var marca = new Marca
                {
                    Nombre = "Samsung",
                    Tipo = "M",
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Marcas.Add(marca);

                var marca2 = new Marca
                {
                    Nombre = "LG",
                    Tipo = "M",
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Marcas.Add(marca2);
                await _context.SaveChangesAsync();

                // Insertar múltiples clientes
                var clientesCreados = new List<int>();
                for (int i = 1; i <= 5; i++)
                {
                    var cliente = new Cliente
                    {
                        Nombre = i == 1 ? "Juan" : $"Cliente{i}",
                        Apellido = i == 1 ? "Pérez" : $"Test{i}",
                        Dni = $"1234567{i}",
                        Genero = i % 2 == 0 ? "F" : "M",
                        EstadoCivil = i % 3 == 0 ? "C" : "S",
                        Email = $"cliente{i}@test.com",
                        Celular = $"112233445{i}",
                        Telefono = $"4444555{i}",
                        CiudadId = 1, // Asume que existe La Plata
                        Scoring = 5 + i,
                        ContactoEmergencia = $"Contacto {i}: 911",
                        UsuarioAlta = "TEST",
                        FechaAlta = DateTime.Now
                    };
                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();
                    clientesCreados.Add(cliente.Id);
                }

                // Insertar proveedores
                var proveedor = new Proveedor
                {
                    Nombre = "Distribuidora Test SA",
                    Contacto = "ventas@test.com",
                    Activo = true,
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Proveedors.Add(proveedor);

                var proveedor2 = new Proveedor
                {
                    Nombre = "Importadora Buenos Aires SRL",
                    Contacto = "compras@importba.com",
                    Activo = true,
                    UsuarioAlta = "TEST",
                    FechaAlta = DateTime.Now
                };
                _context.Proveedors.Add(proveedor2);
                await _context.SaveChangesAsync();

                // Insertar múltiples productos
                var productosCreados = new List<int>();
                var nombresProductos = new[] {
            "Heladera Samsung 360L",
            "Lavarropas LG 8kg",
            "Microondas Samsung 20L",
            "Aire Acondicionado LG 3000",
            "Freezer Vertical Samsung"
        };

                for (int i = 0; i < 5; i++)
                {
                    var producto = new Producto
                    {
                        CodigoNum = 1001 + i,
                        CodigoAlfaNum = $"PROD-{1001 + i}",
                        Nombre = nombresProductos[i],
                        CategoriaId = subrubro.Id,
                        MarcaId = i % 2 == 0 ? marca.Id : marca2.Id,
                        PrecioCosto = 150000 + (50000 * i),
                        MargenVentaPct = 30,
                        DescuentoContadoPct = 10,
                        IvaPct = 21,
                        StockActual = 10 + (i * 2),
                        StockMinimo = 2,
                        Activo = true,
                        EstadoProducto = "A",
                        UsuarioAlta = "TEST",
                        FechaAlta = DateTime.Now
                    };
                    _context.Productos.Add(producto);
                    await _context.SaveChangesAsync();
                    productosCreados.Add(producto.Id);

                    // Asociar producto con proveedor
                    _context.ProveedorProductos.Add(new ProveedorProducto
                    {
                        ProductoId = producto.Id,
                        ProveedorId = i % 2 == 0 ? proveedor.Id : proveedor2.Id,
                        PrecioCosto = producto.PrecioCosto,
                        PlazoEntrega = 7,
                        UsuarioAlta = "TEST",
                        FechaAlta = DateTime.Now
                    });
                }

                // Insertar formas de pago
                var formasPago = new[] { "Efectivo", "Tarjeta Débito", "Tarjeta Crédito", "Transferencia" };
                foreach (var fp in formasPago)
                {
                    _context.FormaPagos.Add(new FormaPago
                    {
                        Descripcion = fp,
                        UsuarioAlta = "TEST",
                        FechaAlta = DateTime.Now
                    });
                }

                // Insertar bancos
                var bancos = new[] { "Banco Nación", "Banco Provincia", "Santander", "BBVA" };
                foreach (var banco in bancos)
                {
                    _context.Bancos.Add(new Banco
                    {
                        Descripcion = banco,
                        UsuarioAlta = "TEST",
                        FechaAlta = DateTime.Now
                    });
                }

                // Insertar tipos de tarjeta
                var tiposTarjeta = new[] { "Visa", "Mastercard", "American Express" };
                foreach (var tipo in tiposTarjeta)
                {
                    _context.TipoTarjeta.Add(new TipoTarjetum
                    {
                        Descripcion = tipo,
                        UsuarioAlta = "TEST",
                        FechaAlta = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Datos de prueba insertados correctamente",
                    resumen = new
                    {
                        clientes = clientesCreados.Count,
                        productos = productosCreados.Count,
                        proveedores = 2,
                        formasPago = formasPago.Length,
                        bancos = bancos.Length
                    }
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