# Task management system API project
_Exported on 5/17/2026 at 11:55:54 GMT-3 from Cursor (3.4.20)_

---

**User**

Create a new project, it's going to be a .NET ASP.NET Web API using .NET 8.
This API is a Task Management System.
The operations are going to be CRUD of tasks.
Each task has title, description, status, and due_date.
Tasks can be associated to a user(Id, Name)

Architecture:
UseCases, Clean Architecture, Separate the domain logic from the repositories, use SOLID principles.
Implements ADO.NET to access the sql server database
Implements JWT Auth
Validate data on controllers with FluentValidation
Uses the pattern ObjectSignature ObjectResponse to the class names
You can map objects between the application layer and infra layer according the contracts/interfaces

After the creation, gives me a .md file with the implementation details, I want to review all that was done.

---

**Cursor**

I'll create the Task Management API following Clean Architecture, matching patterns from your existing VehicleStore project. Exploring that codebase first, then scaffolding the new solution.


