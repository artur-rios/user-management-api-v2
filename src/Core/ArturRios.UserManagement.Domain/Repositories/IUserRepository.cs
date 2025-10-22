using ArturRios.Common.Data.Interfaces;
using ArturRios.UserManagement.Domain.Aggregates;

namespace ArturRios.UserManagement.Domain.Repositories;

public interface IUserRepository : ICrudRepository<User>;
