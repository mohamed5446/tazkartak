using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Infrastructure.Data;
using Tazkartk.Application.Interfaces;
using Tazkartk.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Tazkartk.Application.Repository;

namespace Tazkartk.Infrastructure.Repositories
{
    public  class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private  IDbContextTransaction _transaction;
       

        public IUserRepository Users {get;private set;}

        public ITripRepository Trips { get;private set;}

        public ICompanyRepository Companies { get;private set;}

        public ITicketRepository Bookings { get;private set;}

        public IPaymentRepository Payments {get;private set;}
        public IMessagesRepository Messages { get;private set;}
        public IPayoutsRepository Payouts {  get;private set;}

       
        public UnitOfWork(ApplicationDbContext context,IMapper _mapper)
        {
            _context = context;
            // _repositories = new Dictionary<Type, object>();
            Users = new UserRepository(_context, _mapper);
           Trips=new TripRepository(_context, _mapper);
            Companies=new CompanyRepository(_context,_mapper);
            Bookings=new TicketRepository(_context,_mapper);
            Payments=new PaymentRepository(_context,_mapper);
            Messages = new MessagesRepository(_context, _mapper);
            Payouts=new PayoutsRepository(_context,_mapper);


        }

        //public IGenericRepository<T> Repository<T>() where T : class
        //{
        //    var type = typeof(T);
        //    if (!_repositories.ContainsKey(type))
        //    {
        //        var repo = new GenericRepository<T>(_context);
        //        _repositories.Add(type, repo);
        //    }
        //    return (IGenericRepository<T>)_repositories[type];
        //}
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null) 
                 _transaction= await _context.Database.BeginTransactionAsync();
        }

        public async Task commitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }

        }

        public async Task RollBackAsdync()
        {
            await _transaction.RollbackAsync();
        }
    }
}
