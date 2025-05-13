import React, { useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';

const BooksView = ({ books }) => {
  const [userRole, setUserRole] = useState(null);
  const [userSub, setUserSub] = useState(null);

  const handleEditBook = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
        ...prev,
        [name]: value || null
    }));
};

const handleDeleteBook = (e) => {
  const { name, value } = e.target;
  setFormData(prev => ({
      ...prev,
      [name]: value || null
  }));
};

  useEffect(() => {
    const token = localStorage.getItem("accessToken");

    if (token) {
      const decoded = jwtDecode(token);
      // Assuming roles are in the 'roles' property
      const roles = decoded.role || [];
      if (roles.includes('admin')) {
        setUserRole('admin');
      } else if (roles.includes('manager')) {
        setUserRole('manager');
      } else {
        setUserRole('user'); // No roles or other roles
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
              {books.map(book => (
                <li key={book.id}>
                  {book.title}
                  {book.bookOwner ? (
                    <span style={{ color: 'red' }}> - is taken</span>
                  ) : null}

                  {userRole === 'admin' && (
                    <>
                      <button type="button" class="btn btn-info" onClick={handleEditBook}>Edit</button>
                      <button type="button" class="btn btn-danger" onClick={handleDeleteBook}>Delete</button>
                    </>
                  )}

                  {userRole === 'manager' && (
                    <button type="button" class="btn btn-info" onClick={handleEditBook}>Edit</button>
                  )}
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