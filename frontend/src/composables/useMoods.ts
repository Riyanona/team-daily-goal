// T081: useMoods composable with updateMood method
import { ref } from 'vue'
import api from '../services/api'
import type { UpdateMoodRequest, Mood } from '../types/models'

export function useMoods() {
  const loading = ref(false)
  const error = ref<string | null>(null)

  const updateMood = async (request: UpdateMoodRequest): Promise<Mood | null> => {
    loading.value = true
    error.value = null
    try {
      const response = await api.put<Mood>('/moods', request)
      return response.data
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update mood'
      console.error('Failed to update mood:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    error,
    updateMood,
  }
}
