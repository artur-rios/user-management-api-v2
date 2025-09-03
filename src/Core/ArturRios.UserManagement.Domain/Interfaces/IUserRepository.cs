using ArturRios.Common.Data.Interfaces;

namespace ArturRios.UserManagement.Domain.Interfaces;

public interface IUserRepository : ICrudRepository<Aggregates.User>, IMultiRepository<Aggregates.User>;
