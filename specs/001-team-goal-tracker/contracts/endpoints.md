# API Endpoints: Team Daily Goal Tracker

**Base URL**: `http://localhost:5000/api`
**Content-Type**: `application/json`
**Date Format**: ISO 8601 (dates: `YYYY-MM-DD`, datetimes: `YYYY-MM-DDTHH:mm:ssZ`)

## Endpoint Overview

| Method | Endpoint | Purpose | Request Body | Response |
|--------|----------|---------|--------------|----------|
| GET | `/dashboard` | Get complete dashboard data | - | `DashboardResponse` |
| GET | `/team-members` | Get all team members | - | `TeamMemberDto[]` |
| POST | `/goals` | Create a new goal | `CreateGoalRequest` | `GoalDto` |
| PATCH | `/goals/{id}/complete` | Mark goal complete | - | `GoalDto` |
| PUT | `/moods` | Update mood (upsert) | `UpdateMoodRequest` | `MoodDto` |
| GET | `/stats` | Get team statistics | - | `TeamStatsDto` |

## Endpoint Details

### GET /api/dashboard

**Purpose**: Primary endpoint for dashboard page load. Returns all team members with their goals and moods, plus team stats, in a single request.

**Query Parameters**:
- `date` (optional): Date to retrieve data for (format: `YYYY-MM-DD`, default: today)

**Response** (`200 OK`):
```json
{
  "teamMembers": [
    {
      "id": 1,
      "name": "Alice Johnson",
      "createdAt": "2025-11-20T10:00:00Z",
      "goals": [
        {
          "id": 1,
          "teamMemberId": 1,
          "description": "Complete project proposal",
          "isCompleted": false,
          "date": "2025-11-20",
          "createdAt": "2025-11-20T09:30:00Z"
        },
        {
          "id": 2,
          "teamMemberId": 1,
          "description": "Review pull requests",
          "isCompleted": true,
          "date": "2025-11-20",
          "createdAt": "2025-11-20T08:15:00Z"
        }
      ],
      "currentMood": {
        "id": 1,
        "teamMemberId": 1,
        "moodType": 2,
        "date": "2025-11-20",
        "updatedAt": "2025-11-20T11:45:00Z"
      }
    }
  ],
  "stats": {
    "completionPercentage": 50.0,
    "moodDistribution": {
      "1": 1,
      "2": 3,
      "3": 2,
      "4": 0,
      "5": 1
    },
    "totalGoals": 15,
    "completedGoals": 8
  },
  "date": "2025-11-20"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid date format

**Performance Target**: <500ms p95 (per constitution)

**Frontend Usage**:
```typescript
// Load dashboard on page mount
const { data } = await api.get<DashboardResponse>('/dashboard');
```

---

### GET /api/team-members

**Purpose**: Retrieve list of all team members (basic info only, no goals or moods). Used for dropdown population in forms.

**Response** (`200 OK`):
```json
[
  {
    "id": 1,
    "name": "Alice Johnson",
    "createdAt": "2025-11-20T10:00:00Z"
  },
  {
    "id": 2,
    "name": "Bob Smith",
    "createdAt": "2025-11-20T10:01:00Z"
  }
]
```

**Frontend Usage**:
```typescript
// Populate team member dropdown in AddGoal/UpdateMood forms
const { data: teamMembers } = await api.get<TeamMemberDto[]>('/team-members');
```

---

### POST /api/goals

**Purpose**: Create a new goal for a team member on today's date.

**Request Body** (`CreateGoalRequest`):
```json
{
  "teamMemberId": 1,
  "description": "Complete project proposal"
}
```

**Validation Rules**:
- `teamMemberId`: Required, must be > 0, must exist in TeamMembers table
- `description`: Required, 1-500 characters, cannot be empty/whitespace

**Response** (`201 Created`):
```json
{
  "id": 3,
  "teamMemberId": 1,
  "description": "Complete project proposal",
  "isCompleted": false,
  "date": "2025-11-20",
  "createdAt": "2025-11-20T14:30:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Validation errors
  ```json
  {
    "type": "ValidationError",
    "title": "One or more validation errors occurred",
    "status": 400,
    "errors": {
      "Description": ["Description cannot be empty"]
    }
  }
  ```
- `404 Not Found`: Team member not found

**Performance Target**: <200ms p95

**Frontend Usage**:
```typescript
// Submit AddGoalForm
const newGoal = await api.post<GoalDto>('/goals', {
  teamMemberId: selectedMemberId.value,
  description: goalDescription.value
});
```

---

### PATCH /api/goals/{id}/complete

**Purpose**: Mark a goal as complete (set `IsCompleted = true`). Idempotent - calling multiple times has same effect.

**Path Parameters**:
- `id`: Goal ID (integer)

**Request Body**: None

**Response** (`200 OK`):
```json
{
  "id": 3,
  "teamMemberId": 1,
  "description": "Complete project proposal",
  "isCompleted": true,
  "date": "2025-11-20",
  "createdAt": "2025-11-20T14:30:00Z"
}
```

**Error Responses**:
- `404 Not Found`: Goal not found

**Performance Target**: <200ms p95

**Frontend Usage**:
```typescript
// Checkbox toggle handler
const toggleGoalComplete = async (goalId: number) => {
  await api.patch(`/goals/${goalId}/complete`);
};
```

---

### PUT /api/moods

**Purpose**: Update or create a mood for a team member on today's date (upsert operation). If mood exists for today, update it; otherwise create new.

**Request Body** (`UpdateMoodRequest`):
```json
{
  "teamMemberId": 1,
  "moodType": 2
}
```

**MoodType Values**:
- `1` = Very Happy (😀)
- `2` = Happy (😊)
- `3` = Neutral (😐)
- `4` = Sad (😞)
- `5` = Stressed (😤)

**Validation Rules**:
- `teamMemberId`: Required, must be > 0, must exist in TeamMembers table
- `moodType`: Required, must be 1-5 (inclusive)

**Response** (`200 OK`):
```json
{
  "id": 1,
  "teamMemberId": 1,
  "moodType": 2,
  "date": "2025-11-20",
  "updatedAt": "2025-11-20T15:00:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Validation errors
  ```json
  {
    "type": "ValidationError",
    "title": "One or more validation errors occurred",
    "status": 400,
    "errors": {
      "MoodType": ["Invalid mood type"]
    }
  }
  ```
- `404 Not Found`: Team member not found

**Performance Target**: <200ms p95

**Frontend Usage**:
```typescript
// Submit UpdateMoodForm
const updatedMood = await api.put<MoodDto>('/moods', {
  teamMemberId: selectedMemberId.value,
  moodType: selectedMood.value
});
```

---

### GET /api/stats

**Purpose**: Get aggregated team statistics for a specific date (goal completion percentage and mood distribution).

**Query Parameters**:
- `date` (optional): Date to calculate stats for (format: `YYYY-MM-DD`, default: today)

**Response** (`200 OK`):
```json
{
  "completionPercentage": 53.33,
  "moodDistribution": {
    "1": 1,
    "2": 3,
    "3": 2,
    "4": 0,
    "5": 1
  },
  "totalGoals": 15,
  "completedGoals": 8
}
```

**Calculation Notes**:
- `completionPercentage`: `(completedGoals / totalGoals) * 100`, rounded to 2 decimals
- If `totalGoals = 0`, return `completionPercentage: 0.0`
- `moodDistribution`: Count of team members in each mood category (only members with moods set)

**Error Responses**:
- `400 Bad Request`: Invalid date format

**Performance Target**: <200ms p95

**Frontend Usage**:
```typescript
// Refresh stats after goal/mood update (alternative to full dashboard reload)
const { data: stats } = await api.get<TeamStatsDto>('/stats');
```

---

## Error Response Format

All error responses follow Problem Details (RFC 7807) format:

```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred",
  "status": 400,
  "errors": {
    "FieldName": ["Error message 1", "Error message 2"]
  },
  "traceId": "0HN7GMLQF2K3J:00000001"
}
```

**Common HTTP Status Codes**:
- `200 OK`: Successful GET, PATCH, PUT
- `201 Created`: Successful POST
- `204 No Content`: Successful operation with no response body (not used in this API)
- `400 Bad Request`: Validation error or malformed request
- `404 Not Found`: Resource not found (team member, goal)
- `500 Internal Server Error`: Unexpected server error

**Error Types**:
- `ValidationError`: Request validation failed
- `NotFoundError`: Requested resource doesn't exist
- `ServerError`: Unexpected internal error

---

## CORS Configuration

**Development**: Allow `http://localhost:5173` (Vite dev server)

**Headers**:
```
Access-Control-Allow-Origin: http://localhost:5173
Access-Control-Allow-Methods: GET, POST, PUT, PATCH, DELETE, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization
```

---

## Frontend API Client

### Axios Setup

```typescript
// src/services/api.ts
import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 400) {
      // Validation errors - extract messages
      const errors = error.response.data.errors || {};
      throw new ValidationError(errors);
    } else if (error.response?.status === 404) {
      throw new NotFoundError(error.response.data.title);
    } else {
      throw new Error('An unexpected error occurred');
    }
  }
);

export default api;
```

### Usage in Composables

```typescript
// src/composables/useGoals.ts
import api from '@/services/api';
import type { GoalDto, CreateGoalRequest } from '@/types/models';

export function useGoals() {
  const loading = ref(false);
  const error = ref<string | null>(null);

  const createGoal = async (request: CreateGoalRequest): Promise<GoalDto> => {
    loading.value = true;
    error.value = null;
    try {
      const { data } = await api.post<GoalDto>('/goals', request);
      return data;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create goal';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const completeGoal = async (goalId: number): Promise<GoalDto> => {
    loading.value = true;
    error.value = null;
    try {
      const { data } = await api.patch<GoalDto>(`/goals/${goalId}/complete`);
      return data;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to complete goal';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  return { createGoal, completeGoal, loading, error };
}
```

---

## Testing Contract Compliance

### Backend Contract Tests (xUnit + TestServer)

```csharp
[Fact]
public async Task POST_Goals_WithValidRequest_Returns201()
{
    // Arrange
    var request = new CreateGoalRequest(TeamMemberId: 1, Description: "Test goal");

    // Act
    var response = await _client.PostAsJsonAsync("/api/goals", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    var goal = await response.Content.ReadFromJsonAsync<GoalDto>();
    goal.Should().NotBeNull();
    goal!.Description.Should().Be("Test goal");
    goal.IsCompleted.Should().BeFalse();
}

[Fact]
public async Task POST_Goals_WithEmptyDescription_Returns400()
{
    // Arrange
    var request = new CreateGoalRequest(TeamMemberId: 1, Description: "");

    // Act
    var response = await _client.PostAsJsonAsync("/api/goals", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    error!.Errors.Should().ContainKey("Description");
}
```

### Frontend Contract Tests (Vitest)

```typescript
// tests/unit/api-contracts.spec.ts
describe('API Contract: POST /goals', () => {
  it('should return 201 with GoalDto on successful creation', async () => {
    const mockGoal: GoalDto = {
      id: 1,
      teamMemberId: 1,
      description: 'Test goal',
      isCompleted: false,
      date: '2025-11-20',
      createdAt: '2025-11-20T14:30:00Z',
    };

    mock.onPost('/goals').reply(201, mockGoal);

    const result = await createGoal({ teamMemberId: 1, description: 'Test goal' });
    expect(result).toEqual(mockGoal);
  });

  it('should throw ValidationError on 400 response', async () => {
    const mockError = {
      type: 'ValidationError',
      status: 400,
      errors: { Description: ['Description cannot be empty'] },
    };

    mock.onPost('/goals').reply(400, mockError);

    await expect(createGoal({ teamMemberId: 1, description: '' }))
      .rejects
      .toThrow(ValidationError);
  });
});
```

---

## Summary

6 RESTful endpoints defined covering all functional requirements (FR-001 through FR-015). Single optimized dashboard endpoint reduces initial page load to one request. All endpoints use standard HTTP methods and status codes. Comprehensive error handling with RFC 7807 Problem Details format. CORS configured for local development. Frontend API client provides type-safe requests with centralized error handling. Contract tests ensure backend/frontend alignment.
