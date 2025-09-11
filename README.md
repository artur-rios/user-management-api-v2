# User Management API

- Web API for managing users, roles, and permissions in a system
- Can be used as a microservice
- This project is designed to be a standalone service that can be integrated into larger systems
- It provides a RESTful interface for user management operations, including CRUD operations and authentication
- This project aims to be an example of a clean architecture implementation of a Web API in .NET, focusing on separation of concerns and testability

## Stack

- .NET 8.0 / C# 12
- Entity Framework

## Details

- Rest API
- JWT Authentication
- Unit Tests
- Functional Tests

## Domain class diagram

<!--

@startuml
class Constants{
  +MinimumPasswordLength : int
}
class Role{
  +Name : string
  +Description : string
}
class User{
  +Name : string
  +Email : string
  +Password : byte
  +Salt : byte
  +RoleId : int
  +CreatedAt : DateTime
  +Active : bool
  +CanActivate() : ProcessOutput
  +CanDeactivate() : ProcessOutput
  +CanDelete() : ProcessOutput
  +CanRegister(,) : ProcessOutput
  +CanUpdate() : ProcessOutput
}
class Roles{
  -Admin : int
  -Regular : int
  -Test : int
}
class UserFilter{
  +Name : string?
  +Email : string?
  +RoleId : int?
  +CreatedAt : DateTime?
  +Active : bool?
}
class UserMultiFilter{
  +Ids : 
  +Names : 
  +Emails : 
  +RoleIds : 
  +CreationDates : 
}
class Entity{
  +Id : int
}
class DataFilter
Role --|> Entity
User --|> Entity
UserFilter --|> DataFilter
UserMultiFilter --|> DataFilter
@enduml


-->

![domain-class-diagram](/docs/diagrams/domain-class-diagram.svg)

*Documentation under construction*
