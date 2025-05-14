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

    useEffect(() => {
        const loadData = async () => {
            setLoading(true);
            try {
                const result = view === "books" ? await fetchBooks() : await fetchAuthors();
                setData(result.items || result);
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
    }, [view]);

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

            {loading ? (
                <div className="text-center">
                    <div className="spinner-border text-primary" role="status" />
                </div>
            ) : view === "books" ? (
                <BooksView books={data} />
            ) : (
                <AuthorsView authors={data} />
            )}
        </div>
    );
};

export default Dashboard;