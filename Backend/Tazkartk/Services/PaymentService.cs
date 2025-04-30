using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Tazkartk.Data;
using Tazkartk.DTO;
using Tazkartk.DTO;
using Tazkartk.Interfaces;

namespace Tazkartk.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public PaymentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PaymentDTO>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .AsNoTracking()
                .ProjectTo<PaymentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
