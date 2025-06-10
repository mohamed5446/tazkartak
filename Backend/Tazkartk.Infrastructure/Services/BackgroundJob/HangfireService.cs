using Hangfire;
using System.Linq.Expressions;
using Tazkartk.Application.Interfaces.External;

namespace Tazkartk.Infrastructure.BackgroundJob
{
    public class HangfireService : IBackgroundService
    {
  
        private readonly IBackgroundJobClient _jobClient;

        public HangfireService(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }
       
        public string AddSchedule<T>(Expression<Action<T>> methodcall, DateTimeOffset time) where T : class
        {
          
            var delay = time - DateTimeOffset.UtcNow;
            if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;
            return _jobClient.Schedule(methodcall, delay);
            // return _jobClient.Schedule(methodcall,TimeSpan.FromMinutes(time));
        }
        public void DeleteJob(string jobId)
        {
            //Hangfire.BackgroundJob.Delete(jobId);
            _jobClient.Delete(jobId);
        }
        public async Task DeleteExistingJobs(int tripId)
        {
            var monitor = JobStorage.Current.GetMonitoringApi();

            var jobsScheduled = monitor.ScheduledJobs(0, int.MaxValue);
            var tasks = new List<Task>();

            //.Where(x => x.Value.Job.Method.Name == "SendReminderEmail"||x.Value.Job.Method.Name== "MarkTripUnavailable");
            foreach (var j in jobsScheduled)
            {
                var job = j.Value.Job;
                if (job != null)
                {
                    var args = j.Value.Job.Args;
                    if (args != null)
                    {
                        if (args[0] is int t && t == tripId)
                        {
                            tasks.Add(Task.Run(() => Hangfire.BackgroundJob.Delete(j.Key)));
                            //Hangfire.BackgroundJob.Delete(j.Key);
                        }
                    }
                }
            }
            await Task.WhenAll(tasks);
        }
    }
}

