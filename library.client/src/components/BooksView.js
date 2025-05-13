import React, { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { Link } from "react-router-dom";


const BooksView = ({ books }) => {
  const [userRole, setUserRole] = useState(null);
  const [userSub, setUserSub] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("accessToken");

    if (token) {
      const decoded = jwtDecode(token);
      // Assuming roles are in the 'roles' property
      const roles = decoded.role || [];
      if (roles.includes("admin")) {
        setUserRole("admin");
      } else if (roles.includes("manager")) {
        setUserRole("manager");
      } else {
        setUserRole("user"); // No roles or other roles
      }
    }
  }, []);

  if (!Array.isArray(books)) {
    return <p>No books available or data is not an array.</p>;
  }

  return (
    <div>
      <div class="my-3 p-3 bg-white rounded box-shadow">
        <h2 className="fs-2 fw-bold text-primary">Books</h2>
        <div class="media text-muted pt-3">
          <div class="media-body pb-3 mb-0 small lh-125 border-bottom border-gray">
            <ul class="text-gray-dark">
              {books.map((book) => (
                <li key={book.id}>
                  <Link to={`/books/${book.id}`}>
                    {book.title}
                    {book.title}
                    {book.bookOwner ? (
                      <span style={{ color: "red" }}> - is taken</span>
                    ) : null}
                  </Link>
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BooksView;
