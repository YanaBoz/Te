import { useState, useEffect } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import API_BASE_URL from '../config'; // Импортируйте базовый URL

const AuthorList = () => {
    const [authors, setAuthors] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [pageNumber, setPageNumber] = useState(1);
    const [loading, setLoading] = useState(false);
    const [isAdmin, setIsAdmin] = useState(false); // State to track if the user is an admin

    const getCurrentUser = () => JSON.parse(localStorage.getItem('currentUser'));
    const currentUser = getCurrentUser();
    const token = currentUser ? currentUser.accessToken : '';

    const fetchUserRole = async () => {
        if (!token) return;
        try {
            const response = await axios.get(`${API_BASE_URL}/auth/profile`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setIsAdmin(response.data.role === 'Admin'); // Set admin status
        } catch (error) {
            console.error('Error fetching user role:', error);
        }
    };

    const fetchAuthors = async () => {
        setLoading(true);
        try {
            const response = await axios.get(`${API_BASE_URL}/authors`, {
                params: { pageNumber, pageSize: 10 },
            });
            setAuthors(response.data.authors);
            setTotalCount(response.data.totalCount);
        } catch (error) {
            console.error('Error fetching authors:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchUserRole(); // Fetch user role to determine if admin
        fetchAuthors(); // Fetch authors
    }, [pageNumber, token]);

    return (
        <div className="body">
            <h1>Authors List</h1>
            {loading ? <p>Loading...</p> : (
                <>
                    {isAdmin && (
                        <Link to="/authors/add">
                            <button>Add New Author</button>
                        </Link>
                    )}
                    <ul className="book">
                        {authors.map((author) => (
                            <li key={author.id}>
                                <h2>{author.firstName} {author.lastName}</h2>
                                <p>{author.country}</p>
                                <p>Born: {new Date(author.birthDate).toLocaleDateString()}</p>
                                <Link to={`/authors/${author.id}`}>View Details</Link>
                            </li>
                        ))}
                    </ul>
                    <div className="pagin">
                        <button onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))} disabled={pageNumber === 1}>
                            Previous
                        </button>
                        <button onClick={() => setPageNumber((prev) => (prev * 10 < totalCount ? prev + 1 : prev))} disabled={pageNumber * 10 >= totalCount}>
                            Next
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default AuthorList;