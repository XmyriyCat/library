import React, { useEffect, useState } from "react";
import { fetchBooks, fetchAuthors } from "../services/libraryService";
import BooksView from "../components/BooksView";
import AuthorsView from "../components/AuthorsView";
import { useSearchParams } from "react-router-dom";

const Dashboard = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const view = searchParams.get("view") || "books";
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
        loadData();
    }, [view]);

    return (
        <div className="container mt-5">
            <div className="text-center mb-4">
                <h1 className="fw-bold">Library Dashboard</h1>
                <div className="btn-group mt-3">
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
