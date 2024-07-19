using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.Services;

namespace Project.HostedServices
{
    public class BackgroundRecommendationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public BackgroundRecommendationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // This can be used to start background tasks if needed.
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // This can be used to stop background tasks if needed.
            return Task.CompletedTask;
        }

        public async Task RunRecommendationTask(int userId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var recommendationService = scope.ServiceProvider.GetRequiredService<RecommendationService>();
                try
                {
                    await recommendationService.GetRecommendations(userId);
                }
                catch (Exception ex)
                {
                    // Log the exception (example using Serilog)
                    // Log.Error(ex, "Error occurred while generating recommendations for user {UserId}", userId);
                    throw; // Optional: Rethrow if you want to handle it further up the call stack
                }
            }
        }
    }
}
