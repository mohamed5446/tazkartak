using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Data;

namespace Tazkartk.Infrastructure.Repositories
{
    public class PayoutsRepository : GenericRepository<Payout>, IPayoutsRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public PayoutsRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
   
}
