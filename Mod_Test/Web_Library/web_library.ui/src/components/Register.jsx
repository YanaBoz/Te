import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import API_BASE_URL from '../config';

const Register = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [fullName, setFullName] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const validatePassword = (password) => {
        const minLength = 6;
        return password.length >= minLength;
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        if (!username || !password || !fullName) {
            setError('All fields are required.');
            return;
        }

        if (!validatePassword(password)) {
            setError('Password must be at least 6 characters long.');
            return;
        }

        setLoading(true);

        const source = axios.CancelToken.source();

        try {
            await axios.post(`${API_BASE_URL}/auth/register`, {
                username,
                password,
                fullName
            }, { cancelToken: source.token });
            navigate('/login');
        } catch (err) {
            setError('Registration error: ' + (err.response?.data?.message || 'An error occurred.'));
        } finally {
            setLoading(false);
        }

        return () => {
            source.cancel('Registration request cancelled.');
        };
    };

    return (
        <div className="register-container">
            <h2>Register</h2>
            <form onSubmit={handleRegister}>
                <input
                    type="text"
                    placeholder="Username"
                    onChange={(e) => setUsername(e.target.value)}
                    value={username}
                    required
                />
                <input
                    type="password"
                    placeholder="Password"
                    onChange={(e) => setPassword(e.target.value)}
                    value={password}
                    required
                />
                <input
                    type="text"
                    placeholder="Full Name"
                    onChange={(e) => setFullName(e.target.value)}
                    value={fullName}
                    required
                />
                <button type="submit" disabled={loading}>
                    {loading ? 'Registering...' : 'Register'}
                </button>
            </form>
            {error && <p className="error-message">{error}</p>}
        </div>
    );
};

export default Register;
