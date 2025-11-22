<!-- T029: Root component with basic layout -->
<script setup lang="ts">
import Dashboard from './views/Dashboard.vue'
</script>

<template>
  <div id="app" class="min-h-screen bg-base-200">
    <header class="navbar bg-base-100 shadow-lg">
      <div class="flex-1">
        <h1 class="text-xl font-bold px-4">Team Daily Goal Tracker</h1>
      </div>
    </header>

    <main class="container mx-auto px-4 py-8">
      <Dashboard />
    </main>
  </div>
</template>

<style scoped>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>
