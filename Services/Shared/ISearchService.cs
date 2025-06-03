namespace theburycode.Services.Shared
{
    public interface ISearchService
    {
        Task<List<ClienteSearchDto>> BuscarClientes(string termino);
        Task<List<ProductoSearchDto>> BuscarProductos(string termino);
        Task<ProductoSearchDto?> GetProductoPorCodigo(string codigo);
        Task<List<ProveedorSearchDto>> BuscarProveedores(string termino);
    }    
}