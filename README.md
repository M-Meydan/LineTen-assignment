# LineTen-assignment

The solution is restructured to follow clean architectural practices, consisting of:

- `App`: Application specific code and abstractions.
- `Domain`: Core domain entities.
- `Infrastructure`: Repository implementations and migrations.
- `Unit and Integration Tests`


### Solution Overview

- CRUD endpoints implemented for Customer,Product and Order entities.

- Global error middleware is added for exceptions, ensuring a consistent error handling mechanism throughout the API.

- Exception handlers have been added for handling application-specific exception types that implement the open-closed principle

- Dependencies have been configured and injected appropriately, promoting better maintainability and testability.

- CQRS pattern implemented.

- FluentValidation has been used for validations.

- Swagger Documentation added.

- Docker file created for the API project.


## Tests

- Unit tests and Integration tests included.


## Areas for improvement

There is always room for increasing test coverage.
