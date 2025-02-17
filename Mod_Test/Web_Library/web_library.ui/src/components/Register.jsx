import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import API_BASE_URL from '../config'; // Импортируйте базовый URL

const Register = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [fullName, setFullName] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault();
        if (!username || !password || !fullName) {
            setError('All fields are required.');
            return;
        }

        try {
            await axios.post(`${API_BASE_URL}/auth/register`, {
                username,
                password,
                fullName
            });
            navigate('/login');
        } catch (err) {
            setError('Registration error: ' + (err.response?.data?.message || 'An error occurred.'));
        }
    };

    return (
        <div>
            <h2>Register</h2>
            <form onSubmit={handleRegister}>
                <input type="text" placeholder="Name" onChange={(e) => setUsername(e.target.value)} required />
                <input type="password" placeholder="Password" onChange={(e) => setPassword(e.target.value)} required />
                <input type="text" placeholder="Full Name" onChange={(e) => setFullName(e.target.value)} required />
                <button type="submit">Register</button>
            </form>
            {error && <p>{error}</p>}
        </div>
    );
};

export default Register;