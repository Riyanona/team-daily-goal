# Data Model: Team Daily Goal Tracker

**Date**: 2025-11-20
**Feature**: Team Daily Goal Tracker
**Purpose**: Define database schema, domain entities, and data relationships

## Entity Relationship Diagram

```
┌─────────────────┐
│   TeamMember    │
├─────────────────┤
│ Id (PK)         │
│ Name            │
│ CreatedAt       │
└────────┬────────┘
         │
         │ 1
         │
         │
         │ *
    ┌────┴────┐
    │         │
┌───▼──────┐  │
│   Goal   │  │
├──────────┤  │
│ Id (PK)  │  │
│ TeamMember│ │
│  Id (FK) │  │
│ Descrip- │  │
│  tion    │  │
│ IsComple-│  │
│  ted     │  │
│ Date     │  │
│ CreatedAt│  │
└──────────┘  │
              │
         ┌────▼──────┐
         │   Mood    │
         ├───────────┤
         │ Id (PK)   │
         │ TeamMember│
         │  Id (FK)  │
         │ MoodType  │
         │ Date      │
         │ UpdatedAt │
         └───────────┘
```

**Relationships**:
- TeamMember → Goal: One-to-Many (one team member has many goals)
- TeamMember → Mood: One-to-Many (one team member has many mood entries, but typically one per day)

## Database Schema (SQL Server)

### TeamMembers Table

Represents individual team members who have goals and moods tracked.

```sql
CREATE TABLE TeamMembers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT CK_TeamMembers_Name_NotEmpty CHECK (LEN(TRIM(Name)) > 0)
);

CREATE INDEX IX_TeamMembers_Name ON TeamMembers(Name);
```

**Columns**:
- `Id`: Primary key, auto-increment
- `Name`: Team member's display name (required, max 100 characters)
- `CreatedAt`: Timestamp of when member was added

**Constraints**:
- Name cannot be empty or whitespace
- Name must be unique (enforced at application layer, not DB constraint, to allow for future flexibility)

**Validation Rules** (from FR-014):
- Name required when creating team member
- Name length: 1-100 characters

### Goals Table

Represents daily goals assigned to team members.

```sql
CREATE TABLE Goals (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamMemberId INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    Date DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Goals_TeamMember FOREIGN KEY (TeamMemberId)
        REFERENCES TeamMembers(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Goals_Description_NotEmpty CHECK (LEN(TRIM(Description)) > 0)
);

-- Composite index for efficient date-filtered queries
CREATE INDEX IX_Goals_TeamMemberId_Date ON Goals(TeamMemberId, Date);

-- Index for date filtering (used by stats calculation)
CREATE INDEX IX_Goals_Date_IsCompleted ON Goals(Date, IsCompleted);
```

**Columns**:
- `Id`: Primary key, auto-increment
- `TeamMemberId`: Foreign key to TeamMembers (required)
- `Description`: Goal text (required, max 500 characters)
- `IsCompleted`: Boolean flag for completion status (default: false)
- `Date`: Date the goal belongs to (default: today's date)
- `CreatedAt`: Timestamp of when goal was created

**Constraints**:
- TeamMemberId must reference existing TeamMember
- Description cannot be empty or whitespace
- Cascading delete: When TeamMember deleted, all their goals deleted

**Validation Rules** (from FR-013, FR-014):
- TeamMemberId required
- Description required (non-empty)
- Description length: 1-500 characters
- Date defaults to current date
- IsCompleted defaults to false

**State Transitions**:
- **Created**: `IsCompleted = false` (initial state)
- **Completed**: `IsCompleted = true` (terminal state for current scope)
- **Note**: No "uncomplete" transition in MVP, no editing, no deletion

### Moods Table

Represents team members' mood states, typically one entry per day per member.

```sql
CREATE TABLE Moods (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TeamMemberId INT NOT NULL,
    MoodType INT NOT NULL,
    Date DATE NOT NULL DEFAULT CAST(GETUTCDATE() AS DATE),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Moods_TeamMember FOREIGN KEY (TeamMemberId)
        REFERENCES TeamMembers(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Moods_MoodType_Range CHECK (MoodType BETWEEN 1 AND 5),
    CONSTRAINT UQ_Moods_TeamMember_Date UNIQUE (TeamMemberId, Date)
);

-- Composite index for efficient date-filtered queries
CREATE INDEX IX_Moods_TeamMemberId_Date ON Moods(TeamMemberId, Date);

-- Index for date filtering (used by stats calculation)
CREATE INDEX IX_Moods_Date ON Moods(Date);
```

**Columns**:
- `Id`: Primary key, auto-increment
- `TeamMemberId`: Foreign key to TeamMembers (required)
- `MoodType`: Integer enum representing mood (1-5, required)
- `Date`: Date the mood entry belongs to (default: today's date)
- `UpdatedAt`: Timestamp of when mood was last updated

**MoodType Enum**:
```
1 = VeryHappy  (😀)
2 = Happy      (😊)
3 = Neutral    (😐)
4 = Sad        (😞)
5 = Stressed   (😤)
```

**Constraints**:
- TeamMemberId must reference existing TeamMember
- MoodType must be between 1-5 (inclusive)
- Unique constraint: One mood entry per team member per day
- Cascading delete: When TeamMember deleted, all their moods deleted

**Validation Rules** (from FR-005, FR-014):
- TeamMemberId required
- MoodType required (must be 1-5)
- Date defaults to current date
- UpdatedAt automatically updated on mood change

**State Transitions**:
- **No Mood Set**: No row exists for TeamMember + Date
- **Mood Set**: Row exists with MoodType value
- **Mood Updated**: UPDATE existing row (UpdatedAt timestamp changes)

## Domain Entities (C# Backend)

### TeamMember Entity

```csharp
namespace TeamGoalTracker.Core.Entities;

public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation properties (not stored in DB, populated by Dapper multi-mapping)
    public List<Goal> Goals { get; set; } = new();
    public Mood? CurrentMood { get; set; }
}
```

### Goal Entity

```csharp
namespace TeamGoalTracker.Core.Entities;

public class Goal
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Mood Entity

```csharp
namespace TeamGoalTracker.Core.Entities;

public class Mood
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    public MoodType MoodType { get; set; }
    public DateTime Date { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum MoodType
{
    VeryHappy = 1,
    Happy = 2,
    Neutral = 3,
    Sad = 4,
    Stressed = 5
}
```

## TypeScript Models (Frontend)

### Interface Definitions

```typescript
// src/types/models.ts

export interface TeamMember {
  id: number;
  name: string;
  createdAt: string; // ISO 8601 date string
  goals: Goal[];
  currentMood: Mood | null;
}

export interface Goal {
  id: number;
  teamMemberId: number;
  description: string;
  isCompleted: boolean;
  date: string; // ISO 8601 date string (YYYY-MM-DD)
  createdAt: string; // ISO 8601 datetime string
}

export interface Mood {
  id: number;
  teamMemberId: number;
  moodType: MoodType;
  date: string; // ISO 8601 date string (YYYY-MM-DD)
  updatedAt: string; // ISO 8601 datetime string
}

export enum MoodType {
  VeryHappy = 1,
  Happy = 2,
  Neutral = 3,
  Sad = 4,
  Stressed = 5,
}

export interface TeamStats {
  completionPercentage: number; // 0-100
  moodDistribution: Record<MoodType, number>; // Count of each mood type
  totalGoals: number;
  completedGoals: number;
}

export interface DashboardData {
  teamMembers: TeamMember[];
  stats: TeamStats;
  date: string; // Date being viewed (ISO 8601 YYYY-MM-DD)
}
```

### Mood Emoji Mapping

```typescript
// src/types/models.ts

export const MOOD_EMOJIS: Record<MoodType, string> = {
  [MoodType.VeryHappy]: '😀',
  [MoodType.Happy]: '😊',
  [MoodType.Neutral]: '😐',
  [MoodType.Sad]: '😞',
  [MoodType.Stressed]: '😤',
};

export const MOOD_LABELS: Record<MoodType, string> = {
  [MoodType.VeryHappy]: 'Very Happy',
  [MoodType.Happy]: 'Happy',
  [MoodType.Neutral]: 'Neutral',
  [MoodType.Sad]: 'Sad',
  [MoodType.Stressed]: 'Stressed',
};
```

## Data Validation Rules

### Goal Validation

**Frontend** (TypeScript):
```typescript
export interface CreateGoalRequest {
  teamMemberId: number;
  description: string;
}

export function validateGoalRequest(request: CreateGoalRequest): string[] {
  const errors: string[] = [];

  if (!request.teamMemberId || request.teamMemberId <= 0) {
    errors.push('Team member must be selected');
  }

  if (!request.description || request.description.trim().length === 0) {
    errors.push('Goal description cannot be empty');
  }

  if (request.description && request.description.length > 500) {
    errors.push('Goal description cannot exceed 500 characters');
  }

  return errors;
}
```

**Backend** (C#):
```csharp
public record CreateGoalRequest(int TeamMemberId, string Description);

public class CreateGoalRequestValidator : AbstractValidator<CreateGoalRequest>
{
    public CreateGoalRequestValidator()
    {
        RuleFor(x => x.TeamMemberId)
            .GreaterThan(0)
            .WithMessage("Team member must be selected");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Goal description cannot be empty")
            .MaximumLength(500)
            .WithMessage("Goal description cannot exceed 500 characters");
    }
}
```

### Mood Validation

**Frontend** (TypeScript):
```typescript
export interface UpdateMoodRequest {
  teamMemberId: number;
  moodType: MoodType;
}

export function validateMoodRequest(request: UpdateMoodRequest): string[] {
  const errors: string[] = [];

  if (!request.teamMemberId || request.teamMemberId <= 0) {
    errors.push('Team member must be selected');
  }

  if (!Object.values(MoodType).includes(request.moodType)) {
    errors.push('Invalid mood type');
  }

  return errors;
}
```

**Backend** (C#):
```csharp
public record UpdateMoodRequest(int TeamMemberId, MoodType MoodType);

public class UpdateMoodRequestValidator : AbstractValidator<UpdateMoodRequest>
{
    public UpdateMoodRequestValidator()
    {
        RuleFor(x => x.TeamMemberId)
            .GreaterThan(0)
            .WithMessage("Team member must be selected");

        RuleFor(x => x.MoodType)
            .IsInEnum()
            .WithMessage("Invalid mood type");
    }
}
```

## Business Logic Rules

### Goal Completion Calculation

**Formula**: `(Number of Completed Goals / Total Goals) * 100`

**Edge Cases**:
- If total goals = 0: Return 0% (or "N/A" in UI)
- Round to nearest integer (e.g., 66.67% → 67%)

**Implementation** (TypeScript):
```typescript
export function calculateCompletionPercentage(goals: Goal[]): number {
  if (goals.length === 0) return 0;
  const completedCount = goals.filter(g => g.isCompleted).length;
  return Math.round((completedCount / goals.length) * 100);
}
```

### Mood Distribution Calculation

**Output**: Count of team members in each mood category for the current date

**Edge Cases**:
- Team member with no mood set: Not included in any category count
- Empty team: All mood counts = 0

**Implementation** (TypeScript):
```typescript
export function calculateMoodDistribution(moods: (Mood | null)[]): Record<MoodType, number> {
  const distribution: Record<MoodType, number> = {
    [MoodType.VeryHappy]: 0,
    [MoodType.Happy]: 0,
    [MoodType.Neutral]: 0,
    [MoodType.Sad]: 0,
    [MoodType.Stressed]: 0,
  };

  moods.forEach(mood => {
    if (mood) {
      distribution[mood.moodType]++;
    }
  });

  return distribution;
}
```

## Data Seeding (Development)

### Sample Team Members

```sql
-- Seed team members for development
INSERT INTO TeamMembers (Name, CreatedAt) VALUES
('Alice Johnson', GETUTCDATE()),
('Bob Smith', GETUTCDATE()),
('Charlie Davis', GETUTCDATE()),
('Diana Prince', GETUTCDATE()),
('Eve Wilson', GETUTCDATE());
```

### Sample Goals

```sql
-- Seed sample goals for today
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
```

### Sample Moods

```sql
-- Seed sample moods for today
DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);

INSERT INTO Moods (TeamMemberId, MoodType, Date) VALUES
(1, 2, @Today),  -- Alice: Happy
(2, 3, @Today),  -- Bob: Neutral
(3, 1, @Today),  -- Charlie: Very Happy
(4, 4, @Today),  -- Diana: Sad
(5, 5, @Today);  -- Eve: Stressed
```

## Data Migration Strategy

### Initial Schema Creation

**File**: `docker/init-db.sql`

1. Create database
2. Create tables (TeamMembers, Goals, Moods)
3. Create indexes
4. Seed initial team members (for development)

### Future Migrations (Out of Scope for MVP)

- Version-controlled SQL scripts (e.g., `migrations/001_initial.sql`)
- Migration tracking table (applied migrations)
- Rollback scripts for each migration

**Note**: No EF Core migrations used - simple SQL scripts are sufficient for this schema.

## Performance Considerations

### Index Strategy

**Goals Table**:
- `IX_Goals_TeamMemberId_Date`: Covers queries filtering by team member and date (dashboard load)
- `IX_Goals_Date_IsCompleted`: Covers stats calculation (count completed/total for date)

**Moods Table**:
- `IX_Moods_TeamMemberId_Date`: Covers queries finding mood for team member on date
- `IX_Moods_Date`: Covers stats calculation (mood distribution for date)

### Query Optimization

**Dashboard Load** (single query):
```sql
-- Fetch all data in one query to avoid N+1
SELECT
    tm.Id AS TeamMemberId, tm.Name, tm.CreatedAt,
    g.Id AS GoalId, g.Description, g.IsCompleted, g.Date AS GoalDate, g.CreatedAt AS GoalCreatedAt,
    m.Id AS MoodId, m.MoodType, m.UpdatedAt
FROM TeamMembers tm
LEFT JOIN Goals g ON tm.Id = g.TeamMemberId AND g.Date = @Today
LEFT JOIN Moods m ON tm.Id = m.TeamMemberId AND m.Date = @Today
ORDER BY tm.Name, g.CreatedAt;
```

**Dapper Multi-Mapping**:
Use `QueryAsync` with multiple types to map flat result set into nested objects.

## Summary

Data model defined with 3 tables (TeamMembers, Goals, Moods), clear relationships, and comprehensive validation rules. Database schema includes appropriate indexes for performance. Domain entities defined for both backend (C#) and frontend (TypeScript) with matching structures. Business logic rules documented for goal completion and mood distribution calculations. Sample seed data provided for development and testing.
