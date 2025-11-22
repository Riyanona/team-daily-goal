// T046: useDashboard composable with API integration
import { ref } from 'vue'
import api from '../services/api'
import type { DashboardData } from '../types/models'

export function useDashboard() {
  const dashboard = ref<DashboardData | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const loadDashboard = async (date?: string) => {
    loading.value = true
    error.value = null
    try {
      const params = date ? { date } : {}
      const response = await api.get<DashboardData>('/dashboard', { params })
      dashboard.value = response.data
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load dashboard'
      console.error('Failed to load dashboard:', err)
    } finally {
      loading.value = false
    }
  }

  return {
    dashboard,
    loading,
    error,
    loadDashboard,
  }
}
