using System.Linq.Expressions;

namespace Tazkartk.Application.Repository
{
    public interface IGenericRepository<T>
    {
        Task<T> GetById(int id);
        Task<T?> GetById(Expression<Func<T, bool>> match);
        Task<T?> GetById(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] Includes);
        Task<IEnumerable<T>> GetAll();
        Task<List<TDto>> ProjectToList<TDto>(Expression<Func<T, bool>>? match = null) where TDto : class;
        Task<TDto?> GetById<TDto>(Expression<Func<T, bool>> match) where TDto : class;
        Task Add(T entity);
        Task AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        //Task<TDto?> GetById<TDto>(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] Includes) where TDto : class;

    }
}
