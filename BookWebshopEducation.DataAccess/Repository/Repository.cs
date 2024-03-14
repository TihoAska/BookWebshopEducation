using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookWebshopEducation.DataAccess.Data;
using BookWebshopEducation.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BookWebshopEducation.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Add(T Entity)
        {
            dbSet.Add(Entity);
        }

        public void Delete(T Entity)
        {
            dbSet.Remove(Entity);
        }

        public void DeleteRange(IEnumerable<T> Entity)
        {
            dbSet.RemoveRange(Entity);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);

            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }
    }
}
