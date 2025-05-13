import axios from 'axios';

const instance = axios.create({
  baseURL: 'http://library.api:5100/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

export default instance;