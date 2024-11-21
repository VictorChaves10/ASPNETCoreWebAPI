using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Pagination;
using X.PagedList;

namespace ASP.NETCore_WebAPI.Repositories.Interfaces
{
    public interface IProdutoRepository : IRepositoryBase<Produto>
    {
        Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters);    
        Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutoFiltroPreco filtroPreco);     
    }
}
