# Quickstart Guide: Team Daily Goal Tracker

**Last Updated**: 2025-11-20
**Feature**: Team Daily Goal Tracker (001-team-goal-tracker)

This guide walks you through setting up and running the Team Daily Goal Tracker application locally for development and testing.

## Prerequisites

Before starting, ensure you have the following installed:

- **Docker Desktop** (for SQL Server container)
  - macOS: [Docker Desktop for Mac](https://docs.docker.com/desktop/install/mac-install/)
  - Windows: [Docker Desktop for Windows](https://docs.docker.com/desktop/install/windows-install/)
  - Linux: [Docker Engine](https://docs.docker.com/engine/install/)

- **Node.js 18+ and npm**
  - Download: [Node.js Official Site](https://nodejs.org/)
  - Verify: `node --version` (should be v18 or higher)

- **.NET 8 SDK**
  - Download: [.NET 8 Download](https://dotnet.microsoft.com/download/dotnet/8.0)
  - Verify: `dotnet --version` (should be 8.0.x)

- **Git** (for version control)
  - Download: [Git Official Site](https://git-scm.com/downloads)

## Project Structure Overview

```
team-daily-goal/
├── backend/              # .NET 8 Web API
│   ├── src/
│   │   └── TeamGoalTracker.Api/
│   └── tests/
├── frontend/             # Vue 3 + TypeScript
│   ├── src/
│   └── tests/
├── docker/               # SQL Server Docker configuration
│   ├── docker-compose.yml
│   └── init-db.sql
└── specs/                # Design documentation
    └── 001-team-goal-tracker/
```

## Setup Steps

### Step 1: Start SQL Server Database

1. Navigate to the docker directory:
   ```bash
   cd docker
   ```

2. Start SQL Server container:
   ```bash
   docker-compose up -d
   ```

3. Verify SQL Server is running:
   ```bash
   docker ps
   ```
   You should see a container named `team-goal-tracker-sqlserver` running on port 1433.

4. Wait 30-60 seconds for SQL Server to initialize fully.

5. Check database initialization logs:
   ```bash
   docker logs team-goal-tracker-sqlserver
   ```
   Look for: `SQL Server is now ready for client connections`

**Troubleshooting**:
- **Port 1433 already in use**: Stop any existing SQL Server instances or change the port in `docker-compose.yml`
- **Container fails to start on WSL2**: Ensure WSL2 is updated and Docker Desktop integration is enabled
- **Database not created**: Check `init-db.sql` was mounted correctly; rebuild with `docker-compose down -v && docker-compose up -d`

### Step 2: Set Up Backend (.NET API)

1. Navigate to backend directory:
   ```bash
   cd backend/src/TeamGoalTracker.Api
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update connection string (if needed):
   - Open `appsettings.Development.json`
   - Default connection string:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=localhost,1433;Database=TeamGoalTracker;User=sa;Password=YourStrong!Password;TrustServerCertificate=True"
       }
     }
     ```
   - **Security Note**: For production, use User Secrets or environment variables. Never commit passwords to Git.

4. Run database migrations (if not using init-db.sql):
   ```bash
   dotnet run -- migrate
   ```
   *(Skip if `init-db.sql` already created schema)*

5. Build the project:
   ```bash
   dotnet build
   ```

6. Run the API:
   ```bash
   dotnet run
   ```

7. Verify API is running:
   - Open browser: `http://localhost:5000/swagger`
   - You should see Swagger UI with all API endpoints
   - Test GET `/api/team-members` - should return seeded team members

**API Base URL**: `http://localhost:5000/api`

**Troubleshooting**:
- **Connection refused to SQL Server**: Ensure Docker container is running and wait 60s after startup
- **Port 5000 already in use**: Change port in `launchSettings.json` or `appsettings.json`
- **Authentication failed for user 'sa'**: Check password matches `docker-compose.yml` environment variable

### Step 3: Set Up Frontend (Vue 3)

1. Navigate to frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create environment file:
   - Copy `.env.example` to `.env.development`:
     ```bash
     cp .env.example .env.development
     ```
   - Edit `.env.development`:
     ```
     VITE_API_BASE_URL=http://localhost:5000/api
     ```

4. Start development server:
   ```bash
   npm run dev
   ```

5. Open application:
   - Browser: `http://localhost:5173`
   - You should see the Team Daily Goal Tracker dashboard
   - Verify team members are displayed with seeded data

**Development Server**: `http://localhost:5173`

**Troubleshooting**:
- **CORS errors**: Ensure backend CORS policy allows `http://localhost:5173` origin
- **API calls fail**: Check `VITE_API_BASE_URL` in `.env.development` matches backend URL
- **Port 5173 already in use**: Vite will prompt for alternate port, accept or kill conflicting process

## Verifying the Setup

### Test User Story 1: View Team Dashboard

1. Open `http://localhost:5173` in browser
2. **Expected**: See dashboard with team member cards
3. **Verify**:
   - Each card shows team member name
   - Goals are listed (if seeded)
   - Mood emoji is displayed (if seeded)
   - Team stats panel shows completion % and mood distribution

### Test User Story 2: Add a Goal

1. Locate "Add Goal" form on dashboard
2. Select a team member from dropdown
3. Enter goal description (e.g., "Test new feature")
4. Click "Add Goal" button
5. **Expected**: Goal appears in selected team member's card
6. **Verify**: Completion count updates (e.g., "2/3" → "2/4")

### Test User Story 3: Update Mood

1. Locate "Update Mood" form on dashboard
2. Select a team member from dropdown
3. Select a mood emoji (e.g., 😊 Happy)
4. Click "Update Mood" button
5. **Expected**: Mood emoji updates on team member's card
6. **Verify**: Team mood stats update in stats panel

### Test User Story 4: Mark Goal Complete

1. Find an incomplete goal on a team member's card
2. Click the checkbox next to the goal
3. **Expected**: Goal is marked complete (visual distinction: strikethrough or checkmark)
4. **Verify**: Completion count increases (e.g., "2/4" → "3/4")

## Development Workflow

### Running Tests

**Backend Tests** (xUnit):
```bash
cd backend/tests/TeamGoalTracker.Api.Tests
dotnet test
```

**Frontend Unit Tests** (Vitest):
```bash
cd frontend
npm run test
```

**Frontend E2E Tests** (Playwright):
```bash
cd frontend
npm run test:e2e
```

### Code Quality Checks

**Frontend Linting**:
```bash
cd frontend
npm run lint
```

**Frontend Type Check**:
```bash
cd frontend
npm run type-check
```

**Backend Code Analysis**:
```bash
cd backend
dotnet build /p:EnforceCodeStyleInBuild=true
```

### Hot Reload / Watch Mode

- **Backend**: Use `dotnet watch run` for automatic recompilation on file changes
- **Frontend**: Vite dev server (`npm run dev`) has built-in HMR (Hot Module Replacement)

## Common Tasks

### Resetting the Database

```bash
# Stop and remove containers + volumes
cd docker
docker-compose down -v

# Restart (will re-run init-db.sql)
docker-compose up -d
```

### Adding New Team Members (Manual)

```bash
# Connect to SQL Server (via Azure Data Studio, SSMS, or CLI)
docker exec -it team-goal-tracker-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Password' \
  -Q "USE TeamGoalTracker; INSERT INTO TeamMembers (Name) VALUES ('New Member');"
```

### Viewing Database Contents

```bash
# Query team members
docker exec -it team-goal-tracker-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Password' \
  -Q "USE TeamGoalTracker; SELECT * FROM TeamMembers;"

# Query goals
docker exec -it team-goal-tracker-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Password' \
  -Q "USE TeamGoalTracker; SELECT * FROM Goals WHERE Date = CAST(GETUTCDATE() AS DATE);"
```

### Building for Production (Local)

**Backend**:
```bash
cd backend/src/TeamGoalTracker.Api
dotnet publish -c Release -o ../../publish
```

**Frontend**:
```bash
cd frontend
npm run build
# Output in frontend/dist/
```

## Stopping the Application

1. **Frontend**: Press `Ctrl+C` in terminal running `npm run dev`
2. **Backend**: Press `Ctrl+C` in terminal running `dotnet run`
3. **Database** (keep running):
   ```bash
   # No action needed - Docker container continues running
   ```
4. **Database** (stop completely):
   ```bash
   cd docker
   docker-compose stop
   # Or to remove container: docker-compose down
   ```

## Environment Variables Reference

### Backend (`appsettings.Development.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TeamGoalTracker;User=sa;Password=YourStrong!Password;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedOrigins": ["http://localhost:5173"]
}
```

### Frontend (`.env.development`)

```
VITE_API_BASE_URL=http://localhost:5000/api
```

## Architecture Overview

### Request Flow

```
User Browser
    ↓
Vue 3 Dashboard (localhost:5173)
    ↓
Axios API Client (composables)
    ↓
.NET 8 Web API (localhost:5000)
    ↓
Dapper Repository
    ↓
SQL Server (Docker - port 1433)
```

### Data Flow Example: Adding a Goal

1. User fills out "Add Goal" form (component: `AddGoalForm.vue`)
2. Form submit calls `useGoals().createGoal()` composable
3. Composable calls `api.post('/goals', request)`
4. Backend `GoalsController.Create()` receives request
5. Request validated by FluentValidation
6. `GoalService.CreateGoalAsync()` handles business logic
7. `GoalRepository.InsertAsync()` executes Dapper SQL INSERT
8. SQL Server stores goal in Goals table
9. Response flows back: DB → Repository → Service → Controller → API response
10. Frontend receives GoalDto, updates local state
11. Vue reactivity triggers re-render of team member card
12. User sees new goal in card, completion count updates

## Next Steps After Setup

1. **Read the Spec**: Review `specs/001-team-goal-tracker/spec.md` for user stories and requirements
2. **Explore API**: Use Swagger UI (`http://localhost:5000/swagger`) to test endpoints
3. **Review Code Structure**: Examine `plan.md` for architecture decisions
4. **Run All Tests**: Ensure tests pass before making changes
5. **Start Development**: Follow TDD workflow (write tests → fail → implement → pass)

## Troubleshooting Guide

### Database Issues

**Problem**: "Cannot open database 'TeamGoalTracker' requested by the login"
- **Solution**: Database not created; check `init-db.sql` ran successfully; verify with `docker logs`

**Problem**: "A connection was successfully established... but then an error occurred"
- **Solution**: SQL Server not fully started; wait 60 seconds after `docker-compose up`

**Problem**: "Login failed for user 'sa'"
- **Solution**: Password mismatch; check `docker-compose.yml` SA_PASSWORD matches connection string

### API Issues

**Problem**: 404 for all API endpoints
- **Solution**: Check API is running on port 5000; verify base URL in Swagger

**Problem**: 500 Internal Server Error on API calls
- **Solution**: Check API logs for exceptions; verify database connection; check Dapper queries

### Frontend Issues

**Problem**: Blank page, no errors
- **Solution**: Check browser console for errors; verify API is reachable; check CORS

**Problem**: "Network Error" on API calls
- **Solution**: Check `VITE_API_BASE_URL`; verify backend CORS allows origin; check backend is running

**Problem**: Type errors in IDE
- **Solution**: Run `npm run type-check`; ensure TypeScript strict mode issues resolved

### Performance Issues

**Problem**: Dashboard load >2 seconds
- **Solution**: Check SQL query performance; verify indexes exist; check network latency (should be <50ms locally)

**Problem**: Goal completion checkbox laggy
- **Solution**: Check browser DevTools Performance tab; verify no unnecessary re-renders; debounce API calls

## Support Resources

- **Specification**: `specs/001-team-goal-tracker/spec.md`
- **Implementation Plan**: `specs/001-team-goal-tracker/plan.md`
- **API Contracts**: `specs/001-team-goal-tracker/contracts/endpoints.md`
- **Data Model**: `specs/001-team-goal-tracker/data-model.md`
- **Research Notes**: `specs/001-team-goal-tracker/research.md`

## Success Criteria Checklist

After setup, verify these success criteria are met:

- [ ] **SC-001**: Dashboard loads in <2 seconds
- [ ] **SC-002**: Can add a goal and see it appear in <5 seconds
- [ ] **SC-003**: Can update mood and see change in <3 seconds
- [ ] **SC-004**: Goal checkbox updates instantly (<100ms perceived)
- [ ] **SC-008**: Page refresh preserves all goals and moods
- [ ] **SC-010**: Form validation prevents empty goals/unselected members

If all checkboxes pass, your environment is correctly configured and ready for development!
