# Research: Team Daily Goal Tracker

**Date**: 2025-11-20
**Feature**: Team Daily Goal Tracker
**Purpose**: Document technology decisions, best practices, and architectural patterns for implementation

## Technology Stack Research

### Frontend: Vue 3 + TypeScript + DaisyUI

**Decision**: Vue 3 Composition API with TypeScript strict mode and DaisyUI component library

**Rationale**:
- **Vue 3 Composition API**: Provides better TypeScript support, improved code organization through composables, and performance benefits over Options API
- **TypeScript Strict Mode**: Catches type errors at compile time, improves IDE autocomplete, reduces runtime bugs
- **DaisyUI**: Pre-built Tailwind CSS components reduce custom CSS, ensure design consistency, provide accessibility baseline (WCAG 2.1)
- **Vite**: Fast development server with HMR, optimized production builds, native ES modules

**Alternatives Considered**:
- **React + TypeScript**: More ecosystem libraries, but steeper learning curve for composables-style state management (hooks)
- **Vue 2 Options API**: Simpler syntax but weaker TypeScript integration, no Composition API benefits
- **Plain Tailwind CSS**: More flexibility but requires custom component design, higher development time

**Best Practices**:
1. **Composables Pattern**: Extract reusable logic into `use*` composables (e.g., `useGoals`, `useTeamStats`)
2. **TypeScript Interfaces**: Define all data models in `types/models.ts` for type safety across frontend
3. **Component Structure**: Keep components focused (single responsibility), use props/emits for communication
4. **Reactive State**: Use `ref` for primitives, `reactive` for objects, `computed` for derived values
5. **API Layer Separation**: Centralize HTTP calls in `services/api.ts`, use Axios interceptors for error handling

**References**:
- [Vue 3 Composition API Guide](https://vuejs.org/guide/extras/composition-api-faq.html)
- [DaisyUI Components](https://daisyui.com/components/)
- [TypeScript with Vue](https://vuejs.org/guide/typescript/overview.html)

### Backend: .NET 8 Web API + Dapper

**Decision**: .NET 8 Web API with Dapper micro-ORM for data access

**Rationale**:
- **.NET 8**: Latest LTS version, cross-platform, excellent performance, minimal API support
- **Dapper**: Lightweight ORM with full SQL control, better performance than EF Core for simple CRUD, explicit queries prevent N+1 issues
- **Web API Template**: Built-in OpenAPI/Swagger support, JSON serialization, dependency injection
- **Minimal Complexity**: No need for EF Core migrations, change tracking, or lazy loading for this simple data model

**Alternatives Considered**:
- **Entity Framework Core**: Full ORM with migrations, but overkill for 3-table schema and adds complexity
- **Node.js + Express**: Unified JavaScript stack, but team specified .NET 8 constraint
- **ASP.NET MVC**: Full framework with Razor views, but unnecessary for API-only backend

**Best Practices**:
1. **Repository Pattern**: Wrap Dapper in repository classes for testability and abstraction
2. **Service Layer**: Business logic in service classes (e.g., `GoalService`, `StatsService`), controllers stay thin
3. **DTOs**: Separate request/response models from domain entities, use AutoMapper or manual mapping
4. **Validation**: FluentValidation for request validation, return 400 Bad Request with error details
5. **Error Handling**: Global exception handler middleware, structured error responses
6. **Dependency Injection**: Register services with appropriate lifetime (Scoped for repos, Singleton for stats cache)

**SQL Query Strategy**:
- Dashboard load: Single JOIN query fetching team members + goals + moods (avoid N+1)
- Goal/Mood updates: Simple parameterized INSERT/UPDATE
- Stats calculation: Option 1 - Calculate in C# (simple logic), Option 2 - SQL aggregations (if performance needed)

**References**:
- [Dapper Official Docs](https://github.com/DapperLib/Dapper)
- [.NET 8 Web API Best Practices](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Minimal APIs vs Controllers](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)

### Database: SQL Server on Docker

**Decision**: SQL Server 2022 running in Docker container for local development

**Rationale**:
- **SQL Server**: Robust relational database, excellent tooling (SSMS, Azure Data Studio), strong .NET integration
- **Docker**: Consistent environment across dev machines, easy setup/teardown, no local SQL Server installation required
- **Schema Simplicity**: 3 tables (TeamMembers, Goals, Moods), simple relationships, no complex queries

**Alternatives Considered**:
- **PostgreSQL**: Open-source, but SQL Server specified in constraints
- **SQLite**: File-based, simpler setup, but lacks concurrency and SQL Server-specific features
- **Azure SQL Database**: Cloud-hosted, but local dev requirement rules this out

**Schema Design**:
- **TeamMembers**: Id (PK), Name, CreatedAt
- **Goals**: Id (PK), TeamMemberId (FK), Description, IsCompleted, Date, CreatedAt
- **Moods**: Id (PK), TeamMemberId (FK), MoodType (enum: 1-5), Date, UpdatedAt

**Indexing Strategy**:
- Primary keys: Clustered indexes on Id columns
- Foreign keys: Non-clustered indexes on TeamMemberId
- Date filtering: Non-clustered index on Goals.Date, Moods.Date (for single-day queries)

**Best Practices**:
1. **Initialization Script**: `init-db.sql` creates schema + seeds sample team members
2. **Docker Compose**: Define SQL Server service with volume for data persistence
3. **Connection String**: Use environment variables, never commit credentials
4. **Migrations**: Simple SQL scripts (no EF migrations), version-controlled in `tests/test-data/`

**References**:
- [SQL Server Docker Image](https://hub.docker.com/_/microsoft-mssql-server)
- [Dapper + SQL Server Performance](https://www.learndapper.com/dapper-query)

## Testing Strategy

### Frontend Testing: Vitest + Playwright

**Decision**: Vitest for unit tests, Playwright for E2E integration tests

**Rationale**:
- **Vitest**: Fast, Vite-native test runner, compatible with Vue Test Utils, TypeScript support
- **Playwright**: Cross-browser E2E testing, headless mode for CI, excellent debugging tools
- **Separation**: Unit tests for composables/utils (fast feedback), E2E for user journeys (confidence)

**Test Coverage Goals**:
- Composables: Test state management, API error handling, stats calculations (>80% coverage)
- Components: Test props/emits, user interactions (form submission, checkbox toggle)
- E2E: Test all 4 user stories end-to-end (view, add, update, mark complete)

**Best Practices**:
1. **Test File Structure**: Co-locate tests with source (`*.spec.ts` next to `*.ts`)
2. **Mock API Calls**: Use MSW (Mock Service Worker) or Vitest mocks for HTTP requests
3. **E2E Test Data**: Seed database with known data before each test, clean up after
4. **Accessibility Testing**: Playwright axe-core integration for WCAG checks

**References**:
- [Vitest Guide](https://vitest.dev/guide/)
- [Vue Test Utils](https://test-utils.vuejs.org/)
- [Playwright for Vue](https://playwright.dev/docs/test-components)

### Backend Testing: xUnit + TestServer

**Decision**: xUnit for unit tests, TestServer for integration/contract tests

**Rationale**:
- **xUnit**: .NET standard, parallel test execution, rich assertion library (FluentAssertions)
- **TestServer**: In-memory HTTP server for testing APIs without deployment, fast integration tests
- **Contract Tests**: Validate request/response contracts match OpenAPI spec

**Test Coverage Goals**:
- Repositories: Test Dapper queries with in-memory database or test database
- Services: Test business logic, validation, error cases (>80% coverage)
- Controllers: Contract tests for all 6 endpoints (request validation, response format, status codes)

**Best Practices**:
1. **Test Database**: Use Docker SQL Server with test schema, reset between test runs
2. **Fixtures**: xUnit class/collection fixtures for shared test setup (DB connection, TestServer)
3. **Arrange-Act-Assert**: Clear test structure, one assertion per test when possible
4. **Integration Tests**: Test full request/response cycle (controller → service → repository → DB)

**References**:
- [xUnit Documentation](https://xunit.net/)
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)

## Architecture Patterns

### Frontend Architecture: Composables + Services

**Pattern**: Composition API with reusable composables for state management, services for HTTP communication

**Structure**:
```
composables/
  useTeamMembers.ts  → Manages team member list state
  useGoals.ts        → Manages goal CRUD operations
  useMoods.ts        → Manages mood updates
  useTeamStats.ts    → Calculates completion % and mood distribution

services/
  api.ts             → Axios client with interceptors
```

**Data Flow**:
1. Component calls composable function (e.g., `addGoal`)
2. Composable calls service (e.g., `api.post('/goals')`)
3. Service returns promise with typed response
4. Composable updates reactive state
5. Component re-renders automatically (Vue reactivity)

**State Management**: No Pinia/Vuex needed - composables provide sufficient state sharing for this small app

**Best Practices**:
- Keep composables focused (single responsibility)
- Return reactive refs/computed from composables
- Handle loading/error states within composables
- Use TypeScript generics for API response types

### Backend Architecture: Service Layer + Repository Pattern

**Pattern**: Controller → Service → Repository → Database

**Structure**:
```
Controllers/
  TeamMembersController  → GET /api/team-members
  GoalsController        → POST /api/goals, PATCH /api/goals/{id}/complete
  MoodsController        → PUT /api/moods
  StatsController        → GET /api/stats

Services/
  GoalService            → Business logic for goal operations
  MoodService            → Business logic for mood updates
  StatsService           → Stats calculation logic

Data/Repositories/
  TeamMemberRepository   → Dapper queries for team members
  GoalRepository         → Dapper queries for goals
  MoodRepository         → Dapper queries for moods
```

**Rationale**:
- **Controllers**: Thin, handle HTTP concerns (routing, validation, status codes)
- **Services**: Business logic, orchestration, domain rules
- **Repositories**: Data access abstraction, Dapper SQL queries

**Dependency Injection**:
- Controllers: Scoped (per request)
- Services: Scoped (per request)
- Repositories: Scoped (per request, ensures connection disposal)

**Best Practices**:
- Controllers return `ActionResult<T>` for typed responses
- Services throw domain exceptions (e.g., `NotFoundException`), caught by middleware
- Repositories return domain entities, services map to DTOs

## Performance Optimization

### Database Query Optimization

**Critical Query**: Dashboard load (all team members + goals + moods)

**Optimized Approach**:
```sql
-- Single query with JOINs (avoid N+1)
SELECT tm.Id, tm.Name,
       g.Id, g.Description, g.IsCompleted, g.Date,
       m.MoodType, m.UpdatedAt
FROM TeamMembers tm
LEFT JOIN Goals g ON tm.Id = g.TeamMemberId AND g.Date = @Today
LEFT JOIN Moods m ON tm.Id = m.TeamMemberId AND m.Date = @Today
ORDER BY tm.Name, g.CreatedAt
```

**Dapper Multi-Mapping**:
Use `QueryAsync<TeamMember, Goal, Mood, TeamMemberDTO>` to map results into nested objects

**Indexes**:
- `Goals.TeamMemberId` + `Goals.Date` (composite index for filtered joins)
- `Moods.TeamMemberId` + `Moods.Date` (composite index for filtered joins)

### Frontend Performance

**Vue 3 Optimizations**:
- **Computed Properties**: Stats calculations cached and only recalculated when dependencies change
- **Debouncing**: Stats recalculation debounced to avoid excessive computation on rapid goal updates
- **Virtual Scrolling**: Not needed for 20 team members (below threshold)
- **Code Splitting**: Lazy-load Playwright E2E test setup, not needed for single-page dashboard

**Performance Budget**:
- Initial bundle size: <500KB (Vue + DaisyUI + Axios)
- Time to Interactive: <1.5s (includes API call + render)

## API Design

### RESTful Endpoints

| Method | Endpoint | Purpose | Request Body | Response |
|--------|----------|---------|--------------|----------|
| GET | `/api/team-members` | Get all team members | - | `TeamMemberDTO[]` |
| POST | `/api/goals` | Add a goal | `CreateGoalRequest` | `GoalDTO` |
| PATCH | `/api/goals/{id}/complete` | Mark goal complete | - | `GoalDTO` |
| PUT | `/api/moods` | Update mood | `UpdateMoodRequest` | `MoodDTO` |
| GET | `/api/stats` | Get team stats | Query: `date` | `TeamStatsDTO` |
| GET | `/api/dashboard` | Get full dashboard | Query: `date` | `DashboardDTO` |

**Note**: `/api/dashboard` is an optimized endpoint for initial page load (single request vs. multiple)

**Request/Response Models**:
```csharp
// Request DTOs
public record CreateGoalRequest(int TeamMemberId, string Description);
public record UpdateMoodRequest(int TeamMemberId, int MoodType);

// Response DTOs
public record GoalDTO(int Id, int TeamMemberId, string Description, bool IsCompleted, DateTime Date);
public record MoodDTO(int TeamMemberId, int MoodType, DateTime UpdatedAt);
public record TeamStatsDTO(double CompletionPercentage, Dictionary<int, int> MoodDistribution);
public record DashboardDTO(List<TeamMemberDTO> TeamMembers, TeamStatsDTO Stats);
```

### Error Handling

**HTTP Status Codes**:
- `200 OK`: Successful GET
- `201 Created`: Successful POST
- `204 No Content`: Successful PATCH/PUT with no response body
- `400 Bad Request`: Validation error (with error details)
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Unexpected error

**Error Response Format**:
```json
{
  "type": "ValidationError",
  "title": "Invalid request",
  "status": 400,
  "errors": {
    "Description": ["Description cannot be empty"]
  }
}
```

## Development Workflow

### Local Setup

1. **Prerequisites**: Docker, Node.js 18+, .NET 8 SDK
2. **Database**: `docker-compose up -d` (starts SQL Server)
3. **Backend**: `cd backend && dotnet run` (starts API on http://localhost:5000)
4. **Frontend**: `cd frontend && npm run dev` (starts Vite on http://localhost:5173)

### Environment Variables

**Backend** (`appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TeamGoalTracker;User=sa;Password=YourStrong!Password;TrustServerCertificate=True"
  }
}
```

**Frontend** (`.env.development`):
```
VITE_API_BASE_URL=http://localhost:5000
```

**Security Note**: Never commit connection strings with passwords. Use user secrets or environment variables.

### Code Quality Tools

**Frontend**:
- ESLint: Vue 3 + TypeScript rules, runs on save
- Prettier: Auto-formatting, integrated with ESLint
- TypeScript: Strict mode, compile errors block build

**Backend**:
- StyleCop: C# style analyzer, enforces naming conventions
- .NET Analyzer: Code quality checks (nullability, async/await)
- SonarLint: IDE integration for code smells

## Deployment (Local Dev)

### Docker Compose

**Services**:
- `sqlserver`: SQL Server 2022 image, port 1433, volume for data persistence
- `backend` (optional): .NET API in container, depends on sqlserver
- `frontend` (optional): Nginx serving built Vue app

**Development Mode**: Run SQL Server in Docker, backend/frontend directly on host (easier debugging)

**Production-Like Mode**: All services in Docker Compose (tests deployment configuration)

### Database Initialization

**init-db.sql** (run on first startup):
```sql
CREATE DATABASE TeamGoalTracker;
GO
USE TeamGoalTracker;
GO

-- Create tables
CREATE TABLE TeamMembers (...);
CREATE TABLE Goals (...);
CREATE TABLE Moods (...);

-- Create indexes
CREATE INDEX IX_Goals_TeamMemberId_Date ON Goals(TeamMemberId, Date);

-- Seed data
INSERT INTO TeamMembers (Name) VALUES ('Alice'), ('Bob'), ('Charlie');
```

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| SQL Server Docker fails to start | Blocks development | Document common issues (port conflict, WSL2), provide troubleshooting guide |
| CORS issues frontend → backend | API calls fail | Configure CORS in .NET API, allow localhost:5173 origin |
| TypeScript strict mode errors | Slows development | Incremental adoption, use `any` sparingly with `// @ts-ignore` comments |
| Dapper query bugs (wrong SQL) | Data errors | Test all queries with integration tests, use raw SQL for transparency |
| Performance < success criteria | Fails gates | Monitor with browser DevTools, SQL Server profiler, optimize queries early |

## Summary

All technology decisions documented and aligned with user constraints (Vue 3, .NET 8, Dapper, SQL Server). Architecture patterns selected for simplicity and testability (composables, service layer, repository pattern). Performance strategy defined (optimized queries, Vue reactivity). Testing strategy covers unit, integration, and E2E tests. No NEEDS CLARIFICATION items remaining - ready for Phase 1 design.
