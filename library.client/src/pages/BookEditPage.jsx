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
                const recievedAuthors = await fetchAuthors()
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

        try {
            const formData = new FormData();
            formData.append("title", book.title);
            formData.append("isbn", book.isbn);
            formData.append("genre", book.genre);
            formData.append("description", book.description);
            formData.append("authorId", book.author?.id || "");

            if (imageFile) {
                formData.append("image", imageFile);
            } else if (imageBlob) {
                formData.append("image", imageBlob, `${book.title || "image"}.png`);
            }

            if (pageMode === "update") {
                await updateBook(book.id, formData);
                enqueueSnackbar("Book updated successfully!", { variant: "success" });
                navigate(`/books/${book.id}`);
            } else if (pageMode === "create") {
                const response = await createBook(formData);
                if (response && response.id) {
                    enqueueSnackbar("Book created successfully!", { variant: "success" });
                    navigate(`/books/${response.id}`);
                } else {
                    console.error("Book creation failed or returned no ID", response);
                    enqueueSnackbar("Book creation failed.", { variant: "error" });
                }
            }
        } catch (error) {
            console.error("Error processing book:", error);
            alert("Fail! Something went wrong...");
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
                                        className="form-control"
                                        value={book.title || ""}
                                        onChange={(e) => setBook({ ...book, title: e.target.value })}
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
                                <div className="mb-3">
                                    <label className="form-label">Upload New Image</label>
                                    <input
                                        type="file"
                                        className="form-control"
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
