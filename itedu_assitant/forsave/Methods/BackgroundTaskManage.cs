using itedu_assitant.forsave.Contact_is;

namespace itedu_assitant.forsave.Methods
{
    public class BackgroundTaskManage : IHostedService
    {

        private Timer _timer;
        private TimerCallback _method;
        private Dictionary<string, object> _args;
        private IServiceProvider _services;

       public BackgroundTaskManage(IServiceProvider service)
       {
            _services = service;
       }

       public BackgroundTaskManage(TimerCallback method, Dictionary<string, object> args = null)
       {
            _method = method;
            _args = args != null ? args : new Dictionary<string, object>();
       }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _args["cttoken"] = cancellationToken;
            _timer = new Timer(_method, _args, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _timer.Dispose();
            return Task.CompletedTask;
        }
    }
}
