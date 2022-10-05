using MCServerManager.Service;

namespace MCServerManager.Service.HostedService
{
	public class RunGameServersHostedService : BackgroundService
	{
		public RunGameServersHostedService(ServerService serverService)
		{

		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return Task.CompletedTask;
		}
	}
}
