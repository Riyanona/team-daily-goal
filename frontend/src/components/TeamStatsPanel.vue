<!-- T048: TeamStatsPanel component -->
<script setup lang="ts">
import type { TeamStats, MoodType } from '../types/models'
import { MOOD_EMOJIS, MOOD_LABELS } from '../types/models'

defineProps<{
  stats: TeamStats
}>()

const moodTypes: MoodType[] = [1, 2, 3, 4, 5] // VeryHappy, Happy, Neutral, Sad, Stressed
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title mb-4">Team Statistics</h2>

      <!-- Goal Completion -->
      <div class="stat mb-4">
        <div class="stat-title">Goal Completion</div>
        <div class="stat-value text-primary">{{ Math.round(stats.completionPercentage) }}%</div>
        <div class="stat-desc">{{ stats.completedGoals }} of {{ stats.totalGoals }} goals completed</div>
      </div>

      <div class="divider"></div>

      <!-- Mood Distribution -->
      <div class="stat">
        <div class="stat-title mb-3">Team Mood</div>
        <div class="space-y-2">
          <div
            v-for="moodType in moodTypes"
            :key="moodType"
            class="flex items-center justify-between p-2 rounded bg-base-200"
          >
            <div class="flex items-center gap-2">
              <span class="text-2xl">{{ MOOD_EMOJIS[moodType] }}</span>
              <span class="text-sm">{{ MOOD_LABELS[moodType] }}</span>
            </div>
            <div class="badge badge-lg">
              {{ stats.moodDistribution[moodType] || 0 }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
