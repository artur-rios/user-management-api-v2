using ArturRios.Common.Output;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Domain.Repositories;
using ArturRios.UserManagement.Dto;

namespace ArturRios.UserManagement.Services;

public class UserService(IUserRepository userRepository, IFluentValidator<UserDto> userValidator)
{

}
