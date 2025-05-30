using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.TripDTOs;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Data;

namespace Tazkartk.Infrastructure.Repositories
{
    internal class TripRepository : GenericRepository<Trip>,ITripRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TripRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Trip>> AddRange(List<CreateTripDtos> trips,int companyId)
        {
            // var entities = _mapper.Map<List<Trip>>(trips);
            var entities = trips.Select(t=>new Trip
            {
                From=t.From,
                To=t.To,
                Time=t.Time,
                Class=t.Class,
                Price=t.Price,
                Date=t.Date,
                CompanyId=companyId,
                Location=t.Location,
                
            });
            await _context.BulkInsertAsync(entities);
            return entities.ToList();
          // await  _context.AddRangeAsync(entities);
           
          //  _mapper.Map<Trip>(TripDtos);
        }

        public async Task<IEnumerable<PassengerDetailsDTO>> GetPassengersAsync(int TripId)
        {
            var userPassengers = await _context.bookings
        .Where(b => b.tripId == TripId && !b.IsCanceled && b.user != null)
        .GroupBy(b => b.user)
        .Select(g => new PassengerDetailsDTO
        {
            FirstName = g.Key.FirstName,
            LastName = g.Key.LastName,
            Email = g.Key.Email,
            PhoneNumber = g.Key.PhoneNumber,
            Seats = g.SelectMany(b => b.seats.Select(s => s.Number)).Distinct().ToList()
        })
        .ToListAsync();

            var guestPassengers = await _context.bookings
                .Where(b => b.tripId == TripId && !b.IsCanceled && b.user == null)
                .GroupBy(b => new { b.GuestFirstName, b.GuestLastName, b.GuestPhoneNumber })
                .Select(g => new PassengerDetailsDTO
                {
                    FirstName = g.Key.GuestFirstName,
                    LastName = g.Key.GuestLastName,
                    PhoneNumber = g.Key.GuestPhoneNumber,
                    Email = null,
                    Seats = g.SelectMany(b => b.seats.Select(s => s.Number)).Distinct().ToList()
                })
                .ToListAsync();

            return userPassengers.Concat(guestPassengers);

            //return await _context.bookings
            //    .Where(b=>!b.IsCanceled&&b.tripId == TripId)
            //    .GroupBy(b=>b.user)
            //    .ProjectTo<PassengerDetailsDTO>(_mapper.ConfigurationProvider)
            //    .AsNoTracking()
            //    .ToListAsync();
            //return await _context.Trips
            //     .Where(t => t.TripId == TripId)
            //     .SelectMany(t => t.bookings)
            //     .Where(b => !b.IsCanceled)
            //     .ProjectTo<PassengerDetailsDTO>(_mapper.ConfigurationProvider)
            //     .AsNoTracking()
            //     .ToListAsync();
        }

       
        public async Task<Trip?> GetTripWithBookingAndUer(int TripId)
        {
            return await _context.Trips.Include(t => t.bookings).ThenInclude(b => b.user).FirstOrDefaultAsync(t => t.TripId == TripId);
        }
        

    }
}
