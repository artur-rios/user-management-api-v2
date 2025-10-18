using ArturRios.Common.Data.Interfaces;

namespace ArturRios.UserManagement.Domain.Repositories;

public interface IUserRepository : ICrudRepository<Aggregates.User>;
