using ASP.NETCore_WebAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCore_WebAPI.Context;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
   

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Produto> Produtos { get; set; }

}
