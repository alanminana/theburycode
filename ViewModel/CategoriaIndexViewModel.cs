using theburycode.Models;

namespace theburycode.ViewModels
{
    public class CategoriaIndexViewModel
    {
        public List<Categoria> Categorias { get; set; } = new List<Categoria>();
        public List<Marca> Marcas { get; set; } = new List<Marca>();
    }
}