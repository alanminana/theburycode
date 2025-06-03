
// Services/Implementations/ClienteService.cs
using Microsoft.EntityFrameworkCore;
using theburycode.Models;

namespace theburycode.Services
{
    public class ClienteService : IClienteService
    {
        private readonly TheBuryCodeContext _context;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(TheBuryCodeContext context, ILogger<ClienteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _context.Clientes
                .Include(c => c.Ciudad)
                .OrderBy(c => c.Apellido)
                .ThenBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes
                .Include(c => c.Ciudad)
                .Include(c => c.DomicilioParticular)
                .Include(c => c.DomicilioLaboral)
                .Include(c => c.Conyuges)
                .Include(c => c.Garantes)
                .Include(c => c.DocumentoClientes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> GetByDniAsync(string dni)
        {
            return await _context.Clientes
                .Include(c => c.Ciudad)
                .FirstOrDefaultAsync(c => c.Dni == dni);
        }

        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            cliente.FechaAlta = DateTime.Now;
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cliente creado: {cliente.Id} - {cliente.Apellido}, {cliente.Nombre}");
            return cliente;
        }

        public async Task<Cliente> UpdateAsync(Cliente cliente)
        {
            cliente.FechaModificacion = DateTime.Now;
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cliente actualizado: {cliente.Id}");
            return cliente;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            // Verificar si tiene ventas o créditos
            var tieneVentas = await _context.Venta.AnyAsync(v => v.ClienteId == id);
            var tieneCreditos = await _context.SolicitudCreditos.AnyAsync(s => s.ClienteId == id);

            if (tieneVentas || tieneCreditos)
            {
                _logger.LogWarning($"No se puede eliminar cliente {id} - tiene ventas o créditos asociados");
                return false;
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Cliente eliminado: {id}");
            return true;
        }

        public async Task<bool> ExisteDni(string dni, int? excludeId = null)
        {
            var query = _context.Clientes.Where(c => c.Dni == dni);
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}