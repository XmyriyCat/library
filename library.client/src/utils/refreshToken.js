import axios from 'axios';
import { ApiEndpoints } from '../api/ApiEndpoints';

const refreshToken = async (navigate) => {
  const token = localStorage.getItem('refreshToken');
  if (!token) {
    throw new Error('No refresh token found');
  }

  try {
    const response = await axios.post(ApiEndpoints.Auth.Refresh, JSON.stringify(token), {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    const { accessToken } = response.data;

    if (!accessToken) {
      throw new Error('No access token returned from server');
    }

    localStorage.setItem('accessToken', accessToken);
    return accessToken;
  } catch (error) {
    console.error('Token refresh failed:', error);
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');

    if (navigate) {
      navigate('/login');
    }

    throw error;
  }
};

export default refreshToken;
