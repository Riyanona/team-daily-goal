// T064: useGoals composable with createGoal method
import { ref } from 'vue'
import api from '../services/api'
import type { CreateGoalRequest, Goal } from '../types/models'

export function useGoals() {
  const loading = ref(false)
  const error = ref<string | null>(null)

  const createGoal = async (request: CreateGoalRequest): Promise<Goal | null> => {
    loading.value = true
    error.value = null
    try {
      const response = await api.post<Goal>('/goals', request)
      return response.data
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create goal'
      console.error('Failed to create goal:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // T095: completeGoal method
  const completeGoal = async (goalId: number): Promise<Goal | null> => {
    loading.value = true
    error.value = null
    try {
      const response = await api.patch<Goal>(`/goals/${goalId}/complete`)
      return response.data
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to complete goal'
      console.error('Failed to complete goal:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    error,
    createGoal,
    completeGoal,
  }
}
