using Core.Specifications;
using Microsoft.AspNetCore.Identity;

namespace Core.Interface
{
    public interface IGenericSeguridadRepository<T> where T : IdentityUser
    {
        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T> GetByIdWithSpec(ISpecification<T> spec);

        Task<IReadOnlyList<T>> GetAllWithSpec(ISpecification<T> spec);

        Task<int> CountAsync(ISpecification<T> spec);

        Task<int> Add(T entity);

        Task<int> Update(T entity);
    }
}
