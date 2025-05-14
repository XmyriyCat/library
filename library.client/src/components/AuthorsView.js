import React from "react";
import { Link } from "react-router-dom";

const AuthorsView = ({ authors }) => {
  if (!Array.isArray(authors)) {
    return <p className="text-danger">No authors available or data is not an array.</p>;
  }

  return (
    <div className="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
      {authors.map((author) => (
        <div key={author.id} className="col">
          <div className="card h-100 shadow-sm">
            <div className="card-body d-flex flex-column">
              <h5 className="card-title text-primary">{author.name}</h5>
              <p className="card-text">
                <strong>Country:</strong> {author.country || "No country info."}
              </p>
              <div className="mt-auto">
                <Link to={`/authors/${author.id}`} className="btn btn-outline-primary w-100">
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

export default AuthorsView;
