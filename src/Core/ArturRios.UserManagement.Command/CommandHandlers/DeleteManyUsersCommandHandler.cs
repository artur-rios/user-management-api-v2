using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class DeleteManyUsersCommandHandler(
    IFluentValidator<DeleteManyUsersCommand> validator,
    IUserRepository userRepository) : ICommandHandler<DeleteManyUsersCommand, DeleteManyUsersCommandOutput>
{
    public DataOutput<DeleteManyUsersCommandOutput?> Handle(DeleteManyUsersCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<DeleteManyUsersCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        List<int> deletedIds = [];
        List<int> notFoundIds = [];
        List<(int, string[])> failedDeletions = [];

        foreach (var id in command.UserIds)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);

                continue;
            }

            var canDelete = user.CanDelete();

            if (canDelete.Success)
            {
                userRepository.Delete(user);

                deletedIds.Add(id);
            }
            else
            {
                failedDeletions.Add((id, canDelete.Errors.ToArray()));
            }
        }

        var notFoundMessage = notFoundIds.Count > 0
            ? $"Users with IDs {string.Join(", ", notFoundIds)} not found"
            : string.Empty;
        var cannotDeleteMessage = failedDeletions.Count > 0
            ? $"Users with IDs {string.Join(", ", failedDeletions.Select(x => x.Item1))} cannot be deleted"
            : string.Empty;

        if (!string.IsNullOrEmpty(notFoundMessage) || !string.IsNullOrEmpty(cannotDeleteMessage))
        {
            var combinedMessage = string.Join("; ",
                new[] { notFoundMessage, cannotDeleteMessage }.Where(msg => !string.IsNullOrEmpty(msg)));

            output.AddError(combinedMessage);
        }

        output.Data = new DeleteManyUsersCommandOutput
        {
            DeletedIds = deletedIds,
            FailedDeletionIds = failedDeletions.Select(x => x.Item1),
            NotFoundIds = notFoundIds
        };

        return output;
    }
}
