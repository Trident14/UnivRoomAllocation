# Student Room Allocation System (Backend)

This project implements a room allocation system for student hostels, focusing on efficient room assignments based on student preferences and gender-based allocations. The backend is built with ASP.NET Core Web API and uses Dapper for database interaction.

## Features
- Room allocation based on student preferences (First-Come-First-Serve)
- Gender-based room assignments
- Room type preference allocation
- Roommate preference management (L & R beds)
- Utilizes stored procedures and functions for complex queries
- Secure and scalable architecture

## Tech Stack
- **Backend:** ASP.NET Core Web API
- **Database:** PostgreSQL (locally running)
- **ORM:** Dapper
- **Authentication:** JWT-based
- **Architecture:** Controller -> InterfaceService -> Service -> InterfaceRepository -> Repository pattern (CISR Pattern)

## Future Enhancements
- Add concurrency handling for better performance
- Introduce more robust authentication and authorization mechanisms
- Improve UI for better user experience

## Setup
1. Clone the repository
2. Set up PostgreSQL database locally
3. Configure the database connection string in `appsettings.json`
4. Run migrations to set up the database schema
5. Start the application

## Usage
- The API allows room allocation based on student preferences, gender, and room type.
- Student preferences are handled using stored procedures and function for complex allocation logic.


