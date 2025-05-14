import axios from 'axios';
import refreshToken from '../utils/refreshToken'; // Adjust path if needed

const instance = axios.create({
  baseURL: 'http://library.api:8080/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor: attach access token
instance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor: handle 401 and refresh token
instance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Handle 401: Token expired, try refreshing
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      localStorage.getItem('refreshToken')
    ) {
      originalRequest._retry = true;

      try {
        await refreshToken();  // Refresh the token
        const newAccessToken = localStorage.getItem('accessToken');
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return instance(originalRequest); // Retry original request
      } catch (refreshError) {
        console.error('Token refresh failed:', refreshError);
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
      }
    }

    return Promise.reject(error);
  }
);

export default instance;
