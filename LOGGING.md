LOGGING
=======

This document describes the console logging system used by the WarSim backend, the configuration keys available in `appsettings.*.json`, and recommended settings for development and production.

Overview
--------
We use a lightweight colored console logger (`WarSim.Logging.ConsoleColorLogger`) in addition to the normal `ILogger<T>` pipeline. The console logger supports:

- Colored output per logical category (Simulation, AI, Weapon, World, API, Combat, etc.).
- Global and per-category minimum log levels.
- Batched console writes to reduce I/O overhead in high-throughput environments.
- "Atomic" logs: very high-frequency, low-value logs can be marked atomic and suppressed in production.

Log Categories
--------------
The system uses several logical categories to organize logs:

- **SimulationEngine**: High-level tick lifecycle (start/end of simulation tick, summary statistics)
- **Combat**: Game-relevant combat events (unit fires weapon, projectile hits, unit destroyed)
- **AI**: AI decision-making (target selection, tactical decisions)
- **Weapon**: Weapon system operations
- **World**: World state changes (manual unit moves, state updates)
- **API**: HTTP API requests and responses
- **EntityProcessor**: Internal entity processing (generally suppressed in production)

Event-Based Logging Strategy
-----------------------------
We focus on **game-relevant events** rather than low-level operations:

✅ **Log these events:**
- Unit fires at target: `🎯 Unit A FIRED Missile at Unit B (distance: 1500m)`
- Projectile hits target: `💥 Unit A HIT Unit B with Shell for 50 damage (HP: 50.0)`
- Unit destroyed: `⚔️ Unit A DESTROYED Unit B with Bullet (dealt 10 damage)`
- Tick summaries: `✅ Tick 123 → 124 complete: 3 moved, 2 fired, 1 destroyed`
- API requests: `Incoming POST /api/units/move`

❌ **Don't log these (too noisy):**
- Per-unit movement updates every tick
- Per-projectile position updates
- Entity cloning operations
- Weapon factory object creation details

Configuration keys
------------------
Put the settings under the `ConsoleLogging` section in `appsettings.Development.json` and `appsettings.Production.json`.

Top-level keys (ConsoleLogging):

- `Enabled` (bool): enable/disable console logging entirely.
- `MinimumLevel` (string): global minimum `LogLevel` (e.g. `Debug`, `Information`, `Warning`, `Error`). Messages below this level are discarded.
- `BatchEnabled` (bool): enable batching of console writes. Recommended `true` in production when many logs are produced.
- `BatchIntervalMs` (int): flush interval (milliseconds) for the batch queue.
- `DisableAtomicLogs` (bool): if true, logs marked as `atomic` (very frequent) will be suppressed.

Category flags (ConsoleLogging:Categories):

- A simple key/value map where the key is a short category substring and the value is a boolean indicating whether that category's logs are enabled. Example:

```
"ConsoleLogging:Categories": {
  "Simulation": true,
  "AI": true,
  "Weapon": true,
  "EntityProcessor": false,
  "World": true,
  "API": true
}
```

Per-category minimum levels (ConsoleLogging:CategoryLevels):

- Allows setting a minimum `LogLevel` for a specific category. Example:

```
"ConsoleLogging:CategoryLevels": {
  "Simulation": "Information",
  "AI": "Information",
  "Weapon": "Information",
  "EntityProcessor": "Warning"
}
```

Behavior and precedence
-----------------------
When a log message is emitted we apply checks in this order:

1. If `ConsoleLogging:Enabled` is `false`, the message is ignored.
2. If the message `LogLevel` is lower than the global `ConsoleLogging:MinimumLevel`, the message is ignored.
3. If a per-category minimum is configured and the category matches, the message must be at or above that per-category minimum.
4. If `ConsoleLogging:Categories` contains a matching key and it is set to `false`, the message is ignored. If set to `true` it is explicitly allowed (subject to levels above).
5. If `DisableAtomicLogs` is true and the message was marked `atomic`, the message is ignored.
6. Otherwise the message will be emitted; if `BatchEnabled` is true it will be queued and flushed periodically.

Category matching is substring-based (case-insensitive). If multiple category-level keys match, the longest match is used.

Atomic logs
-----------
Certain logs that are extremely frequent (per-unit movement, per-projectile position, cloning messages) were previously marked as `atomic` in code but have now been removed. The `DisableAtomicLogs` configuration remains for future use if needed.

Recommended settings
--------------------
Development (`appsettings.Development.json`):

- `Enabled`: true
- `MinimumLevel`: `Debug`
- `BatchEnabled`: false (immediate feedback)
- `DisableAtomicLogs`: false
- `Categories`: enable Combat, Simulation, AI, Weapon, World, API
- `CategoryLevels`: 
  - Combat: `Debug` (see all combat events)
  - Simulation: `Information` (tick summaries)
  - AI: `Debug` (see AI decisions)
  - Weapon: `Information`

Production (`appsettings.Production.json`):

- `Enabled`: true
- `MinimumLevel`: `Information`
- `BatchEnabled`: true (reduce I/O)
- `BatchIntervalMs`: 200-500
- `DisableAtomicLogs`: true
- `Categories`: enable Combat, Simulation, AI, Weapon, World, API; disable EntityProcessor
- `CategoryLevels`: 
  - Combat: `Information` (critical combat events only)
  - Simulation: `Information` (tick summaries)
  - AI: `Information`
  - Weapon: `Information`
  - EntityProcessor: `Warning` (errors only)

Notes
-----
- Console logging is additive to existing `ILogger` usage. Keep structured logs via `ILogger<T>` for production export to files/ephemeral stores, and use the console logger for quick visibility and local debugging.
- If you need more advanced routing (per-category sinks, file/remote logging), integrate a full logging provider (Serilog, Seq, etc.) and forward filtered console logs as desired.
- For client-side interpolation and prediction, use the `GET /api/movement/snapshot` endpoint which returns optimized movement data (normalized speeds in m/s, position, heading).

Client Integration
------------------
The `MovementSnapshotDto` returned by `/api/movement/snapshot` contains:
- `Tick`: Current simulation tick number
- `Timestamp`: UTC timestamp of the snapshot
- `Units`: Array of unit movement data (position, heading, speed in m/s, status, faction)
- `Projectiles`: Array of projectile movement data (position, heading, speed in m/s, type, owner)

Clients can use this data to:
1. Interpolate positions between server ticks
2. Predict future positions using linear extrapolation (position + velocity × deltaTime)
3. Smooth animations by blending between received snapshots

Examples
--------
Development example (partial):

```json
"ConsoleLogging": {
  "Enabled": true,
  "MinimumLevel": "Debug",
  "BatchEnabled": false,
  "DisableAtomicLogs": false
},
"ConsoleLogging:Categories": {
  "Simulation": true,
  "AI": true,
  "Combat": true,
  "Weapon": true,
  "World": true,
  "API": true
},
"ConsoleLogging:CategoryLevels": {
  "Combat": "Debug",
  "Simulation": "Information",
  "AI": "Debug",
  "Weapon": "Information"
}
```

Production example (partial):

```json
"ConsoleLogging": {
  "Enabled": true,
  "MinimumLevel": "Information",
  "BatchEnabled": true,
  "BatchIntervalMs": 250,
  "DisableAtomicLogs": true
},
"ConsoleLogging:Categories": {
  "Simulation": true,
  "AI": true,
  "Combat": true,
  "Weapon": true,
  "World": true,
  "API": true,
  "EntityProcessor": false
},
"ConsoleLogging:CategoryLevels": {
  "Combat": "Information",
  "Simulation": "Information",
  "AI": "Information",
  "Weapon": "Information",
  "EntityProcessor": "Warning"
}
```

Sample Console Output
---------------------
```
⏱️ [2024-01-15T10:30:45.123Z] [Information] [SimulationEngine] Starting tick 42 with 5 units and 12 projectiles
🎯 [2024-01-15T10:30:45.145Z] [Information] [Combat] Alpha-1 (Fighter) FIRED Bullet at Bravo-2 (Truck) (distance: 1200m)
💥 [2024-01-15T10:30:45.167Z] [Information] [Combat] Alpha-1 (Fighter) HIT Bravo-2 (Truck) with Bullet for 10 damage (HP: 40.0)
⚔️ [2024-01-15T10:30:45.189Z] [Warning] [Combat] Charlie-1 (Frigate) DESTROYED Bravo-2 (Truck) with Shell (dealt 50 damage)
✅ [2024-01-15T10:30:45.201Z] [Information] [SimulationEngine] Tick 42 → 43 complete: 3 moved, 2 fired, 1 destroyed
```

Color Scheme
------------
- **SimulationEngine**: Yellow
- **Combat**: Red (critical game events)
- **AI**: Magenta
- **Weapon/Projectile**: Red
- **World**: Cyan
- **API/Controllers**: Green
- **Errors**: Red
- **Warnings**: Yellow

If you want, I can add a short `LOGGING.md` section with suggested examples for `docker-compose` and Kubernetes environments as well.
