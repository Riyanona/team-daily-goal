---
description: "Implementation tasks for Team Daily Goal Tracker"
---

# Tasks: Team Daily Goal Tracker

**Input**: Design documents from `/specs/001-team-goal-tracker/`
**Prerequisites**: plan.md, spec.md, data-model.md, contracts/, research.md, quickstart.md

**Tests**: TDD approach enforced per constitution - tests written FIRST, must FAIL, then implement

**Organization**: Tasks grouped by user story to enable independent implementation and testing

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

- **Web app**: `backend/src/`, `frontend/src/`
- **Tests**: `backend/tests/`, `frontend/tests/`
- **Docker**: `docker/`
- Paths are relative to repository root

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 [P] Create backend directory structure (backend/src/TeamGoalTracker.Api/, backend/src/TeamGoalTracker.Core/, backend/tests/)
- [ ] T002 [P] Initialize .NET 8 Web API project in backend/src/TeamGoalTracker.Api/
- [ ] T003 [P] Initialize .NET class library project in backend/src/TeamGoalTracker.Core/
- [ ] T004 [P] Create frontend directory structure (frontend/src/components/, frontend/src/composables/, frontend/src/services/, frontend/src/types/, frontend/src/views/)
- [ ] T005 [P] Initialize Vue 3 + TypeScript project with Vite in frontend/
- [ ] T006 [P] Install backend dependencies (Dapper, FluentValidation, Microsoft.Data.SqlClient) in backend/src/TeamGoalTracker.Api/TeamGoalTracker.Api.csproj
- [ ] T007 [P] Install frontend dependencies (Vue 3, DaisyUI, Axios, TypeScript) in frontend/package.json
- [ ] T008 [P] Configure TypeScript strict mode in frontend/tsconfig.json
- [ ] T009 [P] Configure ESLint and Prettier for Vue 3 + TypeScript in frontend/.eslintrc.js and frontend/.prettierrc
- [ ] T010 [P] Configure backend test project (xUnit) in backend/tests/TeamGoalTracker.Api.Tests/
- [ ] T011 [P] Configure backend integration test project in backend/tests/TeamGoalTracker.Integration/
- [ ] T012 [P] Configure frontend test setup (Vitest) in frontend/vite.config.ts and frontend/tests/setup.ts
- [ ] T013 [P] Configure Playwright E2E tests in frontend/playwright.config.ts
- [ ] T014 [P] Create Docker Compose file for SQL Server in docker/docker-compose.yml
- [ ] T015 Create database initialization script in docker/init-db.sql with schema for TeamMembers, Goals, Moods tables and seed data

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Database & Core Entities

- [ ] T016 Define TeamMember entity in backend/src/TeamGoalTracker.Core/Entities/TeamMember.cs
- [ ] T017 Define Goal entity in backend/src/TeamGoalTracker.Core/Entities/Goal.cs
- [ ] T018 Define Mood entity and MoodType enum in backend/src/TeamGoalTracker.Core/Entities/Mood.cs
- [ ] T019 [P] Define TypeScript models (TeamMember, Goal, Mood, MoodType enum) in frontend/src/types/models.ts
- [ ] T020 [P] Define mood emoji mappings (MOOD_EMOJIS, MOOD_LABELS) in frontend/src/types/models.ts

### Backend Infrastructure

- [ ] T021 Create Dapper connection factory in backend/src/TeamGoalTracker.Api/Data/DbConnectionFactory.cs
- [ ] T022 Configure dependency injection and services in backend/src/TeamGoalTracker.Api/Program.cs
- [ ] T023 Configure CORS policy (allow localhost:5173) in backend/src/TeamGoalTracker.Api/Program.cs
- [ ] T024 Create global exception handler middleware in backend/src/TeamGoalTracker.Api/Middleware/ExceptionHandlerMiddleware.cs
- [ ] T025 Define DTOs (CreateGoalRequest, UpdateMoodRequest, GoalDto, MoodDto, TeamMemberDto, TeamStatsDto, DashboardResponse) in backend/src/TeamGoalTracker.Api/Models/

### Frontend Infrastructure

- [ ] T026 Configure Axios base client with interceptors in frontend/src/services/api.ts
- [ ] T027 Create environment variables setup (.env.development) with VITE_API_BASE_URL in frontend/
- [ ] T028 Configure Tailwind CSS and DaisyUI in frontend/tailwind.config.js and frontend/src/main.ts
- [ ] T029 Create App.vue root component with basic layout in frontend/src/App.vue
- [ ] T030 Create main.ts entry point with Vue app initialization in frontend/src/main.ts

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - View Team Dashboard (Priority: P1) 🎯 MVP

**Goal**: Display all team members with their goals, moods, and team statistics in a single dashboard view

**Independent Test**: Load dashboard and verify all team members are displayed with goals, moods, and stats

### Tests for User Story 1 (TDD - Write FIRST, Ensure FAIL)

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T031 [P] [US1] Contract test for GET /api/dashboard endpoint in backend/tests/TeamGoalTracker.Integration/DashboardControllerTests.cs
- [ ] T032 [P] [US1] Contract test for GET /api/team-members endpoint in backend/tests/TeamGoalTracker.Integration/TeamMembersControllerTests.cs
- [ ] T033 [P] [US1] Contract test for GET /api/stats endpoint in backend/tests/TeamGoalTracker.Integration/StatsControllerTests.cs
- [ ] T034 [P] [US1] Unit test for stats calculation logic in backend/tests/TeamGoalTracker.Api.Tests/Services/StatsServiceTests.cs
- [ ] T035 [P] [US1] E2E test for dashboard page load in frontend/tests/e2e/dashboard.spec.ts
- [ ] T036 [P] [US1] Unit test for useTeamStats composable in frontend/tests/unit/composables/useTeamStats.spec.ts

### Backend Implementation for User Story 1

- [ ] T037 [P] [US1] Create TeamMemberRepository with GetAllAsync method in backend/src/TeamGoalTracker.Api/Data/Repositories/TeamMemberRepository.cs
- [ ] T038 [P] [US1] Create GoalRepository with GetByDateAsync method in backend/src/TeamGoalTracker.Api/Data/Repositories/GoalRepository.cs
- [ ] T039 [P] [US1] Create MoodRepository with GetByDateAsync method in backend/src/TeamGoalTracker.Api/Data/Repositories/MoodRepository.cs
- [ ] T040 [US1] Create StatsService with CalculateCompletionPercentage and CalculateMoodDistribution methods in backend/src/TeamGoalTracker.Api/Services/StatsService.cs
- [ ] T041 [US1] Create DashboardService with GetDashboardDataAsync method (optimized query with JOIN) in backend/src/TeamGoalTracker.Api/Services/DashboardService.cs
- [ ] T042 [P] [US1] Implement GET /api/team-members endpoint in backend/src/TeamGoalTracker.Api/Controllers/TeamMembersController.cs
- [ ] T043 [P] [US1] Implement GET /api/stats endpoint in backend/src/TeamGoalTracker.Api/Controllers/StatsController.cs
- [ ] T044 [US1] Implement GET /api/dashboard endpoint in backend/src/TeamGoalTracker.Api/Controllers/DashboardController.cs

### Frontend Implementation for User Story 1

- [ ] T045 [P] [US1] Create useTeamStats composable with calculateCompletionPercentage and calculateMoodDistribution in frontend/src/composables/useTeamStats.ts
- [ ] T046 [P] [US1] Create useDashboard composable with loadDashboard API call in frontend/src/composables/useDashboard.ts
- [ ] T047 [P] [US1] Create TeamMemberCard component in frontend/src/components/TeamMemberCard.vue
- [ ] T048 [P] [US1] Create TeamStatsPanel component in frontend/src/components/TeamStatsPanel.vue
- [ ] T049 [US1] Create Dashboard view with team member cards grid and stats panel in frontend/src/views/Dashboard.vue
- [ ] T050 [US1] Add routing to Dashboard view in frontend/src/main.ts (if using Vue Router) or set as main view in App.vue
- [ ] T051 [US1] Add loading states and error handling to Dashboard view in frontend/src/views/Dashboard.vue
- [ ] T052 [US1] Add accessibility attributes (ARIA labels, semantic HTML) to TeamMemberCard and TeamStatsPanel components

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently - Dashboard displays all team data

---

## Phase 4: User Story 2 - Add Goals (Priority: P2)

**Goal**: Enable users to add new daily goals for team members

**Independent Test**: Submit add goal form and verify goal appears in team member's card

### Tests for User Story 2 (TDD - Write FIRST, Ensure FAIL)

- [ ] T053 [P] [US2] Contract test for POST /api/goals endpoint in backend/tests/TeamGoalTracker.Integration/GoalsControllerTests.cs
- [ ] T054 [P] [US2] Unit test for CreateGoalRequest validation in backend/tests/TeamGoalTracker.Api.Tests/Validation/CreateGoalRequestValidatorTests.cs
- [ ] T055 [P] [US2] Unit test for GoalService.CreateGoalAsync in backend/tests/TeamGoalTracker.Api.Tests/Services/GoalServiceTests.cs
- [ ] T056 [P] [US2] E2E test for add goal workflow in frontend/tests/e2e/add-goal.spec.ts
- [ ] T057 [P] [US2] Unit test for useGoals composable createGoal method in frontend/tests/unit/composables/useGoals.spec.ts

### Backend Implementation for User Story 2

- [ ] T058 [US2] Add InsertAsync method to GoalRepository in backend/src/TeamGoalTracker.Api/Data/Repositories/GoalRepository.cs
- [ ] T059 [US2] Create CreateGoalRequestValidator with FluentValidation rules in backend/src/TeamGoalTracker.Api/Validation/CreateGoalRequestValidator.cs
- [ ] T060 [US2] Create GoalService with CreateGoalAsync method in backend/src/TeamGoalTracker.Api/Services/GoalService.cs
- [ ] T061 [US2] Implement POST /api/goals endpoint in backend/src/TeamGoalTracker.Api/Controllers/GoalsController.cs
- [ ] T062 [US2] Add error handling for validation failures (400 response) in GoalsController

### Frontend Implementation for User Story 2

- [ ] T063 [P] [US2] Create frontend validation function (validateGoalRequest) in frontend/src/types/models.ts
- [ ] T064 [P] [US2] Create useGoals composable with createGoal method in frontend/src/composables/useGoals.ts
- [ ] T065 [US2] Create AddGoalForm component with team member dropdown and description input in frontend/src/components/AddGoalForm.vue
- [ ] T066 [US2] Add form validation (client-side) to AddGoalForm component in frontend/src/components/AddGoalForm.vue
- [ ] T067 [US2] Add loading states and error messages to AddGoalForm in frontend/src/components/AddGoalForm.vue
- [ ] T068 [US2] Integrate AddGoalForm into Dashboard view in frontend/src/views/Dashboard.vue
- [ ] T069 [US2] Update Dashboard state after goal creation (refresh or optimistic update) in frontend/src/views/Dashboard.vue

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently - Can view dashboard and add goals

---

## Phase 5: User Story 3 - Update Mood (Priority: P2)

**Goal**: Enable users to log and update team member moods

**Independent Test**: Select team member and mood, verify mood updates on dashboard card

### Tests for User Story 3 (TDD - Write FIRST, Ensure FAIL)

- [ ] T070 [P] [US3] Contract test for PUT /api/moods endpoint in backend/tests/TeamGoalTracker.Integration/MoodsControllerTests.cs
- [ ] T071 [P] [US3] Unit test for UpdateMoodRequest validation in backend/tests/TeamGoalTracker.Api.Tests/Validation/UpdateMoodRequestValidatorTests.cs
- [ ] T072 [P] [US3] Unit test for MoodService.UpdateMoodAsync (upsert logic) in backend/tests/TeamGoalTracker.Api.Tests/Services/MoodServiceTests.cs
- [ ] T073 [P] [US3] E2E test for update mood workflow in frontend/tests/e2e/update-mood.spec.ts
- [ ] T074 [P] [US3] Unit test for useMoods composable updateMood method in frontend/tests/unit/composables/useMoods.spec.ts

### Backend Implementation for User Story 3

- [ ] T075 [P] [US3] Add UpsertAsync method to MoodRepository (INSERT or UPDATE) in backend/src/TeamGoalTracker.Api/Data/Repositories/MoodRepository.cs
- [ ] T076 [US3] Create UpdateMoodRequestValidator with FluentValidation rules in backend/src/TeamGoalTracker.Api/Validation/UpdateMoodRequestValidator.cs
- [ ] T077 [US3] Create MoodService with UpdateMoodAsync method in backend/src/TeamGoalTracker.Api/Services/MoodService.cs
- [ ] T078 [US3] Implement PUT /api/moods endpoint in backend/src/TeamGoalTracker.Api/Controllers/MoodsController.cs
- [ ] T079 [US3] Add error handling for validation failures (400 response) in MoodsController

### Frontend Implementation for User Story 3

- [ ] T080 [P] [US3] Create frontend validation function (validateMoodRequest) in frontend/src/types/models.ts
- [ ] T081 [P] [US3] Create useMoods composable with updateMood method in frontend/src/composables/useMoods.ts
- [ ] T082 [US3] Create UpdateMoodForm component with team member dropdown and mood emoji selector in frontend/src/components/UpdateMoodForm.vue
- [ ] T083 [US3] Add loading states and error messages to UpdateMoodForm in frontend/src/components/UpdateMoodForm.vue
- [ ] T084 [US3] Integrate UpdateMoodForm into Dashboard view in frontend/src/views/Dashboard.vue
- [ ] T085 [US3] Update Dashboard state after mood update (refresh or optimistic update) in frontend/src/views/Dashboard.vue
- [ ] T086 [US3] Update TeamStatsPanel to reflect new mood distribution in frontend/src/components/TeamStatsPanel.vue

**Checkpoint**: At this point, User Stories 1, 2, AND 3 should all work independently - Can view, add goals, and update moods

---

## Phase 6: User Story 4 - Mark Goals Complete (Priority: P3)

**Goal**: Enable users to mark goals as complete and see progress updates

**Independent Test**: Check goal completion checkbox, verify completion count and stats update

### Tests for User Story 4 (TDD - Write FIRST, Ensure FAIL)

- [ ] T087 [P] [US4] Contract test for PATCH /api/goals/{id}/complete endpoint in backend/tests/TeamGoalTracker.Integration/GoalsControllerTests.cs
- [ ] T088 [P] [US4] Unit test for GoalService.CompleteGoalAsync in backend/tests/TeamGoalTracker.Api.Tests/Services/GoalServiceTests.cs
- [ ] T089 [P] [US4] E2E test for mark goal complete workflow in frontend/tests/e2e/complete-goal.spec.ts
- [ ] T090 [P] [US4] Unit test for useGoals composable completeGoal method in frontend/tests/unit/composables/useGoals.spec.ts

### Backend Implementation for User Story 4

- [ ] T091 [US4] Add UpdateCompletionStatusAsync method to GoalRepository in backend/src/TeamGoalTracker.Api/Data/Repositories/GoalRepository.cs
- [ ] T092 [US4] Add CompleteGoalAsync method to GoalService in backend/src/TeamGoalTracker.Api/Services/GoalService.cs
- [ ] T093 [US4] Implement PATCH /api/goals/{id}/complete endpoint in backend/src/TeamGoalTracker.Api/Controllers/GoalsController.cs
- [ ] T094 [US4] Add error handling for goal not found (404 response) in GoalsController

### Frontend Implementation for User Story 4

- [ ] T095 [P] [US4] Add completeGoal method to useGoals composable in frontend/src/composables/useGoals.ts
- [ ] T096 [US4] Add completion checkbox to goal items in TeamMemberCard component in frontend/src/components/TeamMemberCard.vue
- [ ] T097 [US4] Add visual distinction for completed goals (strikethrough or checkmark) in TeamMemberCard component in frontend/src/components/TeamMemberCard.vue
- [ ] T098 [US4] Add goal completion count display (e.g., "2/3") to TeamMemberCard component in frontend/src/components/TeamMemberCard.vue
- [ ] T099 [US4] Update Dashboard state after goal completion (refresh or optimistic update) in frontend/src/views/Dashboard.vue
- [ ] T100 [US4] Update TeamStatsPanel to reflect new completion percentage in frontend/src/components/TeamStatsPanel.vue

**Checkpoint**: All user stories should now be independently functional - Complete goal tracking lifecycle

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories and final quality gates

- [ ] T101 [P] Add comprehensive JSDoc comments to all exported TypeScript functions in frontend/src/
- [ ] T102 [P] Add XML documentation comments to all public C# APIs in backend/src/TeamGoalTracker.Api/
- [ ] T103 [P] Add accessibility review for keyboard navigation in all frontend components
- [ ] T104 [P] Add ARIA live regions for dynamic stats updates in TeamStatsPanel in frontend/src/components/TeamStatsPanel.vue
- [ ] T105 [P] Performance testing for dashboard load (target <2s) using browser DevTools
- [ ] T106 [P] Performance testing for API endpoints (target <500ms p95) using k6 or similar
- [ ] T107 [P] Run frontend linting (ESLint) and fix all issues in frontend/
- [ ] T108 [P] Run backend code analysis (StyleCop) and fix all issues in backend/
- [ ] T109 [P] Create README.md with project overview and quickstart at repository root
- [ ] T110 [P] Verify all tests pass with ≥80% code coverage (frontend + backend)
- [ ] T111 Run complete E2E test suite for all user stories in frontend/tests/e2e/
- [ ] T112 Final constitution compliance check against all 7 quality gates
- [ ] T113 Create demo seed data for presentation in docker/init-db.sql

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-6)**: All depend on Foundational phase completion
  - User stories can proceed in parallel (if staffed) after Phase 2
  - Or sequentially in priority order (US1 → US2 → US3 → US4)
- **Polish (Phase 7)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Independent of US1 but builds on dashboard view
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - Independent of US1/US2 but builds on dashboard view
- **User Story 4 (P3)**: Can start after Foundational (Phase 2) - Requires Goal entity from US2 but can be independently tested

### Within Each User Story

- Tests MUST be written and FAIL before implementation (TDD)
- Backend: Repositories before Services before Controllers
- Frontend: Composables before Components before Views
- Tests for each layer can run in parallel (marked with [P])
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks (T001-T015) can run in parallel
- All Foundational entity definitions (T016-T020) can run in parallel
- All Foundational infrastructure (T021-T030) can run in parallel
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- All repositories within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1 (Dashboard)

```bash
# Launch all contract tests for User Story 1 together (TDD - write first):
Task T031: Contract test for GET /api/dashboard
Task T032: Contract test for GET /api/team-members
Task T033: Contract test for GET /api/stats

# Launch all repositories for User Story 1 together (after tests fail):
Task T037: TeamMemberRepository.GetAllAsync
Task T038: GoalRepository.GetByDateAsync
Task T039: MoodRepository.GetByDateAsync

# Launch all controllers for User Story 1 together (after services complete):
Task T042: GET /api/team-members endpoint
Task T043: GET /api/stats endpoint

# Launch all frontend components for User Story 1 together:
Task T047: TeamMemberCard component
Task T048: TeamStatsPanel component
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T015)
2. Complete Phase 2: Foundational (T016-T030) - CRITICAL, blocks all stories
3. Complete Phase 3: User Story 1 (T031-T052)
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready - **This is the MVP!**

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP - Dashboard viewing)
3. Add User Story 2 → Test independently → Deploy/Demo (Goal creation added)
4. Add User Story 3 → Test independently → Deploy/Demo (Mood tracking added)
5. Add User Story 4 → Test independently → Deploy/Demo (Goal completion added)
6. Add Polish → Final quality gates → Production-ready

Each story adds value without breaking previous stories.

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together (T001-T030)
2. Once Foundational is done:
   - Developer A: User Story 1 (T031-T052)
   - Developer B: User Story 2 (T053-T069)
   - Developer C: User Story 3 (T070-T086)
   - Developer D: User Story 4 (T087-T100)
3. Stories complete and integrate independently
4. Team completes Polish together (T101-T113)

---

## Notes

- **[P]** tasks = different files, no dependencies - can execute in parallel
- **[Story]** label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- **TDD Enforced**: Write tests FIRST, verify they FAIL, then implement
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Target: ≥80% code coverage (constitution requirement)
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence

---

## Task Count Summary

- **Phase 1 (Setup)**: 15 tasks
- **Phase 2 (Foundational)**: 15 tasks
- **Phase 3 (US1 - Dashboard)**: 22 tasks (6 tests + 16 implementation)
- **Phase 4 (US2 - Add Goals)**: 17 tasks (5 tests + 12 implementation)
- **Phase 5 (US3 - Update Mood)**: 17 tasks (5 tests + 12 implementation)
- **Phase 6 (US4 - Complete Goals)**: 14 tasks (4 tests + 10 implementation)
- **Phase 7 (Polish)**: 13 tasks

**Total**: 113 tasks

**Parallel Opportunities**: 45 tasks marked [P] can run concurrently within their phase
**Independent Stories**: 4 user stories can be developed in parallel after Foundational phase
**MVP Scope**: 52 tasks (Setup + Foundational + US1)
