using ICommand = PetFamily.Application.Interfaces.Abstractions.ICommand;

namespace PetFamily.Application.VolunteerAggregate.Commands.ChangePetsPosition;

public record ChangePetsPositionCommand(
    Guid VolunteerId,
    Guid PetId,
    int NewPosition) : ICommand;