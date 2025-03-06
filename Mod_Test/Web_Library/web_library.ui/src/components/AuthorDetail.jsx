import { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import API_BASE_URL from '../config';

const AuthorDetail = () => {
    const { id } = useParams();
    const [author, setAuthor] = useState(null);
    const [error, setError] = useState('');
    const [isAdmin, setIsAdmin] = useState(false);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    const getCurrentUser = () => JSON.parse(localStorage.getItem('currentUser'));
    const currentUser = getCurrentUser();
    const token = currentUser ? currentUser.accessToken : '';

    const fetchUserRole = async (cancelToken) => {
        if (!token) return;
        try {
            const response = await axios.get(`${API_BASE_URL}/auth/profile`, {
                headers: { Authorization: `Bearer ${token}` },
                cancelToken
            });
            setIsAdmin(response.data.role === 'Admin');
        } catch (error) {
            if (axios.isCancel(error)) {
                console.log('Request canceled:', error.message);
            } else {
                console.error('Error fetching user role:', error);
                setError('Error fetching user role');
            }
        }
    };

    const fetchAuthor = async (cancelToken) => {
        try {
            const response = await axios.get(`${API_BASE_URL}/authors/${id}`, {
                headers: { Authorization: `Bearer ${token}` },
                cancelToken
            });
            setAuthor(response.data);
        } catch (error) {
            if (axios.isCancel(error)) {
                console.log('Request canceled:', error.message);
            } else {
                setError('Error fetching author data: ' + (error.response?.data?.message || error.message));
            }
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async () => {
        if (!window.confirm('Are you sure you want to delete this author?')) return;

        try {
            await axios.delete(`${API_BASE_URL}/authors/${id}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            alert('Author deleted successfully.');
            navigate('/authors');
        } catch (error) {
            setError('Error deleting author: ' + (error.response?.data?.message || error.message));
        }
    };

    useEffect(() => {
        const cancelSource = axios.CancelToken.source();

        fetchUserRole(cancelSource.token);
        fetchAuthor(cancelSource.token);

        return () => {
            cancelSource.cancel('Operation canceled by user.');
        };
    }, [id, token]);

    if (loading) return <p>Loading...</p>;
    if (error) return <div className="error">{error}</div>;

    return (
        <div className="author-detail">
            {author ? (
                <>
                    <h1>{author.firstName} {author.lastName}</h1>
                    <h3>Date of Birth: {new Date(author.birthDate).toLocaleDateString()}</h3>
                    <p>Country: {author.country}</p>

                    {author.books?.length > 0 ? (
                        <div>
                            <h3>Books by {author.firstName}:</h3>
                            <ul>
                                {author.books.map((book) => (
                                    <li key={book.id}>
                                        <Link to={`/books/${book.id}`}>{book.title}</Link> - {book.genre}
                                    </li>
                                ))}
                            </ul>
                        </div>
                    ) : (
                        <p>No books found for this author.</p>
                    )}

                    {isAdmin && (
                        <div className="admin-actions">
                            <Link to={`/authors/edit/${id}`}>
                                <button>Edit Author</button>
                            </Link>
                            <button onClick={handleDelete} className="delete-button">Delete Author</button>
                        </div>
                    )}
                </>
            ) : (
                <p>No author found.</p>
            )}
        </div>
    );
};

export default AuthorDetail;
