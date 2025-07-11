import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { fetchAuthorById, updateAuthor, createAuthor } from "../services/libraryService";
import { useSnackbar } from "notistack";

export default function AuthorEditPage() {
    const { id } = useParams();
    const [author, setAuthor] = useState({});
    const [errors, setErrors] = useState({});
    const [userRole, setUserRole] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [pageMode, setPageMode] = useState("create");
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();

    useEffect(() => {
        const loadData = async () => {
            try {
                if (id !== undefined) {
                    setPageMode("update");
                    const receivedAuthor = await fetchAuthorById(id);
                    setAuthor(receivedAuthor);
                }
            } catch (err) {
                console.error("Error loading author details:", err);
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

    const validate = () => {
        const newErrors = {};
        if (!author.name || author.name.trim() === "") {
            newErrors.name = "Name is required.";
        }
        if (!author.country || author.country.trim() === "") {
            newErrors.country = "Country is required.";
        }
        if (!author.dateOfBirth || author.dateOfBirth.trim() === "") {
            newErrors.dateOfBirth = "Date of Birth is required.";
        }
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleUpdateAuthor = async (e) => {
        e.preventDefault();
        if (!validate()) return;

        try {
            const authorPayload = {
                name: author.name,
                country: author.country,
                dateOfBirth: author.dateOfBirth
            };

            if (pageMode === "update") {
                await updateAuthor(author.id, authorPayload);
                enqueueSnackbar("Author updated successfully!", { variant: "success" });
                navigate(`/authors/${author.id}`);
            } else if (pageMode === "create") {
                const response = await createAuthor(authorPayload);
                if (response && response.id) {
                    enqueueSnackbar("Author created successfully!", { variant: "success" });
                    navigate(`/authors/${response.id}`);
                } else {
                    console.error("Author creation failed or returned no ID", response);
                    enqueueSnackbar("Author creation failed.", { variant: "error" });
                }
            }
        } catch (error) {
            console.error("Error processing author:", error);
            alert("Fail! Something went wrong...");
        }
    };

    if (isLoading) {
        return <div className="container mt-5">Loading author details...</div>;
    }

    return (
        <div className="container mt-5">
            <div className="row justify-content-center">
                <div className="col-md-8">
                    <div className="card shadow">
                        <div className="card-header bg-primary text-white">
                            <h4>{pageMode === "update" ? "Edit Author Details" : "Create New Author"}</h4>
                        </div>
                        <div className="card-body">
                            <form onSubmit={handleUpdateAuthor}>
                                <div className="mb-3">
                                    <label className="form-label">Name</label>
                                    <input
                                        type="text"
                                        className={`form-control ${errors.name ? "is-invalid" : ""}`}
                                        value={author.name || ""}
                                        onChange={(e) => setAuthor({ ...author, name: e.target.value })}
                                    />
                                    {errors.name && <div className="invalid-feedback">{errors.name}</div>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label">Country</label>
                                    <input
                                        type="text"
                                        className={`form-control ${errors.country ? "is-invalid" : ""}`}
                                        value={author.country || ""}
                                        onChange={(e) => setAuthor({ ...author, country: e.target.value })}
                                    />
                                    {errors.country && <div className="invalid-feedback">{errors.country}</div>}
                                </div>

                                <div className="mb-3">
                                    <label className="form-label">Date of Birth</label>
                                    <input
                                        type="date"
                                        className={`form-control ${errors.dateOfBirth ? "is-invalid" : ""}`}
                                        value={author.dateOfBirth ? new Date(author.dateOfBirth).toISOString().split("T")[0] : ""}
                                        onChange={(e) => {
                                            const isoDate = new Date(e.target.value).toISOString();
                                            setAuthor({ ...author, dateOfBirth: isoDate });
                                        }}
                                    />
                                    {errors.dateOfBirth && <div className="invalid-feedback">{errors.dateOfBirth}</div>}
                                </div>

                                <div className="d-grid">
                                    <button type="submit" className="btn btn-primary">
                                        {pageMode === "update" ? "Save Changes" : "Create Author"}
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
