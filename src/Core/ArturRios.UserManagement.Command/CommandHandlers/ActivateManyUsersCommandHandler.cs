using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class ActivateManyUsersCommandHandler(
    IFluentValidator<ActivateManyUsersCommand> validator,
    IUserRepository userRepository) : ICommandHandler<ActivateManyUsersCommand, ActivateManyUsersCommandOutput>
{
    public DataOutput<ActivateManyUsersCommandOutput?> Handle(ActivateManyUsersCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<ActivateManyUsersCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        List<int> activatedIds = [];
        List<int> notFoundIds = [];
        List<(int, string[])> failedActivations = [];

        foreach (var id in command.UserIds)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);

                continue;
            }

            var result = user.Activate();

            if (result.Success)
            {
                userRepository.Update(user);

                activatedIds.Add(id);
            }
            else
            {
                failedActivations.Add((id, result.Errors.ToArray()));
            }
        }

        var notFoundMessage = notFoundIds.Count > 0
            ? $"Users with IDs {string.Join(", ", notFoundIds)} not found"
            : string.Empty;
        var cannotActivateMessage = failedActivations.Count > 0
            ? $"Users with IDs {string.Join(", ", failedActivations.Select(x => x.Item1))} cannot be activated"
            : string.Empty;

        List<string> errors = [];

        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            errors.Add(notFoundMessage);
        }

        if (!string.IsNullOrEmpty(cannotActivateMessage))
        {
            errors.Add(cannotActivateMessage);
        }

        foreach (var cannotActivate in failedActivations)
        {
            errors.Add($"User with Id {cannotActivate.Item1}: {string.Join(", ", cannotActivate.Item2)}");
        }

        var totalInput = command.UserIds.ToList().Count;
        var totalErrors = notFoundIds.Count + failedActivations.Count;

        if (totalErrors >= totalInput)
        {
            output.AddErrors(errors);

            return output;
        }

        if (errors.Count > 0)
        {
            output.AddMessage($"{activatedIds.Count} activations were successful");
            output.AddMessages(errors);
        }
        else
        {
            output.AddMessage($"{totalInput} user(s) activated successfully");
        }


        output.Data = new ActivateManyUsersCommandOutput
        {
            ActivatedIds = activatedIds,
            FailedActivationIds = failedActivations.Select(x => x.Item1),
            NotFoundIds = notFoundIds
        };

        return output;
    }
}
