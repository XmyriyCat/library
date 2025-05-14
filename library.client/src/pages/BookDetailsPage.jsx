import { useParams, Link, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { fetchBookById, getBookImage, deleteBook, takeBook } from "../services/libraryService";
import { useSnackbar } from "notistack";

export default function BookDetails() {
  const { id } = useParams();
  const [book, setBook] = useState({});
  const [userRole, setUserRole] = useState(null);
  const [image, setImage] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const navigate = useNavigate();
  const { enqueueSnackbar } = useSnackbar();

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
      const result = await takeBook(bookId);

      if (result && result.returnDate) {
        const returnDate = new Date(result.returnDate).toLocaleDateString();
        alert(`Book taken successfully! Please return it by ${returnDate}.`);
        const updatedBook = await fetchBookById(bookId);
        setBook(updatedBook);
      }
    } catch (error) {
      console.error("Error taking book:", error);
      alert("Failed to take book. Please try again.");
    }
  };

  const confirmDelete = async () => {
    try {
      await deleteBook(book.id);
      setShowModal(false);
      enqueueSnackbar("Book deleted successfully!", { variant: "info" });
      navigate("/dashboard");
    } catch (error) {
      console.error("Error deleting book:", error);
      alert("Failed to delete book.");
      setShowModal(false);
    }
  };

  return (
    <div>
      <div className="container mt-5">
        <div className="row justify-content-center">
          <div className="col-md-8">
            <div className="card shadow">
              <div className="card-header bg-primary text-white">
                <h4>Book Details</h4>
              </div>
              <div className="card-body">
                <h5 className="card-title">{book.title}</h5>
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
                <div className="d-flex gap-2 flex-wrap">

                  {userRole !== null && (
                    <button
                      className="btn btn-success"
                      onClick={() => handleTakeBook(book.id)}
                      disabled={book.bookOwner !== null}
                      title={book.bookOwner !== null ? "Book is currently taken" : "Take this book"}
                    >
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
                      onClick={() => setShowModal(true)}
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
      {showModal && (
        <div
          className="modal fade show d-block"
          tabIndex="-1"
          style={{
            backgroundColor: "rgba(0, 0, 0, 0.6)",
            backdropFilter: "blur(3px)"
          }}
        >
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content shadow-lg rounded-4 border-0">
              <div className="modal-header bg-danger text-white rounded-top-4">
                <h5 className="modal-title">
                  <i className="bi bi-exclamation-triangle-fill me-2"></i>
                  Confirm Deletion
                </h5>
                <button
                  type="button"
                  className="btn-close btn-close-white"
                  onClick={() => setShowModal(false)}
                ></button>
              </div>
              <div className="modal-body text-center">
                <p className="fs-5">
                  Are you sure you want to delete
                  <br />
                  <strong className="text-danger">{book.title}</strong>?
                </p>
              </div>
              <div className="modal-footer justify-content-center border-0 pb-4">
                <button
                  className="btn btn-outline-secondary px-4"
                  onClick={() => setShowModal(false)}
                >
                  Cancel
                </button>
                <button
                  className="btn btn-danger px-4"
                  onClick={confirmDelete}
                >
                  Yes, Delete
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
