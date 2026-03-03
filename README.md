# PROJECT CHESS
<img width="788" height="346" alt="image" src="https://github.com/user-attachments/assets/51f6f698-083f-49f2-b8e7-cbcae628274c" />

A full-stack web-based chess application built using Angular, ASP.NET Core, and SQL Server.  
This project focuses on clean architecture, game logic separation, and database-driven state persistence.

---

## Tech Stack

Frontend:
- Angular (TypeScript)
- Component-based UI architecture
- Service-based HTTP communication

Backend:
- ASP.NET Core Web API (C#)
- RESTful endpoints
- Service layer abstraction
- Game engine logic separated from persistence

Database:
- Microsoft SQL Server
- Stored procedures
- Relational schema for game state, players, and moves

---

## Architecture Overview

The application follows a layered architecture:

- **Frontend (Angular)**  
  Handles UI rendering, move interaction, and API communication.

- **Controller Layer (ASP.NET)**  
  Exposes REST endpoints for game actions.

- **Service Layer**  
  Contains game orchestration logic and validation.

- **Game Engine**  
  Encapsulates chess rules including:
  - Move validation
  - Turn tracking
  - Check/checkmate logic
  - Special rules (castling, en passant, promotion)

- **Repository Layer**  
  Handles database interaction and persistence.

This separation ensures:
- Clear responsibility boundaries
- Easier testing and debugging
- Maintainable and scalable structure

---

## Key Features

- Full chess move validation
- Turn-based state management
- Persistent game storage
- SQL-based move logging
- Clean separation between logic and data access
- REST API design for frontend-backend communication

---

## Database Design

The database stores:
- Users
- Games
- GamePlayers
- GameMoves
- GamePieces

Stored procedures are used to:
- Save board state
- Log moves
- Initialize new games

---

## Engineering Highlights

- Refactored circular dependencies into service-based architecture
- Separated game logic from persistence layer
- Implemented DTOs for safe data transfer
- Designed schema to reflect real-world chess relationships

---

## Future Improvements

- Authentication and user accounts
- ELO rating system
- Game replay system
- Deployment to Azure

---

## What I Learned

- Designing layered backend architecture
- Managing complex state in a distributed system
- Structuring SQL schemas for game state persistence
- Debugging cross-layer communication issues
