// T045: useTeamStats composable
import { computed, type Ref } from 'vue'
import type { Goal, Mood, TeamStats, MoodType } from '../types/models'
import { calculateCompletionPercentage, calculateMoodDistribution } from '../types/models'

export function useTeamStats(goals: Ref<Goal[]>, moods: Ref<(Mood | null)[]>) {
  const stats = computed<TeamStats>(() => {
    const totalGoals = goals.value.length
    const completedGoals = goals.value.filter(g => g.isCompleted).length
    const completionPercentage = calculateCompletionPercentage(goals.value)
    const moodDistribution = calculateMoodDistribution(moods.value)

    return {
      completionPercentage,
      moodDistribution,
      totalGoals,
      completedGoals,
    }
  })

  return {
    stats,
  }
}
