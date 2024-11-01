namespace ASP.NETCore_WebAPI.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoriaRepository CategoriaRepository { get; }

        IProdutoRepository ProdutoRepository { get; }

        void Commit();
    }
}
