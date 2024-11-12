using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Pagination;

namespace ASP.NETCore_WebAPI.Repositories.Interfaces
{
    public interface ICategoriaRepository : IRepositoryBase<Categoria>
    {
        PagedList<Categoria> GetCategorias(CategoriaParameters categoria);

    }
}
