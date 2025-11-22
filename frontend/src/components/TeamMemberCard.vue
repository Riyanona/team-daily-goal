<!-- T047, T096-T098: TeamMemberCard component with interactive goal completion -->
<script setup lang="ts">
import type { TeamMember } from '../types/models'
import { MOOD_EMOJIS } from '../types/models'
import { useGoals } from '../composables/useGoals'

const props = defineProps<{
  member: TeamMember
}>()

const emit = defineEmits<{
  goalCompleted: []
}>()

const { completeGoal } = useGoals()

const handleToggleGoal = async (goalId: number, isCompleted: boolean) => {
  // Only allow completing goals, not un-completing
  if (!isCompleted) {
    const result = await completeGoal(goalId)
    if (result) {
      emit('goalCompleted')
    }
  }
}
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <!-- Member header with name and mood -->
      <div class="flex items-center justify-between mb-4">
        <h2 class="card-title">{{ member.name }}</h2>
        <div v-if="member.currentMood" class="text-4xl" :title="`Mood: ${member.currentMood.moodType}`">
          {{ MOOD_EMOJIS[member.currentMood.moodType] }}
        </div>
        <div v-else class="text-2xl text-base-content/30">😶</div>
      </div>

      <!-- Goal completion count -->
      <div class="flex items-center gap-2 mb-3">
        <div class="badge badge-primary badge-lg">
          {{ member.goals.filter(g => g.isCompleted).length }}/{{ member.goals.length }}
        </div>
        <span class="text-sm text-base-content/70">goals completed</span>
      </div>

      <!-- Goals list -->
      <div class="space-y-2">
        <div v-if="member.goals.length === 0" class="text-sm text-base-content/50 italic">
          No goals for today
        </div>
        <div
          v-for="goal in member.goals"
          :key="goal.id"
          class="flex items-start gap-2 p-2 rounded hover:bg-base-200 transition-colors"
        >
          <input
            type="checkbox"
            :checked="goal.isCompleted"
            @change="handleToggleGoal(goal.id, goal.isCompleted)"
            class="checkbox checkbox-sm mt-1"
            :class="{ 'checkbox-primary': goal.isCompleted }"
          />
          <span
            :class="[
              'text-sm flex-1',
              goal.isCompleted ? 'line-through text-base-content/50' : ''
            ]"
          >
            {{ goal.description }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
