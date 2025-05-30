using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO.UserDTOs;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Data;

namespace Tazkartk.Infrastructure.Repositories
{
    internal class UserRepository : GenericRepository<User>,IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public UserRepository( ApplicationDbContext context, IMapper mapper) :base(context, mapper  )
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<User>GetUserWtihBookingDetails(int userId)
        {
            return  await _context.Users
               .Include(u => u.books)
                   .ThenInclude(b => b.seats)
               .Include(u => u.books)
                   .ThenInclude(b => b.trip)
            .ThenInclude(t => t.company)
               .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<User?> GetUserWithBookingandTrip(int id)
        {
            return await _context.Users.Include(u => u.books).ThenInclude(v => v.trip).FirstOrDefaultAsync(u => u.Id == id);
        }

       
    }
}
