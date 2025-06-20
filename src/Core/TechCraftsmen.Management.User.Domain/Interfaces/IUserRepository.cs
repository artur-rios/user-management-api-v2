using TechCraftsmen.Core.Data;

namespace TechCraftsmen.Management.User.Domain.Interfaces;

public interface IUserRepository : ICrudRepository<Aggregates.User>, IMultiRepository<Aggregates.User>;
