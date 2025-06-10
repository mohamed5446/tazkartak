using AutoMapper;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Data;
namespace Tazkartk.Infrastructure.Repositories
{
    public class MessagesRepository : GenericRepository<Message>, IMessagesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MessagesRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
