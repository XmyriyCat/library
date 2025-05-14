import axios from 'axios';
import refreshToken from '../utils/refreshToken'; // adjust the path if necessary

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

// Response interceptor: refresh token on 401
instance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      localStorage.getItem('refreshToken')
    ) {
      originalRequest._retry = true;

      try {
        await refreshToken();
        const newAccessToken = localStorage.getItem('accessToken');
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return instance(originalRequest); // retry request with new token
      } catch (refreshError) {
        console.error('Token refresh failed:', refreshError);
        // Optional: redirect to login or clear storage
      }
    }

    return Promise.reject(error);
  }
);

export default instance;
