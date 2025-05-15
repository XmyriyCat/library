import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { fetchBookById, getBookImage, updateBook, fetchAuthors, createBook } from "../services/libraryService";
import { useSnackbar } from "notistack";

export default function BookDetails() {
    const { id } = useParams();
    const [book, setBook] = useState({});
    const [authors, setAuthors] = useState([]);
    const [userRole, setUserRole] = useState(null);
    const [image, setImage] = useState(null);
    const [imageBlob, setImageBlob] = useState(null);
    const [imageFile, setImageFile] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [pageMode, setPageMode] = useState("create");
    const [validationErrors, setValidationErrors] = useState({});
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();

    useEffect(() => {
        const loadData = async () => {
            try {
                if (id !== undefined) {
                    setPageMode("update")
                    const recievedBook = await fetchBookById(id)
                    setBook(recievedBook);
                    const res = await getBookImage(id);
                    if (res.ok) {
                        const blob = await res.blob();
                        setImageBlob(blob);
                        setImage(URL.createObjectURL(blob));
                    }
                }
                const recievedAuthors = await fetchAuthors({ page: 1, pageSize: 30 })
                setAuthors(recievedAuthors.items)
            } catch (err) {
                console.error("Error loading book details:", err);
            } finally {
                setIsLoading(false);
            }
        };

        const token = localStorage.getItem("accessToken");
        if (token) {
            const decoded = jwtDecode(token);
            const roles = decoded.role || [];
            setUserRole(roles.includes("admin") ? "admin" : roles.includes("manager") ? "manager" : "user");
            loadData();
        }
    }, [id]);

    const handleUpdateBook = async (e) => {
        e.preventDefault();
        const errors = {};

        if (!book.isbn || book.isbn.length < 10 || book.isbn.length > 20) {
            errors.isbn = "ISBN must be between 10 and 20 characters.";
        }
        if (!book.title || book.title.length > 50) {
            errors.title = "Title is required and must be less than 50 characters.";
        }
        if (!book.genre || book.genre.length > 30) {
            errors.genre = "Genre is required and must be less than 30 characters.";
        }
        if (!book.description || book.description.length > 300) {
            errors.description = "Description is required and must be less than 300 characters.";
        }
        if (!book.author || !book.author.id) {
            errors.author = "Author selection is required.";
        }
        if (!imageFile && !imageBlob) {
            errors.image = "Book image is required.";
        }

        setValidationErrors(errors);
        if (Object.keys(errors).length > 0) {
            return;
        }

        try {
            const formData = new FormData();
            formData.append("title", book.title);
            formData.append("isbn", book.isbn);
            formData.append("genre", book.genre);
            formData.append("description", book.description);
            formData.append("authorId", book.author.id);

            if (imageFile) {
                formData.append("image", imageFile);
            } else if (imageBlob) {
                formData.append("image", imageBlob, `${book.title || "image"}.png`);
            }

            if (pageMode === "update") {
                await updateBook(book.id, formData);
                enqueueSnackbar("Book updated successfully!", { variant: "success" });
                navigate(`/books/${book.id}`);
            } else {
                const response = await createBook(formData);
                if (response && response.id) {
                    enqueueSnackbar("Book created successfully!", { variant: "success" });
                    navigate(`/books/${response.id}`);
                } else {
                    enqueueSnackbar("Book creation failed.", { variant: "error" });
                }
            }
        } catch (error) {
            console.error("Error processing book:", error);
            enqueueSnackbar("Fail! Something went wrong...", { variant: "error" });
        }
    };

    if (isLoading) {
        return <div className="container mt-5">Loading book details...</div>;
    }

    return (
        <div className="container mt-5">
            <div className="row justify-content-center">
                <div className="col-md-8">
                    <div className="card shadow">
                        <div className="card-header bg-primary text-white">
                            <h4>{pageMode === "update" ? "Edit Book Details" : "Create New Book"}</h4>
                        </div>
                        <div className="card-body">
                            <form onSubmit={handleUpdateBook}>
                                <div className="mb-3">
                                    <label className="form-label">Title</label>
                                    <input
                                        type="text"
                                        className={`form-control ${validationErrors.title ? 'is-invalid' : ''}`}
                                        value={book.title || ""}
                                        onChange={(e) => setBook({ ...book, title: e.target.value })}
                                    />
                                    {validationErrors.title && <div className="text-danger mt-1">{validationErrors.title}</div>}
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

                                <div className="mb-3">
                                    <label className="form-label">Upload New Image</label>
                                    <input
                                        type="file"
                                        className={`form-control ${validationErrors.image ? 'is-invalid' : ''}`}
                                        accept="image/*"
                                        onChange={(e) => {
                                            const file = e.target.files[0];
                                            setImageFile(file);

                                            if (file) {
                                                if (image && typeof image === "string") {
                                                    URL.revokeObjectURL(image);
                                                }
                                                const imageUrl = URL.createObjectURL(file);
                                                setImage(imageUrl);
                                            }
                                        }}
                                    />
                                    {validationErrors.image && <div className="text-danger mt-1">{validationErrors.image}</div>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label">ISBN</label>
                                    <input
                                        type="text"
                                        className={`form-control ${validationErrors.isbn ? 'is-invalid' : ''}`}
                                        value={book.isbn || ""}
                                        onChange={(e) => setBook({ ...book, isbn: e.target.value })}
                                    />
                                    {validationErrors.isbn && <div className="text-danger mt-1">{validationErrors.isbn}</div>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label">Genre</label>
                                    <input
                                        type="text"
                                        className={`form-control ${validationErrors.genre ? 'is-invalid' : ''}`}
                                        value={book.genre || ""}
                                        onChange={(e) => setBook({ ...book, genre: e.target.value })}
                                    />
                                    {validationErrors.genre && <div className="text-danger mt-1">{validationErrors.genre}</div>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label">Description</label>
                                    <textarea
                                        className={`form-control ${validationErrors.description ? 'is-invalid' : ''}`}
                                        rows="3"
                                        value={book.description || ""}
                                        onChange={(e) => setBook({ ...book, description: e.target.value })}
                                    ></textarea>
                                    {validationErrors.description && <div className="text-danger mt-1">{validationErrors.description}</div>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label">Author</label>
                                    <select
                                        className={`form-select ${validationErrors.author ? 'is-invalid' : ''}`}
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
                                    {validationErrors.author && <div className="text-danger mt-1">{validationErrors.author}</div>}
                                </div>

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
