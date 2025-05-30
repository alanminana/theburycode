// Services/Interfaces/IClienteService.cs
using theburycode.Models;

namespace theburycode.Services
{
    public interface IClienteService
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente> GetByIdAsync(int id);
        Task<Cliente> GetByDniAsync(string dni);
        Task<Cliente> CreateAsync(Cliente cliente);
        Task<Cliente> UpdateAsync(Cliente cliente);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExisteDni(string dni, int? excludeId = null);
    }
}