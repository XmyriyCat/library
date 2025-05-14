import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Login from "./components/Login";
import Register from "./components/Register";
import Dashboard from "./pages/Dashboard";
import BookDetails from "./pages/BookDetailsPage";
import AuthorDetails from "./pages/AuthorDetailsPage";
import BookEdit from "./pages/BookEditPage";
import AuthorEdit from "./pages/AuthorEditPage";
import Navbar from "./components/Navbar";
import "bootstrap/dist/css/bootstrap.min.css";

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/dashboard" element={<Dashboard />} />

        <Route path="/books/:id" element={<BookDetails />} />
        <Route path="/books/:id/edit" element={<BookEdit />} />
        <Route path="/books/create" element={<BookEdit />} />

        <Route path="/authors/:id" element={<AuthorDetails />} />
        <Route path="/authors/:id/edit" element={<AuthorEdit />} />
        <Route path="/authors/create" element={<AuthorEdit />} />

      </Routes>
    </Router>
  );
}

export default App;
