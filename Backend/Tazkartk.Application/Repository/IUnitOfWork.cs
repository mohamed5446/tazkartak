using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users {  get; }
        ITripRepository Trips { get; }
        ICompanyRepository Companies { get; }
        ITicketRepository Bookings { get; }
        IPaymentRepository Payments { get; }
        // public IGenericRepository<T> Repository<T>() where T : class;
        Task BeginTransactionAsync();
        Task commitTransactionAsync();
        Task RollBackAsdync();

        Task<int> CompleteAsync();
    }
}
