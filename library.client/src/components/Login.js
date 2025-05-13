import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';  // Updated import
import { ApiEndpoints } from '../api/ApiEndpoints';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();  // Use useNavigate

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(ApiEndpoints.Auth.Login, {
        email,
        password,
      });
      const { accessToken, refreshToken } = response.data;
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', refreshToken);

      // Redirect to dashboard after successful registration
      navigate('/dashboard');
    } catch (err) {
      if (err.response?.data?.errors) {
        const messages = Object.entries(err.response.data.errors)
          .map(([key, value]) => `${key}: ${value.join(', ')}`)
          .join('\n');
        setError(messages);
      } else if (err.response?.data?.message) {
        setError('Registration failed: ' + err.response.data.message);
      } else if (err.response.data) {
        setError('Registration failed: ' + err.response.data);
      } else {
        setError('Registration failed: An unknown error occurred.');
      }
    }
  };

  return (
    <div>
      <h2>Login</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <form onSubmit={handleSubmit}>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <button type="submit">Login</button>
      </form>
    </div>
  );
};

export default Login;