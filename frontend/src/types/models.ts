// T019-T020: TypeScript models and mood emoji mappings

export interface TeamMember {
  id: number;
  name: string;
  createdAt: string;
  goals: Goal[];
  currentMood: Mood | null;
}

export interface Goal {
  id: number;
  teamMemberId: number;
  description: string;
  isCompleted: boolean;
  date: string;
  createdAt: string;
}

export interface Mood {
  id: number;
  teamMemberId: number;
  moodType: MoodType;
  date: string;
  updatedAt: string;
}

export enum MoodType {
  VeryHappy = 1,
  Happy = 2,
  Neutral = 3,
  Sad = 4,
  Stressed = 5,
}

export interface TeamStats {
  completionPercentage: number;
  moodDistribution: Record<MoodType, number>;
  totalGoals: number;
  completedGoals: number;
}

export interface DashboardData {
  teamMembers: TeamMember[];
  stats: TeamStats;
  date: string;
}

// Request/Response types
export interface CreateGoalRequest {
  teamMemberId: number;
  description: string;
  date?: string;
}

export interface UpdateMoodRequest {
  teamMemberId: number;
  moodType: MoodType;
}

// Mood emoji mappings
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

// Validation functions
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

// Stats calculation utilities
export function calculateCompletionPercentage(goals: Goal[]): number {
  if (goals.length === 0) return 0;
  const completedCount = goals.filter(g => g.isCompleted).length;
  return Math.round((completedCount / goals.length) * 100);
}

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
