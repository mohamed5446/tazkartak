using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Tazkartk.Application.DTO;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Data;

namespace Tazkartk.Infrastructure.Repositories
{
    internal class PaymentRepository : GenericRepository<Payment>,IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PaymentRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        public async Task<IReadOnlyList<PaymentDTO>> GetAllPaymentsAsync()
        {

            return await _context.Payments
                .AsNoTracking()
                .ProjectTo<PaymentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

      
    }
}
