import { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import API_BASE_URL from '../config';

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const source = axios.CancelToken.source();

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const response = await axios.post(`${API_BASE_URL}/auth/token`,
                { username, password },
                { cancelToken: source.token }
            );

            if (response.data && response.data.accessToken) {
                const user = { username, accessToken: response.data.accessToken };
                localStorage.setItem('currentUser', JSON.stringify(user));

                navigate('/');
            } else {
                setError('Authentication failed. Please check your credentials.');
            }
        } catch (err) {
            if (axios.isCancel(err)) {
                console.log('Login request was cancelled');
            } else {
                if (err.response) {
                    setError(`Login failed: ${err.response?.data?.message || 'Unknown error'}`);
                } else {
                    setError('Login failed. Please check your credentials.');
                }
            }
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        return () => {
            source.cancel('Login request cancelled due to component unmounting');
        };
    }, []);

    return (
        <div>
            <h2>Login</h2>
            <form onSubmit={handleLogin}>
                <input
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <button type="submit" disabled={loading}>
                    {loading ? 'Logging in...' : 'Log in'}
                </button>
            </form>
            {error && <p style={{ color: 'red' }}>{error}</p>}
            <p>Dont have an account? <a href="/register">Register here</a></p>
        </div>
    );
};

export default Login;
