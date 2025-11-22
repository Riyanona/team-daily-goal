# Continue Implementation Guide

## Current Status

**6 out of 52 MVP tasks completed** (11.5% progress)

The project foundation is in place with proper directory structure and .NET projects initialized. Environment configuration issues prevented full automated implementation.

## Immediate Next Steps

### 1. Fix NuGet Configuration (Required)

```bash
# Check current NuGet sources
dotnet nuget list source

# If you see an HTTP source, remove it:
dotnet nuget remove source <source-name>

# Ensure nuget.org is present:
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Restore packages
cd backend/src/TeamGoalTracker.Api
dotnet restore

cd ../TeamGoalTracker.Core
dotnet restore
```

### 2. Initialize Frontend

```bash
cd frontend

# Initialize Vite project (choose "vue-ts" template when prompted)
npm create vite@latest .

# Install dependencies
npm install

# Install additional packages
npm install -D tailwindcss postcss autoprefixer daisyui
npm install axios
npm install -D vitest @vue/test-utils happy-dom
npm install -D @playwright/test

# Initialize Tailwind CSS
npx tailwindcss init -p
```

### 3. Install Backend Dependencies

```bash
cd backend/src/TeamGoalTracker.Api

# Add NuGet packages
dotnet add package Dapper --version 2.1.28
dotnet add package FluentValidation.AspNetCore --version 11.3.0
dotnet add package Microsoft.Data.SqlClient --version 5.1.5
dotnet add package Swashbuckle.AspNetCore --version 6.5.0

# Verify packages installed
dotnet list package
```

### 4. Create Docker Configuration

Create `docker/docker-compose.yml`:

```yaml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: team-goal-tracker-sqlserver
    environment:
      SA_PASSWORD: "YourStrong!Password"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - ./init-db.sql:/docker-entrypoint-initdb.d/init-db.sql
      - sqlserver-data:/var/opt/mssql
volumes:
  sqlserver-data:
```

Create `docker/init-db.sql` (schema from `/specs/001-team-goal-tracker/data-model.md`):

```sql
CREATE DATABASE TeamGoalTracker;
GO
USE TeamGoalTracker;
GO

-- TeamMembers Table
CREATE TABLE TeamMembers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT CK_TeamMembers_Name_NotEmpty CHECK (LEN(TRIM(Name)) > 0)
);

CREATE INDEX IX_TeamMembers_Name ON TeamMembers(Name);

-- Goals Table
CREATE TABLE Goals (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamMemberId INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    Date DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Goals_TeamMember FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Goals_Description_NotEmpty CHECK (LEN(TRIM(Description)) > 0)
);

CREATE INDEX IX_Goals_TeamMemberId_Date ON Goals(TeamMemberId, Date);
CREATE INDEX IX_Goals_Date_IsCompleted ON Goals(Date, IsCompleted);

-- Moods Table
CREATE TABLE Moods (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamMemberId INT NOT NULL,
    MoodType INT NOT NULL,
    Date DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Moods_TeamMember FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Moods_MoodType_Range CHECK (MoodType BETWEEN 1 AND 5),
    CONSTRAINT UQ_Moods_TeamMember_Date UNIQUE (TeamMemberId, Date)
);

CREATE INDEX IX_Moods_TeamMemberId_Date ON Moods(TeamMemberId, Date);
CREATE INDEX IX_Moods_Date ON Moods(Date);

-- Seed Data
INSERT INTO TeamMembers (Name) VALUES
('Alice Johnson'),
('Bob Smith'),
('Charlie Davis'),
('Diana Prince'),
('Eve Wilson');

DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);

INSERT INTO Goals (TeamMemberId, Description, IsCompleted, Date) VALUES
(1, 'Complete project proposal', 0, @Today),
(1, 'Review pull requests', 1, @Today),
(1, 'Update documentation', 0, @Today),
(2, 'Fix bug #123', 1, @Today),
(2, 'Write unit tests', 0, @Today),
(3, 'Design new feature mockups', 0, @Today),
(3, 'Client meeting preparation', 1, @Today),
(4, 'Code review for team', 0, @Today),
(5, 'Sprint planning', 1, @Today);

INSERT INTO Moods (TeamMemberId, MoodType, Date) VALUES
(1, 2, @Today),  -- Alice: Happy
(2, 3, @Today),  -- Bob: Neutral
(3, 1, @Today),  -- Charlie: Very Happy
(4, 4, @Today),  -- Diana: Sad
(5, 5, @Today);  -- Eve: Stressed
```

## Implementation Sequence

After completing setup steps above, follow this sequence:

### Phase 2: Foundational (Tasks T016-T030)

1. **Create Entity Classes**:
   - `backend/src/TeamGoalTracker.Core/Entities/TeamMember.cs`
   - `backend/src/TeamGoalTracker.Core/Entities/Goal.cs`
   - `backend/src/TeamGoalTracker.Core/Entities/Mood.cs`

2. **Create TypeScript Models**:
   - `frontend/src/types/models.ts` (all interfaces + enums)

3. **Backend Infrastructure**:
   - `backend/src/TeamGoalTracker.Api/Data/DbConnectionFactory.cs`
   - `backend/src/TeamGoalTracker.Api/Program.cs` (DI, CORS, middleware)
   - `backend/src/TeamGoalTracker.Api/Models/` (DTOs)

4. **Frontend Infrastructure**:
   - `frontend/src/services/api.ts` (Axios client)
   - `frontend/.env.development` (API URL)
   - `frontend/tailwind.config.js` (DaisyUI setup)
   - `frontend/src/App.vue` and `frontend/src/main.ts`

### Phase 3: User Story 1 - Dashboard (Tasks T031-T052)

**TDD Approach - Write tests FIRST**:

1. **Backend Contract Tests** (T031-T033):
   - Test GET /api/dashboard
   - Test GET /api/team-members
   - Test GET /api/stats

2. **Backend Implementation** (T037-T044):
   - Repositories (TeamMember, Goal, Mood)
   - Services (Dashboard, Stats)
   - Controllers (Dashboard, TeamMembers, Stats)

3. **Frontend E2E Tests** (T035):
   - Test dashboard page load

4. **Frontend Implementation** (T045-T052):
   - Composables (useDashboard, useTeamStats)
   - Components (TeamMemberCard, TeamStatsPanel)
   - Views (Dashboard.vue)

## Testing the MVP

Once implementation is complete:

```bash
# Start SQL Server
cd docker
docker-compose up -d

# Start Backend API
cd ../backend/src/TeamGoalTracker.Api
dotnet run
# API runs on http://localhost:5000

# Start Frontend Dev Server (in new terminal)
cd ../../../frontend
npm run dev
# Frontend runs on http://localhost:5173

# Open browser to http://localhost:5173
# You should see the Team Daily Goal Tracker dashboard!
```

## Validation Checklist

MVP is complete when:

- [ ] Database created with seeded team members
- [ ] Backend API running and accessible at http://localhost:5000/swagger
- [ ] GET /api/dashboard returns team data
- [ ] Frontend displays dashboard with team member cards
- [ ] Team member cards show name, mood emoji, goals, completion count
- [ ] Stats panel shows completion % and mood distribution
- [ ] All tests pass (contract + E2E)
- [ ] Page load time < 2 seconds (per SC-001)

## Reference Documentation

- **Tasks**: `/specs/001-team-goal-tracker/tasks.md` - Complete task breakdown
- **Data Model**: `/specs/001-team-goal-tracker/data-model.md` - Entity definitions
- **API Contracts**: `/specs/001-team-goal-tracker/contracts/endpoints.md` - API spec
- **Quickstart**: `/specs/001-team-goal-tracker/quickstart.md` - Full setup guide

## Need Help?

If you encounter issues:
1. Check `/specs/001-team-goal-tracker/quickstart.md` troubleshooting section
2. Verify all dependencies are installed correctly
3. Ensure Docker is running for SQL Server
4. Check CORS configuration allows localhost:5173

Good luck with the implementation! The foundation is solid and all design decisions are documented.
