using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Repositories.Interfaces;

namespace ASP.NETCore_WebAPI.Repositories;

public class CategoriaRepository : RepositoryBase<Categoria>, ICategoriaRepository 
{
    public CategoriaRepository(AppDbContext context) : base(context) { }

}
