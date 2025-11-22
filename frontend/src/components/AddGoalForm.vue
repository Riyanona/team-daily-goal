<!-- T065-T067: AddGoalForm component with validation, loading states, and error messages -->
<script setup lang="ts">
import { ref, computed } from 'vue'
import type { TeamMember, CreateGoalRequest } from '../types/models'
import { validateGoalRequest } from '../types/models'
import { useGoals } from '../composables/useGoals'

const props = defineProps<{
  teamMembers: TeamMember[]
}>()

const emit = defineEmits<{
  goalCreated: []
}>()

const { loading, error: apiError, createGoal } = useGoals()

const selectedMemberId = ref<number>(0)
const description = ref('')
const validationErrors = ref<string[]>([])

const isFormValid = computed(() => {
  return selectedMemberId.value > 0 && description.value.trim().length > 0
})

const handleSubmit = async () => {
  validationErrors.value = []

  const request: CreateGoalRequest = {
    teamMemberId: selectedMemberId.value,
    description: description.value.trim(),
  }

  // Client-side validation
  const errors = validateGoalRequest(request)
  if (errors.length > 0) {
    validationErrors.value = errors
    return
  }

  const goal = await createGoal(request)
  if (goal) {
    // Reset form
    selectedMemberId.value = 0
    description.value = ''
    emit('goalCreated')
  }
}
</script>

<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <h2 class="card-title">Add New Goal</h2>

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

        <!-- Goal description input -->
        <div class="form-control">
          <label class="label">
            <span class="label-text">Goal Description</span>
            <span class="label-text-alt">{{ description.length }}/500</span>
          </label>
          <textarea
            v-model="description"
            class="textarea textarea-bordered h-24"
            placeholder="Enter goal description..."
            maxlength="500"
            :disabled="loading"
            required
          ></textarea>
        </div>

        <!-- Submit button -->
        <div class="card-actions justify-end">
          <button
            type="submit"
            class="btn btn-primary"
            :disabled="!isFormValid || loading"
          >
            <span v-if="loading" class="loading loading-spinner loading-sm"></span>
            <span v-else>Add Goal</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
