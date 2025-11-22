// T026: Axios base client with interceptors
import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5205/api',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 400) {
      // Validation errors - extract messages
      const errors = error.response.data.errors || {};
      const errorMessages = Object.values(errors).flat().join(', ');
      throw new Error(errorMessages || 'Validation error');
    } else if (error.response?.status === 404) {
      throw new Error(error.response.data.title || 'Resource not found');
    } else {
      throw new Error('An unexpected error occurred');
    }
  }
);

export default api;
