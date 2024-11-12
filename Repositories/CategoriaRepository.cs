using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Pagination;
using ASP.NETCore_WebAPI.Repositories.Interfaces;

namespace ASP.NETCore_WebAPI.Repositories;

public class CategoriaRepository : RepositoryBase<Categoria>, ICategoriaRepository 
{
    public CategoriaRepository(AppDbContext context) : base(context) { }

    public PagedList<Categoria> GetCategorias(CategoriaParameters categoriaParameters)
    {
        var categorias = GetAll().OrderBy(x => x.Nome).AsQueryable();

        var pagination = PagedList<Categoria>.ToPagedList(categorias, categoriaParameters.PageNumber, categoriaParameters.PageSize);

        return pagination;

    }
}
