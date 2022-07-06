using BussinessLogic.Data;
using Core.Entities;
using Core.Interface;
using System.Collections;

namespace BussinessLogic.Logic
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;

        private readonly MarketDbContext _context;

        public UnitOfWork(MarketDbContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ClaseBase
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var respositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, respositoryInstance);
            }

            return (IGenericRepository<TEntity>) _repositories[type];
        }
    }
}
