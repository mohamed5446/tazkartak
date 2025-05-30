using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Repository
{
    public interface ITicketRepository:IGenericRepository<Booking>
    {    
       void DeleteTicket(Booking Ticket);

    }
}
