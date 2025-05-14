import { useParams, Link } from "react-router-dom";
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import {
  fetchBookById,
  getBookImage,
  updateBook,
  deleteBook,
  takeBook,
} from "../services/libraryService";

export default function BookDetails() {
  const { id } = useParams();

  const [book, setBook] = useState({});
  const [userRole, setUserRole] = useState(null);
  const [image, setImage] = useState(null);

  useEffect(() => {
    const fetchBook = async () => {
      try {
        const recievedBook = await fetchBookById(id);
        setBook(recievedBook);
      } catch (error) {
        console.error("Error fetching book:", error);
      }
    };

    const fetchImage = async () => {
      try {
        const res = await getBookImage(id);
        if (!res.ok) throw new Error("Image fetch failed");

        const blob = await res.blob();
        const url = URL.createObjectURL(blob);
        setImage(url);
      } catch (err) {
        console.error("Image fetch error:", err);
      }
    };

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

    fetchBook();
    fetchImage();
  }, [id]);

  const handleTakeBook = async (bookId) => {
    try {
      await takeBook(bookId);
    } catch (error) {
      console.error("Error taking book:", error);
    }
  };

  const handleDeleteBook = async (bookId) => {
    try {
      await deleteBook(bookId);
    } catch (error) {
      console.error("Error deleting book:", error);
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-8">
          <div className="card shadow">
            <div className="card-header bg-primary text-white">
              <h4>Book Details</h4>
            </div>
            <div className="card-body">

              <h5 className="card-title">{book.title}</h5>
              <p className="card-text"><strong>ISBN:</strong> {book.isbn}</p>
              <p className="card-text"><strong>Genre:</strong> {book.genre}</p>
              <p className="card-text"><strong>Description:</strong> {book.description}</p>
              <p className="card-text"><strong>Author:</strong> {book.author?.name}</p>
              <p className="card-text">
                <strong>Status:</strong>{" "}
                {book.bookOwner !== null ? (
                  <span className="text-danger fw-semibold">Currently Taken</span>
                ) : (
                  <span className="text-success fw-semibold">Available</span>
                )}
              </p>
              {image ? (
                <div className="text-center mb-3">
                  <img
                    src={image}
                    alt={book.title}
                    className="img-thumbnail"
                    style={{ maxHeight: "300px" }}
                  />
                </div>
              ) : (
                <p>Loading image...</p>
              )}
              <div className="d-flex gap-2 flex-wrap">

                {(userRole !== null) && (
                  <button
                    className="btn btn-success"
                    onClick={() => handleTakeBook(book.id)}>
                    Take Book
                  </button>
                )}
                {(userRole === "admin" || userRole === "manager") && (
                  <Link to={`/books/${book.id}/edit`} className="btn btn-primary">
                    Edit Book
                  </Link>
                )}
                {userRole === "admin" && (
                  <button
                    className="btn btn-danger"
                    onClick={() => handleDeleteBook(book.id)}
                  >
                    Delete Book
                  </button>
                )}
                <Link to="/dashboard" className="btn btn-secondary ms-auto">
                  Back to Dashboard
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div >
  );
}
