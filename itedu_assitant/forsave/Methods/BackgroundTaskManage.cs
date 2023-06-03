namespace itedu_assitant.forsave.Methods
{
    public class BackgroundTaskManage : IHostedService
    {

        private Timer _timer;
        public CheckActive currentActiveInstance;
        

       public BackgroundTaskManage(CheckActive cins)
       {
            currentActiveInstance = cins;
       }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            _timer = new Timer(currentActiveInstance.BaseWriter, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }
    }
}
