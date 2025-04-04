# Internship Management System

This project is an Internship Management System built using ASP.NET Core MVC and MSSQL Server. It provides a platform for managing internship programs, including student registration, internship offers from companies, and administration of internship placements.

## Features

- **User Roles:**
  - **Admin:** Manages the entire system, including users, and internship offers.
  - **Student:** Registers for  internships and views internship offers.

- **Functionalities:**
  - User authentication and authorization.
  - 
  - CRUD operations for managing users, and students.
  - Internship application and placement management by students.

## Technologies Used

- **ASP.NET Core MVC:** Framework for building web applications using the Model-View-Controller architecture.
- **Entity Framework Core:** Object-Relational Mapping (ORM) framework for interacting with databases.
- **MSSQL Server:** Relational database management system.
- **Bootstrap:** Front-end framework for responsive design.
- **HTML/CSS/JavaScript:** Front-end technologies for user interface and interactivity.

## Prerequisites

- Visual Studio (or Visual Studio Code) with ASP.NET Core development tools installed.
- MSSQL Server installed locally or accessible remotely.
- .NET Core SDK.

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ShivamKr7250/RF-Technologies.git
   ```
   
2. **Set up the database:**
   - Update the connection string in `appsettings.json` under `InternshipManagementSystem` project to match your MSSQL Server instance.

3. **Run the application:**
   - Open the solution file (`InternshipManagementSystem.sln`) in Visual Studio.
   - Build the solution to restore NuGet packages.
   - Set `InternshipManagementSystem.Web` as the startup project.
   - Run the application using IIS Express or your preferred local server configuration.

4. **Initial setup:**
   - Upon running the application for the first time, it will create necessary database tables and seed initial data (admin user, roles, etc.).
   - Login with the provided admin credentials to access the system.

## Project Structure

- **InternshipManagementSystem.Web:** ASP.NET Core MVC web application.
  - Controllers: Handle incoming requests and provide responses to the user.
  - Views: User interface templates written in Razor syntax.
  - Models: C# classes representing data entities and view models.
  - Services: Business logic and data access operations.
  - Repositories: Data access layer using Entity Framework Core.
  - wwwroot: Static files (CSS, JavaScript, images).

- **InternshipManagementSystem.Data:** Entity Framework Core data context and migrations.

- **InternshipManagementSystem.Models:** C# classes representing data entities used across the application.

## Contributing

Feel free to fork this repository, propose changes, & submit pull requests.

## License

This project is licensed under the [MIT License](LICENSE).

## Acknowledgments

- ASP.NET Core Documentation
- Entity Framework Core Documentation
- Microsoft Docs for .NET


This README file covers essential information about your Internship Management System project, including setup instructions, technologies used, project structure, and guidelines for contributing. Adjust the sections as needed to fit your specific project details and preferences.
