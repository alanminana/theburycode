// Services/Shared/ISearchService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services.Shared
{

    public class ClienteSearchDto
    {
        public int Id { get; set; }
        public string Dni { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Celular { get; set; }
        public int? Scoring { get; set; }
        public decimal? LimiteCredito { get; set; }
        public bool TieneDeuda { get; set; }
    }

    public class ProductoSearchDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Stock { get; set; }
        public decimal PrecioVenta { get; set; }
        public string? Categoria { get; set; }
        public string? Marca { get; set; }
        public string? ImagenUrl { get; set; }
    }

    public class ProveedorSearchDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Contacto { get; set; }
        public int CantidadProductos { get; set; }
    }
    public class SearchService : ISearchService
    {
        private readonly TheBuryCodeContext _context;
        private readonly IPrecioCalculatorService _precioCalculator;

        public SearchService(TheBuryCodeContext context, IPrecioCalculatorService precioCalculator)
        {
            _context = context;
            _precioCalculator = precioCalculator;
        }

        public async Task<List<ClienteSearchDto>> BuscarClientes(string termino)
        {
            termino = termino?.ToLower() ?? "";

            var clientes = await _context.Clientes
                .Where(c => c.Dni.Contains(termino) ||
           c.Nombre.ToLower().Contains(termino) ||
           c.Apellido.ToLower().Contains(termino) ||
           (c.Email != null && c.Email.ToLower().Contains(termino))).OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)  // AGREGAR ESTA LÍNEA

                .Select(c => new ClienteSearchDto
                {
                    Id = c.Id,
                    Dni = c.Dni,
                    NombreCompleto = c.Apellido + ", " + c.Nombre,
                    Email = c.Email,
                    Celular = c.Celular,
                    Scoring = c.Scoring,
                    TieneDeuda = c.SolicitudCreditos.Any(s =>
                        s.Estado == "A" && s.LineaCreditos.Any(l =>
                            l.Cuota.Any(cu => cu.Estado == "P" || cu.Estado == "V")))
                })
                .Take(10)
                .ToListAsync();

            return clientes;
        }

        public async Task<List<ProductoSearchDto>> BuscarProductos(string termino)
        {
            termino = termino?.ToLower() ?? "";

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.Activo == true &&
                           (p.CodigoAlfaNum.ToLower().Contains(termino) ||
                            p.Nombre.ToLower().Contains(termino) ||
                            p.Marca.Nombre.ToLower().Contains(termino))).OrderBy(p => p.Nombre)
                .Take(20)
                .ToListAsync();

            var result = new List<ProductoSearchDto>();
            foreach (var p in productos)
            {
                var precios = await _precioCalculator.GetPreciosProducto(p.Id);
                result.Add(new ProductoSearchDto
                {
                    Id = p.Id,
                    Codigo = p.CodigoAlfaNum,
                    Nombre = p.Nombre,
                    Stock = p.StockActual ?? 0,
                    PrecioVenta = precios?.PrecioContado ?? 0,
                    Categoria = p.Categoria?.Nombre ?? string.Empty,
                    Marca = p.Marca?.Nombre ?? string.Empty
                });
            }

            return result;
        }

        public async Task<ProductoSearchDto?> GetProductoPorCodigo(string codigo)

        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(p => p.CodigoAlfaNum == codigo && p.Activo == true);

            if (producto == null) return null;

            var precios = await _precioCalculator.GetPreciosProducto(producto.Id);

            return new ProductoSearchDto
            {
                Id = producto.Id,
                Codigo = producto.CodigoAlfaNum,
                Nombre = producto.Nombre,
                Stock = producto.StockActual ?? 0,
                PrecioVenta = precios?.PrecioContado ?? 0,
                Categoria = producto.Categoria?.Nombre ?? string.Empty,
                Marca = producto.Marca?.Nombre ?? string.Empty
            };
        }

        public async Task<List<ProveedorSearchDto>> BuscarProveedores(string termino)
        {
            termino = termino?.ToLower() ?? "";

            return await _context.Proveedors
               .Where(p => p.Activo == true &&
           (p.Nombre.ToLower().Contains(termino) ||
            (p.Contacto != null && p.Contacto.ToLower().Contains(termino))))
                .Select(p => new ProveedorSearchDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Contacto = p.Contacto ?? string.Empty,
                    CantidadProductos = p.ProveedorProductos.Count()
                })
                .Take(10)
                .ToListAsync();
        }
    }   
}