using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Data;

namespace Tazkartk.Infrastructure.Repositories
{
    internal class TicketRepository: GenericRepository<Booking>, ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TicketRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public void DeleteTicket(Booking Ticket)
        {
            _context.Seats.RemoveRange(Ticket.seats);
            _context.Payments.Remove(Ticket.payment);
            _context.bookings.Remove(Ticket);
        }

     
    }
}
