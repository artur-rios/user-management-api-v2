using ArturRios.Common.Data;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Interfaces;

namespace ArturRios.UserManagement.Data.ProtoBuf;

public class UserRepository : IUserRepository
{
    public int Create(User entity)
    {
        throw new NotImplementedException();
    }

    public IQueryable<User> GetByFilter(DataFilter filter, bool track = false)
    {
        throw new NotImplementedException();
    }

    public User? GetById(int id, bool track = false)
    {
        throw new NotImplementedException();
    }

    public void Update(User entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IQueryable<User> GetByMultiFilter(DataFilter filter, bool track = false)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> MultiDelete(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }
}