import React from 'react';
import { Navigate, Route } from 'react-router-dom';

const PrivateRoute = ({ element: Component, ...rest }) => {
  const accessToken = localStorage.getItem('accessToken');

  if (!accessToken) {
    return <Navigate to="/login" />;
  }

  // If accessToken exists, render the protected route.
  return <Route {...rest} element={<Component />} />;
};

export default PrivateRoute;