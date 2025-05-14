import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { fetchBookById, getBookImage, updateBook, fetchAuthors } from "../services/libraryService";

export default function BookDetails() {
    const { id } = useParams();

    const [book, setBook] = useState({});
    const [authors, setAuthors] = useState([]);
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

        const fetchAllAuthors = async () => {
            try {
                const recievedAuthors = await fetchAuthors();
                setAuthors(recievedAuthors.items);
            } catch (error) {
                console.error("Error fetching authors:", error);
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
            setUserRole(roles.includes("admin") ? "admin" : roles.includes("manager") ? "manager" : "user");
            fetchBook();
            fetchAllAuthors();
            fetchImage();
        }
    }, [id]);

    const handleUpdateBook = async (e) => {
        e.preventDefault();
        try {
            const response = await updateBook(book.id, book);
            setBook(response.data);
        } catch (error) {
            console.error("Error updating book:", error);
        }
    };

    return (
        <div className="container mt-5">
            <div className="row justify-content-center">
                <div className="col-md-8">
                    <div className="card shadow">
                        <div className="card-header bg-primary text-white">
                            <h4>Edit Book Details</h4>
                        </div>
                        <div className="card-body">
                            <form onSubmit={handleUpdateBook}>
                                <div className="mb-3">
                                    <label className="form-label">Title</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={book.title || ""}
                                        onChange={(e) => setBook({ ...book, title: e.target.value })}
                                    />
                                </div>
                                <div className="mb-3">
                                    <label className="form-label">ISBN</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={book.isbn || ""}
                                        onChange={(e) => setBook({ ...book, isbn: e.target.value })}
                                    />
                                </div>
                                <div className="mb-3">
                                    <label className="form-label">Genre</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={book.genre || ""}
                                        onChange={(e) => setBook({ ...book, genre: e.target.value })}
                                    />
                                </div>
                                <div className="mb-3">
                                    <label className="form-label">Description</label>
                                    <textarea
                                        className="form-control"
                                        rows="3"
                                        value={book.description || ""}
                                        onChange={(e) => setBook({ ...book, description: e.target.value })}
                                    ></textarea>
                                </div>
                                <div className="mb-3">
                                    <label className="form-label">Author</label>
                                    <select
                                        className="form-select"
                                        value={book.author?.id || ""}
                                        onChange={(e) => {
                                            const selectedAuthor = authors.find(a => a.id === e.target.value);
                                            setBook({ ...book, author: selectedAuthor });
                                        }}
                                    >
                                        <option value="">Select an author</option>
                                        {authors.map((author) => (
                                            <option key={author.id} value={author.id}>
                                                {author.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="mb-3">
                                    <label className="form-label">Upload New Image</label>
                                    <input
                                        type="file"
                                        className="form-control"
                                        accept="image/*"
                                        onChange={(e) => {
                                            const file = e.target.files[0];
                                            if (file) {
                                                if (image && typeof image === "string") {
                                                    URL.revokeObjectURL(image);
                                                }
                                                const imageUrl = URL.createObjectURL(file);
                                                setImage(imageUrl);
                                            }
                                        }}
                                    />
                                </div>
                                {image && typeof image === "string" && (
                                    <div className="mb-3 text-center">
                                        <img
                                            src={image}
                                            alt="Book Cover"
                                            className="img-thumbnail"
                                            style={{ maxHeight: "300px" }}
                                        />
                                    </div>
                                )}
                                <div className="d-grid">
                                    <button type="submit" className="btn btn-primary">
                                        Save Changes
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
