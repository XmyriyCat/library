import { useParams, Link, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { useSnackbar } from "notistack";
import { fetchAuthorById, deleteAuthor, fetchAuthorBooks } from "../services/libraryService";
import ApiEndpoints from "../api/ApiEndpoints";

export default function AuthorDetails() {
  const { id } = useParams();
  const [author, setAuthor] = useState({});
  const [books, setBooks] = useState([]);
  const [showBooks, setShowBooks] = useState(false);
  const [userRole, setUserRole] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const navigate = useNavigate();
  const { enqueueSnackbar } = useSnackbar();

  useEffect(() => {
    const loadAuthor = async () => {
      try {
        const receivedAuthor = await fetchAuthorById(id);
        setAuthor(receivedAuthor);
      } catch (error) {
        console.error("Error fetching author:", error);
      }
    };

    const token = localStorage.getItem("accessToken");
    if (token) {
      const decoded = jwtDecode(token);
      const roles = decoded.role || [];
      setUserRole(
        roles.includes("admin") ? "admin" :
          roles.includes("manager") ? "manager" : "user"
      );
    }

    loadAuthor();
  }, [id]);

  const toggleBooks = async () => {
    if (!showBooks) {
      try {
        const response = await fetchAuthorBooks(id);
        setBooks(response.data);
      } catch (error) {
        console.error("Error fetching author's books:", error);
      }
    }
    setShowBooks(!showBooks);
  };

  const confirmDelete = async () => {
    try {
      await deleteAuthor(author.id);
      setShowModal(false);
      enqueueSnackbar("Author deleted successfully!", { variant: "info" });
      navigate("/dashboard?view=authors");
    } catch (error) {
      console.error("Error deleting author:", error);
      alert("Failed to delete author.");
      setShowModal(false);
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-8">
          <div className="card shadow">
            <div className="card-header bg-primary text-white">
              <h4>Author Details</h4>
            </div>
            <div className="card-body">
              <h5 className="card-title">{author.name}</h5>
              <p className="card-text"><strong>Country:</strong> {author.country}</p>
              <p className="card-text">
                <strong>Date of Birth:</strong>{" "}
                {author.dateOfBirth ? new Date(author.dateOfBirth).toLocaleDateString() : "N/A"}
              </p>

              <div className="d-flex gap-2 flex-wrap mb-3">
                {(userRole === "admin" || userRole === "manager") && (
                  <Link to={`/authors/${author.id}/edit`} className="btn btn-primary">
                    Edit Author
                  </Link>
                )}
                {userRole === "admin" && (
                  <button className="btn btn-danger" onClick={() => setShowModal(true)}>
                    Delete Author
                  </button>
                )}
                <button className="btn btn-warning" onClick={toggleBooks}>
                  {showBooks ? "Hide Books" : "Author Books"}
                </button>
                <Link to="/dashboard?view=authors" className="btn btn-secondary ms-auto">
                  Back to Dashboard
                </Link>
              </div>

              {showBooks && (
                <div className="row mt-4">
                  {books.length === 0 ? (
                    <p className="text-muted">No books found for this author.</p>
                  ) : (
                    books.map((book) => (
                      <div key={book.id} className="col-md-6 mb-3">
                        <div className="card h-100 shadow-sm">
                          <img
                            src={ApiEndpoints.Book.GetImage(book.id)}
                            className="card-img-top"
                            alt={book.title}
                            style={{ height: "200px", objectFit: "cover" }}
                          />
                          <div className="card-body">
                            <h5 className="card-title">{book.title}</h5>
                            <p className="card-text text-muted">{book.genre}</p>
                            <Link to={`/books/${book.id}`} className="btn btn-outline-primary w-100">
                              View Details
                            </Link>
                          </div>
                        </div>
                      </div>
                    ))
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      {showModal && (
        <div className="modal fade show d-block" tabIndex="-1"
          style={{ backgroundColor: "rgba(0, 0, 0, 0.6)", backdropFilter: "blur(3px)" }}
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
                  <strong className="text-danger">{author.name}</strong>?
                </p>
              </div>
              <div className="modal-footer justify-content-center border-0 pb-4">
                <button className="btn btn-outline-secondary px-4" onClick={() => setShowModal(false)}>
                  Cancel
                </button>
                <button className="btn btn-danger px-4" onClick={confirmDelete}>
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