import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axios from 'axios';
import API_BASE_URL from '../config';

const AuthorEdit = () => {
    const [author, setAuthor] = useState({ firstName: '', lastName: '', birthDate: '', country: '' });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();
    const { id } = useParams();

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

    const fetchAuthor = async (currentToken, cancelToken) => {
        if (!id) {
            setLoading(false);
            return;
        }
        try {
            const response = await axios.get(`${API_BASE_URL}/authors/${id}`, {
                headers: { Authorization: `Bearer ${currentToken}` },
                cancelToken
            });
            setAuthor(response.data);
        } catch (error) {
            if (axios.isCancel(error)) {
                console.log('Request canceled:', error.message);
            } else if (error.response?.status === 401) {
                const newToken = await refreshAccessToken();
                if (newToken) fetchAuthor(newToken, cancelToken);
                else setError('Session expired. Please log in again.');
            } else {
                setError('Error fetching author data.');
            }
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const cancelSource = axios.CancelToken.source();

        fetchAuthor(token, cancelSource.token);

        return () => {
            cancelSource.cancel('Operation canceled by user.');
        };
    }, [id, token]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        try {
            const config = { headers: { Authorization: `Bearer ${token}` } };
            if (id) {
                await axios.put(`${API_BASE_URL}/authors/${id}`, author, config);
            } else {
                await axios.post(`${API_BASE_URL}/authors`, author, config);
            }
            navigate('/authors');
        } catch (error) {
            if (error.response?.status === 401) {
                const newToken = await refreshAccessToken();
                if (newToken) handleSubmit(e);
                else setError('Session expired. Please log in again.');
            } else {
                setError('Error saving author: ' + (error.response?.data?.message || error.message));
            }
        }
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div>
            <h2>{id ? 'Edit Author' : 'Add Author'}</h2>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="First Name"
                    value={author.firstName}
                    onChange={(e) => setAuthor({ ...author, firstName: e.target.value })}
                    required
                />
                <input
                    type="text"
                    placeholder="Last Name"
                    value={author.lastName}
                    onChange={(e) => setAuthor({ ...author, lastName: e.target.value })}
                    required
                />
                <input
                    type="date"
                    value={author.birthDate}
                    onChange={(e) => setAuthor({ ...author, birthDate: e.target.value })}
                    required
                />
                <input
                    type="text"
                    placeholder="Country"
                    value={author.country}
                    onChange={(e) => setAuthor({ ...author, country: e.target.value })}
                    required
                />
                <button type="submit">{id ? 'Update' : 'Add'}</button>
            </form>
            {error && <p className="error">{error}</p>}
        </div>
    );
};

export default AuthorEdit;
