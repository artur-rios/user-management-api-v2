# DeleteUserCommand

Description: Delete a single user by id. Handler typically calls `User.CanDelete()` and then repository delete. The domain `User` exposes `CanDelete()` that enforces deletion rules.

```mermaid
classDiagram
    class DeleteUserCommand { +int UserId }
    class DeleteUserCommandHandler { +DeleteUserCommandOutput Handle(DeleteUserCommand) }
    class DeleteUserCommandOutput { }
    class User { +int Id +bool Active +ProcessOutput CanDelete() }

    DeleteUserCommandHandler ..> DeleteUserCommand : handles
    DeleteUserCommandHandler ..> DeleteUserCommandOutput : returns
    DeleteUserCommandHandler ..> User : checks CanDelete()
```

