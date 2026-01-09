# Task Management Application

## Overview
This is a Task Management application developed as part of the screening
assignment for Oritso Private Limited. The application allows users to
create, view, update, delete, and search tasks.

## Technology Stack
- Backend: ASP.NET Core Web API
- Frontend: ASP.NET Core MVC
- ORM: Entity Framework Core (DB First)
- Database: Microsoft SQL Server

## Application Architecture
The application follows a layered architecture:
- MVC Frontend communicates with Web API using HTTP
- Web API handles business logic and data access
- Entity Framework Core (DB First) is used to interact with SQL Server

## Database Design
### Tables
**Tasks**
- Id (Primary Key)
- Title
- Description
- DueDate
- Status
- Remarks
- CreatedOn
- UpdatedOn
- CreatedBy
- UpdatedBy

### ER Diagram
Single entity design with Tasks table representing task records.

### Data Dictionary
| Column | Description |
|------|------------|
| Title | Task title |
| Status | Open / InProgress / Completed |
| DueDate | Task deadline |

### Indexes Used
- Index on Status column
- Index on DueDate column

### DB First Approach
The DB First approach was used to work with an existing database schema
and ensure tight control over database design.

## Features Implemented
- Create Task
- View Tasks
- Update Task
- Delete Task
- Search Tasks

## Build & Run Instructions

### Prerequisites
- .NET SDK 8 
- SQL Server
- Visual Studio 

### Steps
1. Clone the repository
2. Restore NuGet packages
3. Update connection string in `appsettings.json`
4. Run the API project
5. Run the MVC project

## Conclusion
This project demonstrates MVC architecture, RESTful APIs, database
design, and clean coding practices.
