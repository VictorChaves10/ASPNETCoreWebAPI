using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Pagination;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using X.PagedList;

namespace ASP.NETCore_WebAPI.Repositories;

public class CategoriaRepository : RepositoryBase<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context) { }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriaParameters categoriaParameters)
    {
        var categorias = await GetAllAsync();
        var categoriasOrdenadas = categorias.OrderBy(x => x.Nome).AsQueryable();
        var resultado = await categoriasOrdenadas.ToPagedListAsync(categoriaParameters.PageNumber, categoriaParameters.PageSize);
        
        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriaFiltroNome categoriaFiltro)
    {
        var categorias = await GetAllAsync();
        
        if (!string.IsNullOrEmpty(categoriaFiltro.Nome))
        {
            categorias = categorias.Where(c => c.Nome.Contains(categoriaFiltro.Nome, StringComparison.CurrentCultureIgnoreCase));
        }

        var categoriasFiltradas = await categorias.ToPagedListAsync(categoriaFiltro.PageNumber, categoriaFiltro.PageSize);

        return categoriasFiltradas;
    }
}
