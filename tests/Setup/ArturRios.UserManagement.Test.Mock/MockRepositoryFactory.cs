using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Repositories;
using Moq;

namespace ArturRios.UserManagement.Test.Mock;

public class MockRepositoryFactory
{
    public static IUserRangeRepository CreateUserRangeRepositoryInstance()
    {
        Mock<IUserRangeRepository> repository = new();

        repository.Setup(repo => repo.UpdateRange(It.IsAny<List<User>>()))
            .Returns((List<User> users) => users);

        repository.Setup(repo => repo.DeleteRange(It.IsAny<List<int>>()))
            .Returns((List<int> ids) => ids);

        return repository.Object;
    }

    public static IUserReadOnlyRepository CreateUserReadOnlyRepositoryInstance(List<User> users)
    {
        Mock<IUserReadOnlyRepository> repository = new();

        repository.Setup(repo => repo.GetAll())
            .Returns(users.AsQueryable);

        repository.Setup(repo => repo.GetById(It.IsAny<int>()))
            .Returns((int id) =>
            {
                return users.FirstOrDefault(user => user.Id == id);
            });

        return repository.Object;
    }

    public static IUserRepository CreatUserRepositoryInstance(List<User> users)
    {
        Mock<IUserRepository> repository = new();

        repository.Setup(repo => repo.Create(It.IsAny<User>()))
            .Returns(() => users.Max(user => user.Id) + 1);

        repository.Setup(repo => repo.GetById(It.IsAny<int>()))
            .Returns((int id) =>
            {
                return users.FirstOrDefault(user => user.Id == id);
            });

        repository.Setup(repo => repo.GetAll())
            .Returns(users.AsQueryable);

        repository.Setup(repo => repo.Update(It.IsAny<User>()))
            .Returns((User user) => user);

        repository.Setup(repo => repo.Delete(It.IsAny<User>()))
            .Returns((User user) => user.Id);

        return repository.Object;
    }
}
