import React, { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { Link } from "react-router-dom";

const BooksView = ({ books }) => {
  const [userRole, setUserRole] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      const decoded = jwtDecode(token);
      const roles = decoded.role || [];
      if (roles.includes("admin")) {
        setUserRole("admin");
      } else if (roles.includes("manager")) {
        setUserRole("manager");
      } else {
        setUserRole("user");
      }
    }
  }, []);

  if (!Array.isArray(books)) {
    return <p className="text-danger">No books available or data is not an array.</p>;
  }

  return (
    <div className="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
      {books.map((book) => (
        <div key={book.id} className="col">
          <div className="card h-100 shadow-sm">
            <div className="card-body d-flex flex-column">
              <h5 className="card-title text-primary">{book.title}</h5>
              <p className="card-text">
                <strong>Genre:</strong> {book.genre}<br />
                <strong>ISBN:</strong> {book.isbn}<br />
                <strong>Author:</strong> {book.author?.name}
              </p>
              {book.bookOwner !== null ? (
                <p className="text-danger fw-semibold">Currently Taken</p>
              ) : (<p className="text-success fw-semibold">Available</p>)}
              <div className="mt-auto">
                <Link to={`/books/${book.id}`} className="btn btn-outline-primary w-100">
                  View Details
                </Link>
              </div>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default BooksView;
