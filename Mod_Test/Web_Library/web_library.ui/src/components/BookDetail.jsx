import { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import API_BASE_URL from '../config';

const BookDetail = () => {
    const { id } = useParams();
    const [book, setBook] = useState(null);
    const [error, setError] = useState('');
    const [isAdmin, setIsAdmin] = useState(false);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

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

    const fetchBook = async () => {
        try {
            const response = await axios.get(`${API_BASE_URL}/books/${id}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setBook(response.data);
        } catch (error) {
            await handleFetchError(error);
        } finally {
            setLoading(false);
        }
    };

    const handleFetchError = async (error) => {
        if (error.response?.status === 401) {
            const newToken = await refreshAccessToken();
            if (newToken) {
                await fetchBookWithNewToken(newToken);
            } else {
                setError('Unable to refresh token. Please log in again.');
            }
        } else {
            setError('Error fetching book data');
        }
    };

    const fetchBookWithNewToken = async (newToken) => {
        try {
            const response = await axios.get(`${API_BASE_URL}/books/${id}`, {
                headers: { Authorization: `Bearer ${newToken}` }
            });
            setBook(response.data);
        } catch (error) {
            setError('Error fetching book data with new token');
        }
    };

    const handleBorrow = async () => {
        if (book.quantity > 0) {
            try {
                await axios.post(`${API_BASE_URL}/books/issue/${id}`, {}, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                alert('You have successfully borrowed the book!');
                fetchBook(); 
            } catch (error) {
                setError('Error borrowing the book');
            }
        } else {
            alert('This book is not available for borrowing.');
        }
    };

    const handleDelete = async () => {
        const confirmDelete = window.confirm('Are you sure you want to delete this book?');
        if (!confirmDelete) return;

        try {
            await axios.delete(`${API_BASE_URL}/books/${id}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            alert('Book deleted successfully.');
            navigate('/books');
        } catch (error) {
            await handleDeleteError(error);
        }
    };

    const handleDeleteError = async (error) => {
        if (error.response?.status === 401) {
            const newToken = await refreshAccessToken();
            if (newToken) {
                await handleDeleteWithNewToken(newToken);
            } else {
                setError('Unable to refresh token. Please log in again.');
            }
        } else {
            setError('Error deleting book: ' + (error.response?.data?.message || error.message));
        }
    };

    const handleDeleteWithNewToken = async (newToken) => {
        try {
            await axios.delete(`${API_BASE_URL}/books/${id}`, {
                headers: { Authorization: `Bearer ${newToken}` }
            });
            alert('Book deleted successfully.');
            navigate('/books');
        } catch (error) {
            setError('Error deleting book with new token');
        }
    };

    useEffect(() => {
        fetchUserRole();
        fetchBook(); 
    }, [id, token]);

    if (loading) return <p>Loading...</p>;
    if (error) return <div>{error}</div>;

    return (
        <div>
            {book ? (
                <>
                    <h1>{book.title}</h1>
                    <p>Author: {book.author}</p>
                    <p>Genre: {book.genre}</p>
                    <p>Available Copies: {book.quantity}</p>
                    {book.quantity > 0 && (
                        <button onClick={handleBorrow}>Borrow Book</button>
                    )}
                    {isAdmin && (
                        <>
                            <Link to={`/books/edit/${id}`}>Edit Book</Link>
                            <button onClick={handleDelete}>Delete Book</button>
                        </>
                    )}
                </>
            ) : (
                <p>No book found.</p>
            )}
        </div>
    );
};

export default BookDetail;
