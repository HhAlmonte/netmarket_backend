using BussinessLogic.Data;
using Core.Interface;
using Core.Specifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogic.Logic
{
    public class GenericSeguridadRepository<T> : IGenericSeguridadRepository<T> where T : IdentityUser
    {
        private readonly SeguridadDbContext _context;

        public GenericSeguridadRepository(SeguridadDbContext context)
        {
            _context = context;
        }

        //Metodos sin especificaciones de consultas

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        //Metodos con especificaciones de consultas

        public async Task<T> GetByIdWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpec(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }


        //Creamos un metodo que aplique las especificaciones
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SeguridadSpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        //Metodos para agragar y modificar

        public async Task<int> Add(T entity)
        {
            _context.Set<T>().Add(entity);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            _context.Set<T>().Attach(entity);

            _context.Entry(entity).State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }
    }
}
