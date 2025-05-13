import axios from 'axios';
import { ApiEndpoints } from '../api/ApiEndpoints';

const refreshToken = async () => {
  const refreshToken = localStorage.getItem('refreshToken');

  try {
    const response = await axios.post(ApiEndpoints.Auth.Refresh, {
      refreshToken,
    });
    const { accessToken } = response.data;
    localStorage.setItem('accessToken', accessToken);
  } catch (error) {
    console.log('Refresh token failed', error);
  }
};

export default refreshToken;