using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class DeactivateManyUsersCommandHandler(
    IFluentValidator<DeactivateManyUsersCommand> validator,
    IUserRepository userRepository) : ICommandHandler<DeactivateManyUsersCommand, DeactivateManyUsersCommandOutput>
{
    public DataOutput<DeactivateManyUsersCommandOutput?> Handle(DeactivateManyUsersCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<DeactivateManyUsersCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        List<int> deactivatedIds = [];
        List<int> notFoundIds = [];
        List<(int, string[])> failedDeactivations = [];

        foreach (var id in command.UserIds)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);

                continue;
            }

            var result = user.Deactivate();

            if (result.Success)
            {
                userRepository.Update(user);

                deactivatedIds.Add(id);
            }
            else
            {
                failedDeactivations.Add((id, result.Errors.ToArray()));
            }
        }

        var notFoundMessage = notFoundIds.Count > 0
            ? $"Users with IDs {string.Join(", ", notFoundIds)} not found"
            : string.Empty;
        var cannotDeactivateMessage = failedDeactivations.Count > 0
            ? $"Users with IDs {string.Join(", ", failedDeactivations.Select(x => x.Item1))} cannot be deactivated"
            : string.Empty;

        if (!string.IsNullOrEmpty(notFoundMessage) || !string.IsNullOrEmpty(cannotDeactivateMessage))
        {
            var combinedMessage = string.Join(". ",
                new[] { notFoundMessage, cannotDeactivateMessage }.Where(msg => !string.IsNullOrEmpty(msg)));

            output.AddError(combinedMessage);
        }

        foreach (var fail in failedDeactivations)
        {
            var errorMessage = $"User with Id {fail.Item1}: {string.Join(", ", fail.Item2)}";

            output.AddError(errorMessage);
        }

        foreach (var notFoundId in notFoundIds)
        {
            var errorMessage = $"User with Id {notFoundId} not found";

            output.AddError(errorMessage);
        }

        output.Data = new DeactivateManyUsersCommandOutput
        {
            DeactivatedIds = deactivatedIds,
            FailedDeactivationIds = failedDeactivations.Select(x => x.Item1),
            NotFoundIds = notFoundIds
        };

        return output;
    }
}
