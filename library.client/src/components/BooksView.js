import React from 'react';

const BooksView = ({ books }) => {

    if (!Array.isArray(books)) {
        return <p>No books available or data is not an array.</p>;
    }

    return (
        <div>
          <h2>Books</h2>
          <ul>
            {books.map(book => (
              <li key={book.id}>
                {book.title} 
                {book.bookOwner ? (
                  <span style={{ color: 'red' }}> - is taken</span>
                ) : null}
              </li>
            ))}
          </ul>
        </div>
      );
};

export default BooksView;