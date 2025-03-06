import { Navigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import axios from 'axios';
import API_BASE_URL from '../config';

const PrivateRoute = ({ children }) => {
    const [isLoading, setIsLoading] = useState(true);
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    const source = axios.CancelToken.source();

    useEffect(() => {
        const checkAuth = async () => {
            const currentUser = JSON.parse(localStorage.getItem('currentUser'));
            if (currentUser && currentUser.accessToken) {
                setIsAuthenticated(true);
            } else {
                const refreshToken = localStorage.getItem('refreshToken');
                if (refreshToken) {
                    try {
                        const response = await axios.post(`${API_BASE_URL}/auth/refresh`, { refreshToken }, { cancelToken: source.token });
                        const { accessToken } = response.data;

                        localStorage.setItem('currentUser', JSON.stringify({
                            ...currentUser,
                            accessToken
                        }));
                        setIsAuthenticated(true);
                    } catch (error) {
                        console.error('Error refreshing token:', error);
                        setIsAuthenticated(false);
                    }
                }
            }
            setIsLoading(false);
        };

        checkAuth();
        return () => {
            source.cancel('PrivateRoute component unmounted, canceling request');
        };
    }, []);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return isAuthenticated ? children : <Navigate to="/login" />;
};

export default PrivateRoute;
