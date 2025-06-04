using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tazkartk.Application.Repository;

namespace Tazkartk.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;
        // private readonly DbSet<T> _dbSet;
        public GenericRepository(DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            //   _dbSet = context.Set<T>();
        }
        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public async Task AddRange(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T?> GetById(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(match);
        }
        public async Task<T?> GetById(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] Includes)
        {
            IQueryable<T> Query = _context.Set<T>();

            foreach (var include in Includes)
            {
                Query = Query.Include(include);
            }
            return await Query.FirstOrDefaultAsync(match);
        }
        public async Task<TDto?> GetById<TDto>(Expression<Func<T, bool>> match) where TDto : class
        {
            IQueryable<T> Query = _context.Set<T>();
            return await _context.Set<T>()
                .AsNoTracking()
                .Where(match)
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
        public async Task<List<TDto>> ProjectToList<TDto>(Expression<Func<T, bool>>? filter = null) where TDto : class
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            return await query.ProjectTo<TDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
