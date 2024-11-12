using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Pagination;

namespace ASP.NETCore_WebAPI.Repositories.Interfaces
{
    public interface IProdutoRepository : IRepositoryBase<Produto>
    {
        PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters);

    }
}
