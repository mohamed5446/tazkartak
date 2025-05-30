using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.Interfaces
{
    public interface IBackgroundService
    {
        string AddSchedule<T>(Expression<Action<T>> methodcall, DateTimeOffset time) where T : class;
        Task DeleteExistingJobs(int Id);
        void DeleteJob(string jobId);

    }
}
