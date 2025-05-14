import axios from 'axios';
import { ApiEndpoints } from '../api/ApiEndpoints';

// Pass `navigate` from a React component
const refreshToken = async (navigate) => {
  const refreshToken = localStorage.getItem('refreshToken');

  if (!refreshToken) {
    throw new Error('No refresh token found');
  }

  try {
    const response = await axios.post(ApiEndpoints.Auth.Refresh, { refreshToken });
    const { accessToken } = response.data;

    if (!accessToken) {
      throw new Error('No access token returned from server');
    }

    localStorage.setItem('accessToken', accessToken);
    return accessToken;
  } catch (error) {
    console.error('Refresh token failed:', error);
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    
    if (navigate) {
      navigate('/login');
    }

    throw error;
  }
};

export default refreshToken;
