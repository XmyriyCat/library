import React, { useEffect, useState } from "react";
import { fetchBooks, fetchAuthors } from "../services/libraryService";
import BooksView from "../components/BooksView";
import AuthorsView from "../components/AuthorsView";
import { useSearchParams, useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

const Dashboard = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const navigate = useNavigate();
    const view = searchParams.get("view") || "books";
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [userRole, setUserRole] = useState(null);

    const [inputTitle, setInputTitle] = useState("");
    const [inputGenre, setInputGenre] = useState("");
    const [inputAuthor, setInputAuthor] = useState("");

    const [title, setTitle] = useState("");
    const [genre, setGenre] = useState("");
    const [author, setAuthor] = useState("");

    const initialPage = parseInt(searchParams.get("page")) || 1;
    const [page, setPage] = useState(initialPage);
    const [pageSize] = useState(6);
    const [totalPages, setTotalPages] = useState(1);

    useEffect(() => {
        const loadData = async () => {
            setLoading(true);
            try {
                const result = view === "books"
                    ? await fetchBooks({ page, pageSize, title, genre, author })
                    : await fetchAuthors({ page, pageSize });

                setData(result.items || result);

                const totalItems = result.totalItems || 0;
                setTotalPages(Math.max(1, Math.ceil(totalItems / pageSize)));
            } catch (error) {
                console.error("Failed to fetch data", error);
            } finally {
                setLoading(false);
            }

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
        };
        loadData();
    }, [view, page, title, genre, author]);

    const handleApplyFilters = () => {
        setTitle(inputTitle);
        setGenre(inputGenre);
        setAuthor(inputAuthor);
        setPage(1);
        setSearchParams((prev) => {
            const newParams = new URLSearchParams(prev);
            newParams.set("page", 1);
            newParams.set("view", view);
            return newParams;
        });
    };

    const handleCreate = () => {
        navigate(`/${view}/create`);
    };

    return (
        <div className="container mt-5">
            <div className="text-center mb-4">
                <h1 className="fw-bold">Library Dashboard</h1>
                <div className="d-flex justify-content-center align-items-center gap-2 mt-3 flex-wrap">
                    <div className="btn-group">
                        <button
                            className={`btn btn-outline-primary ${view === "books" ? "active" : ""}`}
                            onClick={() => setSearchParams({ view: "books" })}
                        >
                            Books
                        </button>
                        <button
                            className={`btn btn-outline-primary ${view === "authors" ? "active" : ""}`}
                            onClick={() => setSearchParams({ view: "authors" })}
                        >
                            Authors
                        </button>
                    </div>
                    {(userRole === "admin" || userRole === "manager") && (
                        <button className="btn btn-success d-flex align-items-center gap-2" onClick={handleCreate}>
                            <i className="bi bi-plus-lg"></i> Create
                        </button>
                    )}
                </div>
            </div>

            {view === "books" && (
                <div className="mb-4 d-flex flex-wrap gap-2 justify-content-center">
                    <input
                        type="text"
                        placeholder="Search by title"
                        value={inputTitle}
                        onChange={(e) => setInputTitle(e.target.value)}
                        className="form-control w-auto"
                    />
                    <input
                        type="text"
                        placeholder="Filter by genre"
                        value={inputGenre}
                        onChange={(e) => setInputGenre(e.target.value)}
                        className="form-control w-auto"
                    />
                    <input
                        type="text"
                        placeholder="Filter by author"
                        value={inputAuthor}
                        onChange={(e) => setInputAuthor(e.target.value)}
                        className="form-control w-auto"
                    />
                    <button className="btn btn-primary" onClick={handleApplyFilters}>
                        Apply Filters
                    </button>
                    <button
                        className="btn btn-danger"
                        onClick={() => {
                            setInputTitle("");
                            setInputGenre("");
                            setInputAuthor("");

                            setTitle("");
                            setGenre("");
                            setAuthor("");

                            setPage(1);
                            setSearchParams((prev) => {
                                const newParams = new URLSearchParams(prev);
                                newParams.set("page", 1);
                                newParams.set("view", view);
                                return newParams;
                            });
                        }}
                    >
                        Clear
                    </button>
                </div>
            )}


            {loading ? (
                <div className="text-center">
                    <div className="spinner-border text-primary" role="status" />
                </div>
            ) : view === "books" ? (
                <BooksView books={data} />
            ) : (
                <AuthorsView authors={data} />
            )}

            {totalPages > 1 && (
                <div className="d-flex justify-content-center mt-4">
                    <nav>
                        <ul className="pagination">
                            {[...Array(totalPages)].map((_, i) => (
                                <li key={i} className={`page-item ${page === i + 1 ? "active" : ""}`}>
                                    <button
                                        className="page-link"
                                        onClick={() => {
                                            setPage(i + 1);
                                            setSearchParams((prev) => {
                                                const newParams = new URLSearchParams(prev);
                                                newParams.set("page", i + 1);
                                                newParams.set("view", view);
                                                return newParams;
                                            });
                                        }}
                                    >
                                        {i + 1}
                                    </button>
                                </li>
                            ))}
                        </ul>
                    </nav>
                </div>
            )}
        </div>
    );
};

export default Dashboard;