<!-- T082-T083: UpdateMoodForm component with team member dropdown, mood emoji selector, loading states, and error messages -->
<script setup lang="ts">
import { ref, computed } from 'vue'
import type { TeamMember, UpdateMoodRequest, MoodType } from '../types/models'
import { validateMoodRequest, MOOD_EMOJIS, MOOD_LABELS } from '../types/models'
import { useMoods } from '../composables/useMoods'

const props = defineProps<{
  teamMembers: TeamMember[]
}>()

const emit = defineEmits<{
  moodUpdated: []
}>()

const { loading, error: apiError, updateMood } = useMoods()

const selectedMemberId = ref<number>(0)
const selectedMoodType = ref<MoodType | null>(null)
const validationErrors = ref<string[]>([])

const moodOptions: Array<{ type: MoodType; emoji: string; label: string }> = [
  { type: 1, emoji: MOOD_EMOJIS[1], label: MOOD_LABELS[1] },
  { type: 2, emoji: MOOD_EMOJIS[2], label: MOOD_LABELS[2] },
  { type: 3, emoji: MOOD_EMOJIS[3], label: MOOD_LABELS[3] },
  { type: 4, emoji: MOOD_EMOJIS[4], label: MOOD_LABELS[4] },
  { type: 5, emoji: MOOD_EMOJIS[5], label: MOOD_LABELS[5] },
]

const isFormValid = computed(() => {
  return selectedMemberId.value > 0 && selectedMoodType.value !== null
})

const handleSubmit = async () => {
  validationErrors.value = []

  if (!selectedMoodType.value) {
    validationErrors.value.push('Please select a mood')
    return
  }

  const request: UpdateMoodRequest = {
    teamMemberId: selectedMemberId.value,
    moodType: selectedMoodType.value,
  }

  // Client-side validation
  const errors = validateMoodRequest(request)
  if (errors.length > 0) {
    validationErrors.value = errors
    return
  }

  const mood = await updateMood(request)
  if (mood) {
    // Reset form
    selectedMemberId.value = 0
    selectedMoodType.value = null
    emit('moodUpdated')
  }
}
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title">Update Team Member Mood</h2>

      <!-- Validation errors -->
      <div v-if="validationErrors.length > 0" class="alert alert-warning shadow-sm mb-4">
        <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
        <ul class="list-disc list-inside">
          <li v-for="(error, index) in validationErrors" :key="index">{{ error }}</li>
        </ul>
      </div>

      <!-- API error -->
      <div v-if="apiError" class="alert alert-error shadow-sm mb-4">
        <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span>{{ apiError }}</span>
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <!-- Team member dropdown -->
        <div class="form-control">
          <label class="label">
            <span class="label-text">Team Member</span>
          </label>
          <select
            v-model.number="selectedMemberId"
            class="select select-bordered w-full"
            :disabled="loading"
            required
          >
            <option :value="0" disabled>Select a team member</option>
            <option v-for="member in teamMembers" :key="member.id" :value="member.id">
              {{ member.name }}
            </option>
          </select>
        </div>

        <!-- Mood emoji selector -->
        <div class="form-control">
          <label class="label">
            <span class="label-text">Mood</span>
          </label>
          <div class="grid grid-cols-5 gap-2">
            <button
              v-for="mood in moodOptions"
              :key="mood.type"
              type="button"
              @click="selectedMoodType = mood.type"
              :class="[
                'btn btn-lg flex flex-col items-center justify-center p-4 h-auto',
                selectedMoodType === mood.type ? 'btn-primary' : 'btn-outline'
              ]"
              :disabled="loading"
              :title="mood.label"
            >
              <span class="text-3xl">{{ mood.emoji }}</span>
              <span class="text-xs mt-1">{{ mood.label }}</span>
            </button>
          </div>
        </div>

        <!-- Submit button -->
        <div class="card-actions justify-end">
          <button
            type="submit"
            class="btn btn-primary"
            :disabled="!isFormValid || loading"
          >
            <span v-if="loading" class="loading loading-spinner loading-sm"></span>
            <span v-else>Update Mood</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
