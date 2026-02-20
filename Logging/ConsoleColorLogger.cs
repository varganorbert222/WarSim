using System.Collections.Concurrent;

namespace WarSim.Logging
{
    public static class ConsoleColorLogger
    {
        private static bool _enabled = true;
        private static LogLevel _minimumLevel = LogLevel.Information;
        private static bool _batchEnabled = false;
        private static int _batchIntervalMs = 200;
        private static bool _disableAtomicLogs = false;

        private static readonly ConcurrentQueue<(ConsoleColor color, string message)> _queue = new();
        private static Timer? _timer;
        private static readonly Dictionary<string, bool> _categorySettings = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, LogLevel> _categoryMinimumLevels = new(StringComparer.OrdinalIgnoreCase);

        public static void Configure(IConfiguration config, string environment)
        {
            var section = config.GetSection("ConsoleLogging");
            _enabled = section.GetValue<bool?>("Enabled") ?? _enabled;
            var min = section.GetValue<string>("MinimumLevel");
            if (!string.IsNullOrEmpty(min) && Enum.TryParse<LogLevel>(min, true, out var ml))
            {
                _minimumLevel = ml;
            }

            _batchEnabled = section.GetValue<bool?>("BatchEnabled") ?? (_batchEnabled || environment.Equals("Production", StringComparison.OrdinalIgnoreCase));
            _batchIntervalMs = section.GetValue<int?>("BatchIntervalMs") ?? _batchIntervalMs;
            _disableAtomicLogs = section.GetValue<bool?>("DisableAtomicLogs") ?? _disableAtomicLogs;

            if (_batchEnabled)
            {
                _timer ??= new Timer(_ => Flush(), null, _batchIntervalMs, _batchIntervalMs);
            }

            // read category settings (enable/disable)
            var catSection = config.GetSection("ConsoleLogging:Categories");
            if (catSection.Exists())
            {
                foreach (var child in catSection.GetChildren())
                {
                    if (bool.TryParse(child.Value, out var v))
                    {
                        _categorySettings[child.Key] = v;
                    }
                }
            }

            // read category-specific minimum levels (e.g. Simulation=Debug, Weapon=Information)
            var catLevelSection = config.GetSection("ConsoleLogging:CategoryLevels");
            if (catLevelSection.Exists())
            {
                foreach (var child in catLevelSection.GetChildren())
                {
                    if (!string.IsNullOrEmpty(child.Value) && Enum.TryParse<LogLevel>(child.Value, true, out var lv))
                    {
                        _categoryMinimumLevels[child.Key] = lv;
                    }
                }
            }
        }

        public static void Log(string category, LogLevel level, string message, bool atomic = false)
        {
            if (!_enabled)
            {
                return;
            }

            // global minimum level check
            if (level < _minimumLevel)
            {
                return;
            }

            // category-specific minimum level (if configured)
            LogLevel? categoryMin = null;
            // pick the most specific matching key (longest)
            var candidate = _categoryMinimumLevels
                .Where(kv => category.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(kv => kv.Key.Length)
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(candidate.Key))
            {
                categoryMin = candidate.Value;
            }

            if (categoryMin.HasValue && level < categoryMin.Value)
            {
                return;
            }

            if (atomic && _disableAtomicLogs)
            {
                return;
            }

            var color = GetColorForCategory(category, level);
            var line = $"[{DateTime.UtcNow:O}] [{level}] [{category}] {message}";

            if (_batchEnabled)
            {
                _queue.Enqueue((color, line));
            }
            else
            {
                var original = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line);
                Console.ForegroundColor = original;
            }
        }

        private static void Flush()
        {
            while (_queue.TryDequeue(out var item))
            {
                var (color, line) = item;
                var original = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(line);
                Console.ForegroundColor = original;
            }
        }

        private static ConsoleColor GetColorForCategory(string category, LogLevel level)
        {
            if (category.Contains("Combat", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Red;
            }

            if (category.Contains("Simulation", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Yellow;
            }

            if (category.Contains("AI", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Magenta;
            }

            if (category.Contains("Weapon", StringComparison.OrdinalIgnoreCase) || category.Contains("Projectile", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Red;
            }

            if (category.Contains("World", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Cyan;
            }

            if (category.Contains("API", StringComparison.OrdinalIgnoreCase) || category.Contains("Controllers", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Green;
            }

            if (level is LogLevel.Error or LogLevel.Critical)
            {
                return ConsoleColor.Red;
            }

            if (level == LogLevel.Warning)
            {
                return ConsoleColor.Yellow;
            }

            return ConsoleColor.White;
        }
    }
}
