import React, { useState, useEffect } from "react";
import { fetchBooks, fetchAuthors } from "../services/libraryService";
import BooksView from "../components/BooksView";
import AuthorsView from "../components/AuthorsView";
import { jwtDecode } from "jwt-decode";
import { Link } from "react-router-dom"; // Import Link for routing

const Dashboard = () => {
    const [view, setView] = useState("books");
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);

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
        };

        const token = localStorage.getItem("accessToken");
        if (token) {
            const decoded = jwtDecode(token);
            console.log(decoded);
        }

        loadData();
    }, [view]);

    return (
        <div>
            <div className="container mt-5">
                <div className="text-center mb-4">
                    <h1 className="fw-bold">Library Dashboard</h1>
                    <div className="btn-group mt-3">
                        <button
                            className={`btn btn-outline-primary ${view === "books" ? "active" : ""}`}
                            onClick={() => setView("books")}
                        >
                            Books
                        </button>
                        <button
                            className={`btn btn-outline-primary ${view === "authors" ? "active" : ""}`}
                            onClick={() => setView("authors")}
                        >
                            Authors
                        </button>
                    </div>
                </div>

                {loading ? (
                    <div className="text-center">
                        <div className="spinner-border text-primary" role="status" />
                    </div>
                ) : (
                    <div>
                        {view === "books" ? <BooksView books={data} /> : <AuthorsView authors={data} />}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Dashboard;
