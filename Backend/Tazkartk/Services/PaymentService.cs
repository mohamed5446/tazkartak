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

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentDTO>> GetAllPayments()
        {
            var arabicCulture = new CultureInfo("ar-EG");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            return await _context.Payments.AsNoTracking().Select( p =>new PaymentDTO
            {
                UserId=p.booking.UserId,
                UserName=p.booking.user.FirstName,
                UserEmail=p.booking.user.Email,
                PaymentId=p.PaymentId,
                PaymentIntentId=p.PaymentIntentId,
                time=p.Date.ToString("dddd yyyy-MM-dd HH:mm tt",arabicCulture),
                Method=p.Method,
                Amount=p.amount,
                SeatNumbers=p.booking.seats.Select(s=>s.Number).ToList(),
                CompanyName=p.booking.trip.company.Name,
                IsRefunded=p.IsRefunded
              
            }).ToListAsync();
        }

        public Task<PaymentDTO> GetPaymentById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
