import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./components/Login";
import Register from "./components/Register";
import Dashboard from "./pages/Dashboard";
import BookDetails from "./pages/BookDetailsPage";
import BookEdit from "./pages/BookEditPage";
import Navbar from "./components/Navbar";
import "bootstrap/dist/css/bootstrap.min.css";

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/books/:id" element={<BookDetails />} />
        <Route path="/books/:id/edit" element={<BookEdit />} />
        
      </Routes>
    </Router>
  );
}

export default App;
