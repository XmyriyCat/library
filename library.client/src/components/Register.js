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

  const [fieldErrors, setFieldErrors] = useState({});
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const validate = () => {
    const errors = {};
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const userNameRegex = /^[a-zA-Z0-9 _@.+-]+$/;

    if (!formData.email.trim()) {
      errors.email = 'Email is required.';
    } else if (!emailRegex.test(formData.email)) {
      errors.email = 'Email format is invalid.';
    }

    if (!formData.userName.trim()) {
      errors.userName = 'Username is required.';
    } else if (!userNameRegex.test(formData.userName)) {
      errors.userName = 'Username contains invalid characters.';
    } else if (formData.userName.length < 6) {
      errors.userName = 'Username must be at least 6 characters long.';
    } else if (formData.userName.length > 50) {
      errors.userName = 'Username must not exceed 50 characters.';
    }

    if (!formData.password.trim()) {
      errors.password = 'Password is required.';
    } else if (formData.password.length < 6) {
      errors.password = 'Password must be at least 6 characters long.';
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleChange = (e) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    if (!validate()) return;

    try {
      const response = await axios.post(ApiEndpoints.Auth.Register, formData);
      const { accessToken, refreshToken } = response.data;
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', refreshToken);
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
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-6 col-lg-4">
          <div className="card shadow-lg p-4">
            <h2 className="text-center mb-4">Register</h2>
            {error && <div className="alert alert-danger">{error}</div>}
            <form onSubmit={handleSubmit} noValidate>
              <div className="mb-3">
                <label className="form-label">Email</label>
                <input
                  type="email"
                  className={`form-control ${fieldErrors.email ? 'is-invalid' : ''}`}
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  placeholder="Enter your email"
                />
                {fieldErrors.email && (
                  <div className="invalid-feedback">{fieldErrors.email}</div>
                )}
              </div>
              <div className="mb-3">
                <label className="form-label">Username</label>
                <input
                  type="text"
                  className={`form-control ${fieldErrors.userName ? 'is-invalid' : ''}`}
                  name="userName"
                  value={formData.userName}
                  onChange={handleChange}
                  placeholder="Enter your username"
                />
                {fieldErrors.userName && (
                  <div className="invalid-feedback">{fieldErrors.userName}</div>
                )}
              </div>
              <div className="mb-3">
                <label className="form-label">Password</label>
                <input
                  type="password"
                  className={`form-control ${fieldErrors.password ? 'is-invalid' : ''}`}
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  placeholder="Enter your password"
                />
                {fieldErrors.password && (
                  <div className="invalid-feedback">{fieldErrors.password}</div>
                )}
              </div>
              <div className="d-grid">
                <button type="submit" className="btn btn-primary">Register</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;