using WarSim.Domain;

namespace WarSim.Simulation.Commands
{
    public record MoveCommand(System.Guid UnitId, double? Latitude, double? Longitude, double? Heading, double? Speed) : ICommand;

    public record FireCommand(System.Guid UnitId, ProjectileType ProjectileType, double Heading, double Speed, double Damage, double? TargetLatitude = null, double? TargetLongitude = null) : ICommand;
}
