import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { ApiEndpoints } from '../api/ApiEndpoints';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fieldErrors, setFieldErrors] = useState({});
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const validate = () => {
    const errors = {};

    if (!email.trim()) {
      errors.email = 'Email is required.';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      errors.email = 'Invalid email format.';
    }

    if (!password.trim()) {
      errors.password = 'Password is required.';
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    if (!validate()) return;

    try {
      const response = await axios.post(ApiEndpoints.Auth.Login, {
        email,
        password,
      });

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
        setError('Login failed: ' + err.response.data.message);
      } else if (err.response?.data) {
        setError('Login failed: ' + err.response.data);
      } else {
        setError('Login failed: An unknown error occurred.');
      }
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-6 col-lg-4">
          <div className="card shadow-lg p-4">
            <h2 className="text-center mb-4">Login</h2>
            {error && <div className="alert alert-danger">{error}</div>}
            <form onSubmit={handleSubmit} noValidate>
              <div className="mb-3">
                <label className="form-label">Email</label>
                <input
                  type="email"
                  className={`form-control ${fieldErrors.email ? 'is-invalid' : ''}`}
                  placeholder="Enter your email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
                {fieldErrors.email && (
                  <div className="invalid-feedback">{fieldErrors.email}</div>
                )}
              </div>
              <div className="mb-3">
                <label className="form-label">Password</label>
                <input
                  type="password"
                  className={`form-control ${fieldErrors.password ? 'is-invalid' : ''}`}
                  placeholder="Enter your password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
                {fieldErrors.password && (
                  <div className="invalid-feedback">{fieldErrors.password}</div>
                )}
              </div>
              <div className="d-grid">
                <button type="submit" className="btn btn-primary">
                  Login
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
