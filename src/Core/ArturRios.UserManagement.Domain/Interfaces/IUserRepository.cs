using ArturRios.Common.Data;

namespace ArturRios.UserManagement.Domain.Interfaces;

public interface IUserRepository : ICrudRepository<Aggregates.User>, IMultiRepository<Aggregates.User>;
