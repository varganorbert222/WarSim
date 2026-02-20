using WarSim.Simulation;

namespace WarSim.Services
{
    public class SimulationHostedService : BackgroundService
    {
        private readonly ISimulationEngine _engine;
        private readonly ILogger<SimulationHostedService> _logger;
        private readonly int _tickMs;

        public SimulationHostedService(ISimulationEngine engine, ILogger<SimulationHostedService> logger, IConfiguration configuration)
        {
            _engine = engine;
            _logger = logger;

            _tickMs = configuration.GetValue<int?>("Simulation:TickMs") ?? 100; // default 100 ms
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SimulationHostedService starting with tick {TickMs} ms", _tickMs);

            var tickSeconds = _tickMs / 1000.0;

            while (!stoppingToken.IsCancellationRequested)
            {
                var start = DateTime.UtcNow;
                try
                {
                    await _engine.TickAsync(tickSeconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during simulation tick");
                }

                var elapsed = DateTime.UtcNow - start;
                var delay = TimeSpan.FromMilliseconds(_tickMs) - elapsed;
                if (delay > TimeSpan.Zero)
                {
                    try
                    {
                        await Task.Delay(delay, stoppingToken);
                    }
                    catch (TaskCanceledException) { }
                }
            }

            _logger.LogInformation("SimulationHostedService stopping");
        }
    }
}
