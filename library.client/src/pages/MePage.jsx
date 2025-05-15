import { useEffect, useState, useRef } from "react";
import { getBorrowedBooks, deleteBorrowedBooks } from "../services/libraryService";
import { useNavigate } from "react-router-dom";
import { useSnackbar } from "notistack";
import ApiEndpoints from "../api/ApiEndpoints";

export default function MePage() {
    const [borrowedBooks, setBorrowedBooks] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();
    const notifiedRef = useRef(false);

    useEffect(() => {
        const loadBooks = async () => {
            try {
                const response = await getBorrowedBooks();
                const books = response.items || [];
                setBorrowedBooks(books);
    
                const hasOverdue = books.some(
                    (book) => new Date(book.bookOwner.returnDate) <= new Date()
                );
    
                if (hasOverdue && !notifiedRef.current) {
                    enqueueSnackbar("Please, return the book.", { variant: "error" });
                    notifiedRef.current = true;
                }
            } catch (error) {
                console.error("Error fetching borrowed books:", error);
                navigate("/login");
            } finally {
                setLoading(false);
            }
        };
    
        loadBooks();
    }, [navigate, enqueueSnackbar]);

    const handleReturnBook = async (bookId) => {
        if (!window.confirm("Are you sure you want to return this book?")) return;

        try {
            await deleteBorrowedBooks(bookId);
            setBorrowedBooks((prev) => prev.filter((book) => book.id !== bookId));
            enqueueSnackbar("Book returned successfully!", { variant: "success" });
        } catch (error) {
            console.error("Error returning book:", error);
            enqueueSnackbar("Failed to return the book. Please try again.", { variant: "error" });
        }
    };

    if (loading)
        return (
            <div className="container mt-5 text-center">
                <div className="spinner-border text-primary" role="status" />
                <div>Loading borrowed books...</div>
            </div>
        );

    return (
        <div className="container mt-5">
            <h2 className="mb-5 text-center fw-bold text-primary">My Borrowed Books</h2>
            {borrowedBooks.length === 0 ? (
                <div className="alert alert-info text-center fs-5">You haven't borrowed any books yet.</div>
            ) : (
                <div className="row g-4">
                    {borrowedBooks.map((book) => {
                        const isOverdue = new Date(book.bookOwner.returnDate) <= new Date();

                        return (
                            <div key={book.id} className={`col-md-6 col-lg-4`}>
                                <div
                                    className={`card h-100 shadow-sm border-0 rounded-3 overflow-hidden ${isOverdue ? "bg-danger text-white" : ""
                                        }`}
                                >
                                    <img
                                        src={ApiEndpoints.Book.GetImage(book.id)}
                                        className="card-img-top"
                                        alt={book.title}
                                        style={{ height: "200px", objectFit: "cover" }}
                                    />
                                    <div className="card-body d-flex flex-column">
                                        <h5 className="card-title text-truncate" title={book.title}>
                                            {book.title}
                                        </h5>
                                        <p className={`mb-1 ${isOverdue ? "text-white" : "text-secondary"}`}>
                                            <strong>Genre:</strong> {book.genre}
                                        </p>
                                        <p className={`mb-1 ${isOverdue ? "text-white" : "text-secondary"}`}>
                                            <strong>Author:</strong> {book.author.name}
                                        </p>
                                        <p className={`mb-1 ${isOverdue ? "text-white" : "text-secondary"}`}>
                                            <strong>Taken:</strong>{" "}
                                            {new Date(book.bookOwner.takenDate).toLocaleDateString()}
                                        </p>
                                        <p className={`mb-3 ${isOverdue ? "text-white" : "text-success"}`}>
                                            <strong>Return By:</strong>{" "}
                                            {new Date(book.bookOwner.returnDate).toLocaleDateString()}
                                        </p>
                                        <button
                                            className={`btn mt-3 ${isOverdue ? "btn-light text-danger" : "btn-success"
                                                }`}
                                            onClick={() => handleReturnBook(book.id)}
                                        >
                                            Return Book
                                        </button>
                                    </div>
                                </div>
                            </div>
                        );
                    })}

                </div>
            )}
        </div>
    );
}