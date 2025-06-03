using theburycode.Models;

public interface IClienteService
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);  // Agregar ?
    Task<Cliente?> GetByDniAsync(string dni);  // Agregar ?
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente> UpdateAsync(Cliente cliente);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExisteDni(string dni, int? excludeId = null);
}