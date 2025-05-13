import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { ApiEndpoints } from '../api/ApiEndpoints';

const Register = () => {
  const [formData, setFormData] = useState({
    email: '',
    userName: '',
    password: ''
  });

  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const response = await axios.post(ApiEndpoints.Auth.Register, formData);
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
      } else if (err.message) {
        setError('Registration failed: ' + err.message);
      } else {
        setError('Registration failed: An unknown error occurred.');
      }
    }
  };

  return (
    <div>
      <h2>Register</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label>Email:</label><br />
          <input type="email" name="email" value={formData.email} onChange={handleChange} required />
        </div>
        <div>
          <label>Username:</label><br />
          <input type="text" name="userName" value={formData.userName} onChange={handleChange} required />
        </div>
        <div>
          <label>Password:</label><br />
          <input type="password" name="password" value={formData.password} onChange={handleChange} required />
        </div>
        <button type="submit">Register</button>
      </form>
    </div>
  );
};

export default Register;
