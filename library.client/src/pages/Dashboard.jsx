import React, { useState, useEffect } from 'react';
import { fetchBooks, fetchAuthors } from '../services/libraryService';
import BooksView from '../components/BooksView';
import AuthorsView from '../components/AuthorsView';
import { jwtDecode } from 'jwt-decode';

const Dashboard = () => {
    const [view, setView] = useState('books'); // 'books' or 'authors'
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const loadData = async () => {
            setLoading(true);
            try {
                const result = view === 'books' ? await fetchBooks() : await fetchAuthors();
                console.log(result); // Log the fetched result to see the structure
                setData(result.items || result); // If there's no 'items', use the entire result as a fallback
            } catch (error) {
                console.error('Failed to fetch data', error);
            } finally {
                setLoading(false);
            }
        };

        const token = localStorage.getItem("accessToken"); // or from cookie
        if (token) {
            const decoded = jwtDecode(token);
            console.log(decoded);
        }

        loadData();
    }, [view]);

    return (
        <div>
            <h1>Dashboard</h1>
            <div>
                <button onClick={() => setView('books')}>Books</button>
                <button onClick={() => setView('authors')}>Authors</button>
            </div>

            {loading ? (
                <p>Loading...</p>
            ) : (
                <>
                    {view === 'books' ? (
                        <BooksView books={data} />
                    ) : (
                        <AuthorsView authors={data} />
                    )}
                </>
            )}
        </div>
    );
};

export default Dashboard;
