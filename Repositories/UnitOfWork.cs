using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Repositories.Interfaces;

namespace ASP.NETCore_WebAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{

    private  IProdutoRepository _produtoRepository;
    private  ICategoriaRepository _categoriaRepository;

    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICategoriaRepository CategoriaRepository
    {
        get
        {
            return _categoriaRepository = _categoriaRepository ?? new CategoriaRepository(_context); 
        }
    }

    public IProdutoRepository ProdutoRepository
    {
        get 
        {
            return _produtoRepository = _produtoRepository ?? new ProdutoRepository(_context);           
        }
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
