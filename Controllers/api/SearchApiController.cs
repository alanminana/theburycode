using Microsoft.AspNetCore.Mvc;
using theburycode.Services.Shared;

namespace theburycode.Controllers.Api
{
    [ApiController]
    [Route("api/search")]
    public class SearchApiController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchApiController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("clientes")]
        public async Task<IActionResult> BuscarClientes(string termino)
        {
            var clientes = await _searchService.BuscarClientes(termino);
            return Ok(clientes);
        }

        [HttpGet("productos")]
        public async Task<IActionResult> BuscarProductos(string termino)
        {
            var productos = await _searchService.BuscarProductos(termino);
            return Ok(productos);
        }
    }
}