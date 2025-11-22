# Implementation Plan: Team Daily Goal Tracker

**Branch**: `001-team-goal-tracker` | **Date**: 2025-11-20 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-team-goal-tracker/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Team Daily Goal Tracker is a full-stack web application providing real-time visibility into team members' daily goals and morale. The system enables anyone to add goals, update mood states, mark goals complete, and view aggregated team statistics (completion percentage and mood distribution). The MVP focuses on single-day tracking with a desktop-optimized interface, explicitly excluding authentication, historical data, and mobile responsiveness.

**Technical Approach**: Vue 3 + TypeScript frontend with DaisyUI components, .NET 8 Web API backend using Dapper ORM, SQL Server database on Docker, RESTful API contracts with JSON payloads.

## Technical Context

**Language/Version**:
- Frontend: TypeScript (strict mode), Vue 3 with Composition API
- Backend: C# with .NET 8 Web API

**Primary Dependencies**:
- Frontend: Vue 3, TypeScript, Vite, DaisyUI (Tailwind CSS), Axios
- Backend: .NET 8, Dapper (micro-ORM), SQL Server client

**Storage**: SQL Server (running on Docker)

**Testing**:
- Frontend: Vitest (unit), Playwright (integration/E2E)
- Backend: xUnit (unit), TestServer (integration/contract)

**Target Platform**:
- Frontend: Desktop browsers (Chrome, Firefox, Safari, Edge - latest 2 versions), minimum viewport 1024px
- Backend: Linux/Windows/macOS development environment, .NET 8 runtime

**Project Type**: Web application (separate frontend and backend)

**Performance Goals**:
- Page load: <2 seconds (per SC-001)
- Goal operations: <5 seconds (per SC-002, SC-003)
- Goal completion: <100ms perceived performance (per SC-004)
- Support 20 team members with 5 goals each (per SC-009)

**Constraints**:
- Desktop-only (no responsive mobile design)
- Local development only (no production deployment)
- Single-day data scope (no historical tracking)
- Maximum 20 team members
- No authentication/authorization required

**Scale/Scope**:
- Maximum 20 team members
- Maximum 100 goals total (20 members × 5 goals average)
- Single team context
- Single day of data retention

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. Code Quality Standards - COMPLIANT ✅

**Requirements**:
- TypeScript strict mode enabled (enforces type safety)
- ESLint + Prettier for frontend (Vue 3 + TypeScript rules)
- C# analyzer + StyleCop for backend (.NET conventions)
- Single Responsibility Principle enforced in composables and services
- XML documentation comments for all public APIs (.NET)
- JSDoc comments for exported functions (TypeScript)

**Status**: Architecture supports all requirements. Linting and formatting will be configured during setup.

### II. Test-Driven Development - COMPLIANT ✅

**Requirements**:
- TDD workflow enforced: Write tests → Fail → Implement → Pass
- Target: ≥80% code coverage
- Contract tests for all API endpoints (using TestServer)
- Integration tests for critical user journeys (Playwright E2E)
- Unit tests for business logic (Vitest + xUnit)

**Test Strategy**:
- Contract tests: All 6 API endpoints (team members, goals CRUD, mood update, stats)
- Integration tests: All 4 user stories (view dashboard, add goal, update mood, mark complete)
- Unit tests: Goal completion calculations, mood distribution logic, form validation

**Status**: Test frameworks selected. TDD process will be enforced in task implementation order.

### III. User Experience Consistency - COMPLIANT ✅

**Requirements**:
- DaisyUI design system ensures component consistency
- Standard interaction patterns (form submission, checkbox toggle)
- Error messages follow "what + how to fix" pattern
- Loading states for operations >200ms (goal/mood submission)
- Accessibility: Semantic HTML, ARIA labels, keyboard navigation
- Clear visual separation between team member cards

**Design Decisions**:
- DaisyUI components: `card`, `btn`, `dropdown`, `checkbox`, `badge`, `stats`
- Error handling: Validation messages appear inline near form fields
- Loading states: Button disabled + loading spinner during API calls
- Accessibility: Form labels, ARIA live regions for stats updates

**Status**: DaisyUI provides WCAG-compliant base components. Accessibility review gate required before completion.

### IV. Performance Requirements - COMPLIANT ✅

**Requirements**:
- API p95 <500ms, p99 <1s (constitution) vs. SC-002/SC-003 <5s (more lenient)
- UI interactions <100ms perceived (matches SC-004)
- Page load <3s (constitution) vs. SC-001 <2s (more strict)
- Database: Proper indexing on TeamMemberId, Date columns
- No N+1 queries (Dapper query optimization)

**Performance Strategy**:
- Frontend: Vue 3 reactivity for instant UI updates, debounced stats recalculation
- Backend: Single query for dashboard load (JOIN team members + goals + moods)
- Database: Indexes on TeamMembers.Id, Goals.TeamMemberId, Goals.Date, Moods.TeamMemberId
- Caching: Vue computed properties for stats calculations (client-side)

**Budget**:
- Dashboard load: Target 1.5s (includes DB query + render)
- Goal/Mood API: Target 200ms p95
- Stats calculation: Target 50ms (client-side)

**Status**: Architecture supports performance requirements. Performance tests required before completion.

### V. Security & Privacy - RELAXED (Justified) ⚠️

**Constitution Requirements**:
- No secrets in version control ✅
- Encryption at rest/transit ⚠️ (HTTPS only in production, not required for local dev)
- Industry-standard auth ❌ (Authentication explicitly out of scope per spec)
- Server-side validation ✅ (All input validated in .NET API)
- Vulnerability scanning ✅ (npm audit, dotnet list package --vulnerable)

**Justification for Relaxations**:
1. **No Authentication**: Explicitly out of scope per feature spec. MVP assumes trusted environment.
2. **No Encryption at Rest**: Local development only, SQL Server default settings acceptable.
3. **No HTTPS**: Local development (localhost), HTTPS not required.

**Security Measures Implemented**:
- Input validation: All API endpoints validate request payloads
- SQL injection prevention: Dapper parameterized queries
- XSS prevention: Vue auto-escapes template content
- Dependency scanning: CI/CD checks (when implemented)

**Status**: Relaxations justified by MVP scope. Security review required if authentication added in future.

### Quality Gates Summary

| Gate | Status | Notes |
|------|--------|-------|
| Constitution Compliance | ✅ PASS | Security relaxed per MVP scope |
| Test Gate | 🟡 PENDING | TDD process enforced in tasks.md |
| Code Quality Gate | 🟡 PENDING | Linting configured in setup phase |
| Performance Gate | 🟡 PENDING | Budgets defined, tests required |
| Security Gate | ✅ PASS | Validation + scanning configured |
| UX Gate | 🟡 PENDING | Accessibility review required |
| Documentation Gate | 🟡 PENDING | README + API docs in final phase |

**GATE DECISION**: ✅ **PROCEED TO PHASE 0**

All constitutional requirements satisfied or justified. Pending gates addressed in implementation phases.

## Project Structure

### Documentation (this feature)

```text
specs/001-team-goal-tracker/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
│   ├── openapi.yaml    # OpenAPI 3.0 specification
│   └── endpoints.md    # Endpoint documentation
└── checklists/
    └── requirements.md  # Specification quality checklist (already created)
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── TeamGoalTracker.Api/          # Web API project
│   │   ├── Controllers/              # API controllers
│   │   ├── Models/                   # DTOs and request/response models
│   │   ├── Services/                 # Business logic services
│   │   ├── Data/                     # Dapper repositories
│   │   ├── Validation/               # Input validation
│   │   └── Program.cs                # App entry point
│   └── TeamGoalTracker.Core/         # Shared domain models
│       └── Entities/                 # Domain entities
└── tests/
    ├── TeamGoalTracker.Api.Tests/    # Unit tests
    ├── TeamGoalTracker.Integration/  # Integration + contract tests
    └── test-data/                    # SQL seed scripts

frontend/
├── src/
│   ├── components/                   # Vue components
│   │   ├── TeamMemberCard.vue       # Individual member card
│   │   ├── AddGoalForm.vue          # Goal creation form
│   │   ├── UpdateMoodForm.vue       # Mood update form
│   │   └── TeamStatsPanel.vue       # Aggregated statistics
│   ├── composables/                  # Vue composables
│   │   ├── useTeamMembers.ts        # Team member state management
│   │   ├── useGoals.ts              # Goal CRUD operations
│   │   ├── useMoods.ts              # Mood update operations
│   │   └── useTeamStats.ts          # Stats calculation logic
│   ├── services/                     # API client services
│   │   └── api.ts                   # Axios HTTP client
│   ├── types/                        # TypeScript type definitions
│   │   └── models.ts                # Domain model interfaces
│   ├── views/                        # Page components
│   │   └── Dashboard.vue            # Main dashboard page
│   ├── App.vue                       # Root component
│   └── main.ts                       # App entry point
└── tests/
    ├── unit/                         # Vitest unit tests
    └── e2e/                          # Playwright E2E tests

docker/
├── docker-compose.yml                # SQL Server + optional backend container
└── init-db.sql                       # Database initialization script

docs/
└── architecture.md                   # System architecture overview (optional)
```

**Structure Decision**: Web application structure selected (Option 2 from template). Frontend and backend are separate projects with independent build/run processes. Docker Compose manages SQL Server dependency. This structure supports:
- Independent frontend/backend development
- Clear separation of concerns
- Vue 3 Composition API organization (composables pattern)
- .NET project structure conventions
- Test isolation (frontend/backend test suites independent)

## Complexity Tracking

No constitutional violations requiring justification. Security relaxations are within MVP scope boundaries defined in the feature specification.
