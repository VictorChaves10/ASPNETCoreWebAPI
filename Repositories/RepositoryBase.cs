using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ASP.NETCore_WebAPI.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly AppDbContext _context;

        public RepositoryBase(AppDbContext context) => _context = context;


        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking()
                                    .ToList();
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public T Create(T entity)
        {
            _context.Set<T>().Add(entity);            
            return entity;
        }
        public T Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;          
            return entity;
        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);   
            return entity;
        }

 
    }
}
