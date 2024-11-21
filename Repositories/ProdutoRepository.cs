using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Pagination;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using X.PagedList;

namespace ASP.NETCore_WebAPI.Repositories
{
    public class ProdutoRepository : RepositoryBase<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext appDbContext) : base(appDbContext) { }

        public async Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters)
        {
            var produtos = await GetAllAsync();
            var produtosOrdenados = produtos.OrderBy(x => x.Nome).AsQueryable();
            var resultado = await produtosOrdenados.ToPagedListAsync(produtosParameters.PageNumber, produtosParameters.PageSize);

            return resultado;
        }

        public async Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutoFiltroPreco filtroPreco)
        {
            var produtos = await GetAllAsync();

            if (filtroPreco.Preco.HasValue && !string.IsNullOrEmpty(filtroPreco.PrecoCriterio))
            {
                if (filtroPreco.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                    produtos = produtos.Where(x => x.Preco >= filtroPreco.Preco.Value).OrderBy(x => x.Preco);

                if (filtroPreco.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                    produtos = produtos.Where(x => x.Preco <= filtroPreco.Preco.Value).OrderBy(x => x.Preco);

                if (filtroPreco.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                    produtos = produtos.Where(x => x.Preco == filtroPreco.Preco.Value).OrderBy(x => x.Preco);
            }

            var produtosFiltrados = await produtos.AsQueryable().ToPagedListAsync(filtroPreco.PageNumber, filtroPreco.PageSize);

            return produtosFiltrados;
        }
    }
}
