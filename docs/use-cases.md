# Use Cases

## Create Admin

### Actors

- System Administrator

### Description

A user with system administrator privileges must be able to create a new admin user.

### Preconditions

- The system administrator is logged into the system.

## Create Regular User

### Actors

- System Administrator
- Any unregistered user

### Description

A system administrator or an unregistered user must be able to create a new regular user.

### Preconditions

- The system administrator is logged into the system.
- The unregistered user is not logged into the system.

## Read All Users

### Actors

- System Administrator

### Description

A system administrator must be able to view information of all users in the system.

### Preconditions

- The system administrator is logged into the system.

## Read Profile

### Actors

- System Administrator
- Regular User

### Description

A system administrator or a regular user must be able to view their own profile information.

### Preconditions

- The system administrator or regular user is logged into the system.

## Update User

### Actors

- System Administrator

### Description

A system administrator must be able to update the information of any user in the system.

### Preconditions

- The system administrator is logged into the system.

## Update Profile

### Actors

- System Administrator
- Regular User

### Description

A system administrator or a regular user must be able to update their own profile information.

### Preconditions

- The system administrator or regular user is logged into the system.

## Deactivate User

### Actors

- System Administrator

### Description

A system administrator must be able to deactivate any user in the system.

### Preconditions

- The system administrator is logged into the system.

## Activate User

### Actors

- System Administrator

### Description

A system administrator must be able to activate any deactivated user in the system.

### Preconditions

- The system administrator is logged into the system.

## Deactivate Profile

### Actors

- System Administrator
- Regular User

### Description

A system administrator or a regular user must be able to deactivate their own profile.

### Preconditions

- The system administrator or regular user is logged into the system.

## Delete User

### Actors

- System Administrator

### Description

A system administrator must be able to permanently delete any user from the system.

### Preconditions

- The system administrator is logged into the system.
- The user must be deactivated before deletion.

## Authenticate User

### Actors

- System Administrator
- Regular User

### Description

A system administrator or a regular user must be able to log into the system using valid credentials.

### Preconditions

- The user must have an active profile in the system.

## Diagram

```mermaid
flowchart LR
  SysAdmin[System Administrator]
  RegularUser[Regular User]
  Unregistered[Unregistered User]

  CreateAdmin((Create Admin))
  CreateRegular((Create Regular User))
  ReadAll((Read All Users))
  ReadProfile((Read Profile))
  UpdateUser((Update User))
  UpdateProfile((Update Profile))
  DeactivateUser((Deactivate User))
  ActivateUser((Activate User))
  DeactivateProfile((Deactivate Profile))
  DeleteUser((Delete User))
  AuthenticateUser((Authenticate User))

  SysAdmin --> CreateAdmin
  SysAdmin --> CreateRegular
  Unregistered --> CreateRegular

  SysAdmin --> ReadAll
  SysAdmin --> ReadProfile
  RegularUser --> ReadProfile

  SysAdmin --> UpdateUser
  SysAdmin --> UpdateProfile
  RegularUser --> UpdateProfile

  SysAdmin --> DeactivateUser
  SysAdmin --> ActivateUser

  SysAdmin --> DeactivateProfile
  RegularUser --> DeactivateProfile

  SysAdmin --> DeleteUser

  SysAdmin --> AuthenticateUser
  RegularUser --> AuthenticateUser

