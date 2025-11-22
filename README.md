# Team Daily Goal Tracker

A full-stack web application for tracking daily goals and moods of team members.

## Tech Stack

**Frontend:**
- Vue 3 (Composition API)
- TypeScript
- Vite
- DaisyUI (Tailwind CSS)
- Axios

**Backend:**
- .NET 9 Web API
- Dapper (micro-ORM)
- SQL Server 2022
- FluentValidation

## Prerequisites

- Node.js 18+ and npm
- .NET 9 SDK
- Docker and Docker Compose

## Quick Start

### 1. Start the Database

```bash
cd docker
docker-compose up -d
```

This will start SQL Server 2022 on port 1433 and initialize the database with schema and seed data.

### 2. Fix NuGet Configuration (if needed)

If you encounter NuGet HTTP source errors:

```bash
# Remove HTTP sources
dotnet nuget remove source nuget.remotes.local

# Ensure you have the official NuGet source
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
```

### 3. Start the Backend

```bash
cd backend/src/TeamGoalTracker.Api
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5205`

### 4. Start the Frontend`

```bash
cd frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`

## Project Structure

```
team-daily-goal/
├── backend/
│   ├── src/
│   │   ├── TeamGoalTracker.Api/          # Web API project
│   │   │   ├── Controllers/              # API controllers
│   │   │   ├── Data/                     # Repositories and DB factory
│   │   │   ├── Services/                 # Business logic
│   │   │   ├── Middleware/               # Exception handling
│   │   │   └── Program.cs                # DI configuration
│   │   └── TeamGoalTracker.Core/         # Domain entities
│   │       ├── Entities/                 # Domain models
│   │       └── DTOs/                     # Data transfer objects
│   └── tests/                            # Unit and integration tests
├── frontend/
│   ├── src/
│   │   ├── components/                   # Vue components
│   │   ├── composables/                  # Composable functions
│   │   ├── services/                     # API client
│   │   ├── types/                        # TypeScript types
│   │   ├── views/                        # Page views
│   │   ├── App.vue                       # Root component
│   │   └── main.ts                       # App entry point
│   ├── package.json
│   ├── vite.config.ts
│   └── tsconfig.json
├── docker/
│   ├── docker-compose.yml                # SQL Server container
│   └── init-db.sql                       # Database schema and seed data
└── specs/
    └── 001-team-goal-tracker/            # Feature specifications
```

## API Endpoints

- `GET /api/dashboard?date=YYYY-MM-DD` - Get dashboard data (team members, goals, moods, stats)
- `GET /api/team-members` - Get all team members
- `GET /api/stats?date=YYYY-MM-DD` - Get team statistics for a specific date

## Database Schema

**TeamMembers**
- Id (PK)
- Name
- CreatedAt

**Goals**
- Id (PK)
- TeamMemberId (FK)
- Description
- IsCompleted
- Date
- CreatedAt

**Moods**
- Id (PK)
- TeamMemberId (FK)
- MoodType (1=VeryHappy, 2=Happy, 3=Neutral, 4=Sad, 5=Stressed)
- Date
- UpdatedAt

## Features

### User Story 1: View Dashboard (Implemented)
- View team members in card layout
- See goals and completion status for each member
- View current mood for each member with emoji indicators
- See team-wide statistics (completion %, mood distribution)

### Coming Soon
- User Story 2: Add daily goals
- User Story 3: Update mood
- User Story 4: Mark goals as complete

## Development

### Running Tests

```bash
# Backend tests
cd backend/tests/TeamGoalTracker.Tests
dotnet test

# Frontend tests (when implemented)
cd frontend
npm run test
```

### Building for Production

```bash
# Backend
cd backend/src/TeamGoalTracker.Api
dotnet publish -c Release

# Frontend
cd frontend
npm run build
```

## Troubleshooting

**Database connection fails:**
- Ensure Docker container is running: `docker ps`
- Check connection string in `appsettings.json`

**Frontend can't reach API:**
- Check CORS configuration in `Program.cs`
- Verify API is running on port 5205
- Check Vite proxy configuration in `vite.config.ts`

**NuGet package restore fails:**
- Remove HTTP sources as described in Quick Start
- Clear NuGet cache: `dotnet nuget locals all --clear`

## License

MIT
