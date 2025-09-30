# Domain

Contains the core business logic and aggregates of the User Management API.

## Aggregates

- [Role](../src/Core/ArturRios.UserManagement.Domain/Aggregates/Role.cs)
- [User](../src/Core/ArturRios.UserManagement.Domain/Aggregates/User.cs)

## Enums

- [Roles](../src/Core/ArturRios.UserManagement.Domain/Enums/Roles.cs)

## Filters

- [UserFilter](../src/Core/ArturRios.UserManagement.Domain/Filters/UserFilter.cs)
- [UserMultiFilter](../src/Core/ArturRios.UserManagement.Domain/Filters/UserMultiFilter.cs)

## Repositories

- [IUserRepository](../src/Core/ArturRios.UserManagement.Domain/Repositories/IUserRepository.cs)

## Class Diagram

```mermaid
---
title: Domain
---
classDiagram
    Role --|> Entity
    User --|> Entity
    User --> Role : RoleId
    
    class Entity {
        +int Id
    }
    
    class Role {
        +string Name
        +string Description
    }
    
    class User {
        +string Name
        +string Email
        +byte[] Password
        +byte[] Salt
        +int RoleId
        +DateTime CreatedAt
        +bool Active
        
        +Activate() ProcessOutput
        +CanDelete() ProcessOutput
        +CanRegister(int authenticatedRoleId) ProcessOutput
        +Deactivate() ProcessOutput
        +SetPassword(byte[] password, byte[] salt) void
        +Update(User updatedUser) ProcessOutput
    }
```