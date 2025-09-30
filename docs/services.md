# Services

Layer that encapsulates domain business logic and orchestrates data flow between repositories and controllers.

- [AuthenticationService](../src/Core/ArturRios.UserManagement.Services/AuthenticationService.cs)
- [UserService](../src/Core/ArturRios.UserManagement.Services/UserService.cs)

## Class Diagram

```mermaid
---
title: Services
---
classDiagram
    AuthenticationService --> IUserRepository
    AuthenticationService --> IFluentValidator
    AuthenticationService --> JwtTokenConfiguration
    AuthenticationService --> IFluentValidator
    UserService --> IUserRepository
    UserService --> IHttpContextAccessor
    UserService --> IFluentValidator

    class IUserRepository {
        <<interface>>
        + GetByFilter(UserFilter)
        + GetByMultiFilter(UserMultiFilter)
        + GetById(int)
        + Create(User)
        + Update(User)
        + Delete(int)
        + MultiDelete(int[])
    }

    class IFluentValidator {
        <<interface>>
        + Validate(T)

        + ValidateAndReturnErrors(T)
    }

    class AuthenticationService {
        - IFluentValidator<Credentials> _credentialsValidator
        - IUserRepository _userRepository
        - JwtTokenConfiguration _jwtTokenConfiguration
        - IFluentValidator<JwtTokenConfiguration> _jwtTokenConfigurationValidator
        + AuthenticateUser(Credentials) DataOutput<Authentication>
        + ValidateTokenAndGetUser(string) DataOutput<AuthenticatedUser>
    }

    class UserService {
        - IUserRepository userRepository
        - IHttpContextAccessor httpContextAccessor
        - IFluentValidator<UserDto> userValidator
        + CreateUser(UserDto) DataOutput<int>
        + GetUserById(int) DataOutput<UserDto>
        + GetUsersByFilter(UserFilter) DataOutput<IList<UserDto>>
        + GetUsersByMultiFilter(UserMultiFilter) DataOutput<IList<UserDto>>
        + UpdateUser(UserDto) DataOutput<UserDto?>
        + ActivateUser(int) ProcessOutput
        + ActivateManyUsers(int[]) ProcessOutput
        + DeactivateUser(int) ProcessOutput
        + DeactivateManyUsers(int[]) ProcessOutput
        + DeleteUser(int) ProcessOutput
        + DeleteManyUsers(int[]) ProcessOutput
    }

    class JwtTokenConfiguration
    class IHttpContextAccessor
```
