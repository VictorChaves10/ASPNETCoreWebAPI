using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Pagination;
using X.PagedList;

namespace ASP.NETCore_WebAPI.Repositories.Interfaces
{
    public interface ICategoriaRepository : IRepositoryBase<Categoria>
    {
        Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriaParameters categoria);
        Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriaFiltroNome categoria);
    }
}
