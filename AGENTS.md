# Repository Guidelines

## Project Structure & Module Organization
- Backend lives in `backend/src`: `TeamGoalTracker.Api` (controllers, services, data, middleware) and `TeamGoalTracker.Core` (entities/DTOs). Tests are scaffolded in `backend/tests` with `TeamGoalTracker.Api.Tests` and `TeamGoalTracker.Integration`.
- Frontend lives in `frontend/src`: `components`, `composables`, `services`, `types`, `views`, plus `App.vue`/`main.ts`. Tailwind/DaisyUI styling is configured via Vite.
- Docker assets are in `docker` (`docker-compose.yml`, `init-db.sql`). Specifications live under `specs/001-team-goal-tracker`.

## Build, Test, and Development Commands
- Start database: `cd docker && docker-compose up -d` (SQL Server 2022 on 1433 with seeded data).
- Backend: `cd backend/src/TeamGoalTracker.Api && dotnet restore && dotnet run` (dev server on 5000). Build for release with `dotnet publish -c Release`.
- Frontend: `cd frontend && npm install && npm run dev` (Vite dev server on 5173). Production build: `npm run build`; type-check only: `npm run type-check`.
- Tests: placeholder suites under `backend/tests`; run `dotnet test` from `backend` once projects are added. Frontend tests are not yet wired; add vitest before using `npm run test`.

## Coding Style & Naming Conventions
- Formatting: `.editorconfig` enforces LF, UTF-8, trimmed whitespace, final newlines, 2-space indent generally, and 4-space indent for `.cs`.
- C#: use PascalCase for classes/methods, camelCase for locals/fields, DI registration in `Program.cs`. Prefer async/await and nullability annotations (`<Nullable>enable`).
- TypeScript/Vue: Composition API with strict mode (`strict: true` in `tsconfig.json`). Name Vue files in PascalCase, composables as `useX`, services as `*.service.ts`, types under `types/`.
- Styling: Tailwind + DaisyUI; keep utility classes readable and extract repeated patterns into components.

## Testing Guidelines
- Target xUnit-style unit/integration tests in `backend/tests` with fixtures for DB access (use seeded container). Favor `WebApplicationFactory` for API integration.
- Add frontend tests with Vitest and Vue Test Utils when components gain logic. Co-locate specs under `src/**/*.spec.ts`.
- Aim for meaningful coverage on controllers/services and non-trivial Vue composables; include regression cases for bug fixes.

## Commit & Pull Request Guidelines
- Use short, imperative commit messages (e.g., `Add dashboard API`, `Fix mood mapping`). Current history is sparse—set the tone with clear messages and logical commits.
- Pull requests should describe intent, list key changes, and call out tests run. Link issues/tickets when available. Add screenshots/GIFs for UI changes and note API or DB schema impacts.

## Security & Configuration Tips
- Keep connection strings in environment-specific `appsettings.*.json`; do not hardcode secrets. Ensure Docker SQL Server is running before backend start.
- If NuGet restore fails, remove HTTP sources and add `https://api.nuget.org/v3/index.json` as noted in `README.md`. Verify CORS/proxy settings when connecting frontend to the API.
