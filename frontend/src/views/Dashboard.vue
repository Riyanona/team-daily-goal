<!-- T049-T051, T068-T069, T084-T086: Dashboard view with team member cards grid, stats panel, AddGoalForm, and UpdateMoodForm integration -->
<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useDashboard } from '../composables/useDashboard'
import TeamMemberCard from '../components/TeamMemberCard.vue'
import TeamStatsPanel from '../components/TeamStatsPanel.vue'
import AddGoalForm from '../components/AddGoalForm.vue'
import UpdateMoodForm from '../components/UpdateMoodForm.vue'

const { dashboard, loading, error, loadDashboard } = useDashboard()

// Computed properties for easier access
const teamMembers = computed(() => dashboard.value?.teamMembers || [])
const stats = computed(() => dashboard.value?.stats || {
  completionPercentage: 0,
  moodDistribution: { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 },
  totalGoals: 0,
  completedGoals: 0,
})

const handleGoalCreated = () => {
  // Refresh dashboard to show new goal
  loadDashboard()
}

const handleMoodUpdated = () => {
  // Refresh dashboard to show new mood and stats
  loadDashboard()
}

// T099-T100: Handle goal completion and refresh dashboard
const handleGoalCompleted = () => {
  // Refresh dashboard to show updated completion status and stats
  loadDashboard()
}

onMounted(() => {
  loadDashboard()
})
</script>

<template>
  <div class="dashboard">
    <!-- Loading state -->
    <div v-if="loading" class="flex justify-center items-center min-h-[400px]">
      <span class="loading loading-spinner loading-lg"></span>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="alert alert-error shadow-lg">
      <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
      <span>{{ error }}</span>
      <button class="btn btn-sm" @click="loadDashboard()">Retry</button>
    </div>

    <!-- Dashboard content -->
    <div v-else-if="dashboard" class="space-y-6">
      <!-- Date display -->
      <div class="text-sm text-base-content/70">
        Showing data for: {{ new Date(dashboard.date).toLocaleDateString() }}
      </div>

      <!-- Forms Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Add Goal Form -->
        <AddGoalForm :team-members="teamMembers" @goal-created="handleGoalCreated" />

        <!-- Update Mood Form -->
        <UpdateMoodForm :team-members="teamMembers" @mood-updated="handleMoodUpdated" />
      </div>

      <!-- Stats Panel (top on mobile, side on desktop) -->
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
        <!-- Team Members Grid (3 columns) -->
        <div class="lg:col-span-3 grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
          <TeamMemberCard
            v-for="member in teamMembers"
            :key="member.id"
            :member="member"
            @goal-completed="handleGoalCompleted"
          />

          <!-- Empty state -->
          <div v-if="teamMembers.length === 0" class="col-span-full text-center py-12">
            <p class="text-lg text-base-content/50">No team members found</p>
          </div>
        </div>

        <!-- Stats Panel (1 column) -->
        <div class="lg:col-span-1">
          <TeamStatsPanel :stats="stats" />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard {
  width: 100%;
  max-width: 1400px;
  margin: 0 auto;
}
</style>
