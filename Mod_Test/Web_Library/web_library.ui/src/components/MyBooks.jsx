import { useEffect, useState } from 'react';
import axios from 'axios';
import API_BASE_URL from '../config';

const MyBooks = () => {
    const [myBooks, setMyBooks] = useState([]);
    const [overdueBooks, setOverdueBooks] = useState([]);
    const [allOverdueBooks, setAllOverdueBooks] = useState([]); 
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isAdmin, setIsAdmin] = useState(false); 

    const getCurrentUser = () => JSON.parse(localStorage.getItem('currentUser'));
    const currentUser = getCurrentUser();
    const token = currentUser ? currentUser.accessToken : '';

    const refreshAccessToken = async () => {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) return null;

        try {
            const response = await axios.post(`${API_BASE_URL}/auth/refresh`, { refreshToken });
            const { accessToken } = response.data;
            localStorage.setItem('currentUser', JSON.stringify({ ...currentUser, accessToken }));
            return accessToken;
        } catch (error) {
            console.error('Error refreshing token:', error);
            return null;
        }
    };

    const fetchBooks = async (endpoint, setter) => {
        try {
            const response = await axios.get(endpoint, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setter(response.data);
        } catch (error) {
            console.error('Error fetching books from endpoint:', endpoint, error);
            if (error.response?.status === 401) {
                const newToken = await refreshAccessToken();
                if (newToken) {
                    const response = await axios.get(endpoint, {
                        headers: { Authorization: `Bearer ${newToken}` }
                    });
                    setter(response.data);
                } else {
                    setError('Unable to refresh token. Please log in again.');
                }
            } else if (error.response?.status !== 404) {
                setError('Unable to load your books. Please try again.');
            }
        }
    };

    const fetchUserRole = async () => {
        if (!token) return;
        try {
            const response = await axios.get(`${API_BASE_URL}/auth/profile`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setIsAdmin(response.data.role === 'Admin');
        } catch (error) {
            console.error('Error fetching user role:', error);
            setError('Error fetching user role');
        }
    };

    const fetchAllOverdueBooks = async () => {
        try {
            const response = await axios.get(`${API_BASE_URL}/Books/overdue`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setAllOverdueBooks(response.data); 
        } catch (error) {
            if (error.response?.status === 404) {
                setAllOverdueBooks([]);
                setError('No overdue books available.');
            } else {
                setError('Error fetching overdue books.');
            }
        }
    };


    useEffect(() => {
        fetchUserRole(); 
        const loadBooks = async () => {
            try {
                await Promise.all([
                    fetchBooks(`${API_BASE_URL}/auth/borrowed-books`, setMyBooks),
                    fetchBooks(`${API_BASE_URL}/books/user-overdue`, setOverdueBooks)
                ]);
            } catch (error) {
                setError('Failed to load books');
            } finally {
                setLoading(false);
            }
        };
        loadBooks();
    }, [token]);

    if (loading) {
        return <p>Loading...</p>;
    }

    return (
        <div className="body">
            <h1>My Books</h1>
            {error && <p style={{ color: 'red' }}>{error}</p>}

            <h2>Borrowed Books</h2>
            <ul className="book">
                {myBooks.length > 0 ? (
                    myBooks.map((book) => (
                        <li key={book.id}>
                            <h3>{book.title}</h3>
                            <p>Author: {book.author}</p>
                            <p>Genre: {book.genre}</p>
                            <p>Description: {book.description}</p>
                        </li>
                    ))
                ) : (
                    <p>You currently have no borrowed books.</p>
                )}
            </ul>

            <h2>Overdue Books</h2>
            <ul className="book">
                {overdueBooks.length > 0 ? (
                    overdueBooks.map((book) => (
                        <li key={book.id}>
                            <h3>{book.title}</h3>
                            <p>Author: {book.author}</p>
                            <p>Genre: {book.genre}</p>
                            <p>Description: {book.description}</p>
                        </li>
                    ))
                ) : (
                    <p>You currently have no overdue books.</p>
                )}
            </ul>

            {isAdmin && (
                <div>
                    <button onClick={fetchAllOverdueBooks}>Show All Overdue Books</button>
                    {allOverdueBooks.length > 0 && (
                        <div>
                            <h3>All Overdue Books</h3>
                            <ul className="book">
                                {allOverdueBooks.map((book) => (
                                    <li key={book.id}>
                                        <h3>{book.title}</h3>
                                        <p>Author: {book.author}</p>
                                        <p>Genre: {book.genre}</p>
                                        <p>Description: {book.description}</p>
                                    </li>
                                ))}
                            </ul>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default MyBooks;
