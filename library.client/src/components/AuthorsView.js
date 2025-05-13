import React from 'react';

const AuthorsView = ({ authors }) => {
    return (
      <div>
        <h2>Authors</h2>
        <ul>
            {authors.map(author => (
                <li key={author.id}>
                    {author.name}
                </li>
            ))}
        </ul>
      </div>

    );
};

export default AuthorsView;