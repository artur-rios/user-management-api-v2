# Commands

This document describes the `ArturRios.UserManagement.Command` project (located under
`src/Core/ArturRios.UserManagement.Command`)
and the core command-related classes found in the repository `Core` folder. It includes a concise description of each
command, its handler, validator and output types

Overview
--------
`ArturRios.UserManagement.Command` implements the write-side (commands) for the user management domain. Commands are
simple classes that represent an intent (create, update, delete, authenticate, activate/deactivate, etc.). Each command
is
validated, handled by a command handler, and returns a typed output object that can carry data, messages and errors.

## High-level flow

- A `Command` class is created by the API layer.
- A validator (e.g. `CreateUserCommandValidator`) verifies basic invariants.
- A command handler (e.g. `CreateUserCommandHandler`) executes domain operations, uses repositories and domain
  aggregates (e.g. `User`) and produces an output object.
- Outputs (e.g. `CreateUserCommandOutput`) extend the project's `CommandOutput` and carry result data.

Project structure (core classes included in this document)

- Commands: classes under `src/Core/ArturRios.UserManagement.Command/Commands`
- CommandHandlers: classes under `src/Core/ArturRios.UserManagement.Command/CommandHandlers`
- Validators: classes under `src/Core/ArturRios.UserManagement.Command/Validators`
- Output: classes under `src/Core/ArturRios.UserManagement.Command/Output`
- Domain aggregates: `User` and `Role` under `src/Core/ArturRios.UserManagement.Domain/Aggregates`

## Commands (summary)

- [CreateUserCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/CreateUserCommand.cs)
    - Properties: Name, Email, Password, RoleId
    - Purpose: Create a new user
- [AuthenticateUserCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/AuthenticateUserCommand.cs)
    - Properties: Email, Password
    - Purpose: Authenticate a user by email/password
- [ActivateUserCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/ActivateUserCommand.cs)
    - Properties: UserId
    - Purpose: Activate a single user
- [DeactivateUserCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/DeactivateUserCommand.cs)
    - Properties: UserId
    - Purpose: Deactivate a single user
- [ActivateManyUsersCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/ActivateManyUsersCommand.cs)
    - Properties: IEnumerable<int> UserIds
    - Purpose: Activate multiple users
- [DeactivateManyUsersCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/DeactivateManyUsersCommand.cs)
    - Properties: IEnumerable<int> UserIds
    - Purpose: Deactivate multiple users
- [DeleteUserCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/DeleteUserCommand.cs)
    - Properties: UserId
    - Purpose: Delete a user (subject to domain checks)
- [DeleteManyUsersCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/DeleteManyUsersCommand.cs)
    - Properties: IEnumerable<int> UserIds
    - Purpose: Delete multiple users
- [UpdateUserCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/UpdateUserCommand.cs)
    - Properties: UserId, Name
    - Purpose: Update the user's name
- [UpdateUserEmailCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/UpdateUserEmailCommand.cs)
    - Properties: UserId, Email
    - Purpose: Change the user's email
- [UpdateUserRoleCommand](../../src/Core/ArturRios.UserManagement.Command/Commands/UpdateUserRoleCommand.cs)
    - Properties: UserId, RoleId
    - Purpose: Change the user's role

## Validators

- Each command has a matching validator class (e.g. `CreateUserCommandValidator`, `AuthenticateUserCommandValidator`,
  etc.).
- Validators implement the `IFluentValidator<TCommand>` contract (from the common library) and ensure command-level
  invariants before a handler runs.

## Outputs

- Each command has an output type under `Output/` (e.g. `CreateUserCommandOutput`).
- Outputs inherit from `CommandOutput` (from the common pipelines library) and may include result-specific properties (
  e.g. `CreateUserCommandOutput.CreatedUserId`).

## Handlers

- Command handlers are in `CommandHandlers/` and implement the common `ICommandHandler<TCommand, TOutput>` interface.
- Example: `CreateUserCommandHandler` takes an `IFluentValidator<CreateUserCommand>` and an `IUserRepository`, validates
  the command, checks for duplicates, creates a `User` domain object, sets the password and calls repository.Create(
  user). It returns a `DataOutput<CreateUserCommandOutput?>` with result data and messages.

## Class diagram (classes only from the Core folder)

Note: This diagram intentionally only includes classes physically present under the top-level `Core` folder (commands,
handlers, validators, outputs, and domain aggregates). It omits common base classes and external interfaces (for example
the `Command` and `CommandOutput` base classes, or `IUserRepository`), but shows relations (e.g. "handles", "returns", "
uses") between Core classes.

## Per-command class diagrams

The class diagram has been split into one file per command. Each file contains a focused mermaid class diagram and a
brief description. Files are located in `docs/commands/` (relative to this file).

- [CreateUserCommand](create_user.md)
- [AuthenticateUserCommand](authenticate_user.md)
- [ActivateUserCommand](activate_user.md)
- [DeactivateUserCommand](deactivate_user.md)
- [ActivateManyUsersCommand](activate_many_users.md)
- [DeactivateManyUsersCommand](deactivate_many_users.md)
- [DeleteUserCommand](delete_user.md)
- [DeleteManyUsersCommand](delete_many_users.md)
- [UpdateUserCommand](update_user.md)
- [UpdateUserEmailCommand](update_user_email.md)
- [UpdateUserRoleCommand](update_user_role.md)

Each linked file contains a mermaid diagram limited to the classes relevant to that specific command and notes about
handler behavior and domain interactions.

<!-- The large combined diagram was moved into per-command files -->

## Extending and maintaining

- When adding a new command: add a command class under `Commands/`, a validator under `Validators/`, an output under
  `Output/` and a handler under `CommandHandlers/`.
- Keep domain logic in aggregates (User/Role). Handlers should orchestrate repository calls and domain operations but
  must contain only domain logic that does not belong on the aggregates, for example, database related rules.
