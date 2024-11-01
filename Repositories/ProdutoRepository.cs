using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Repositories.Interfaces;

namespace ASP.NETCore_WebAPI.Repositories
{
    public class ProdutoRepository : RepositoryBase<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext appDbContext) : base(appDbContext) { }

 

    }
}
