import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { useState, useEffect } from 'react';
import Register from './components/Register';
import Login from './components/Login';
import BookList from './components/BookList';
import BookEdit from './components/BookEdit';
import BookDetail from './components/BookDetail';
import AuthorList from './components/AuthorList';
import AuthorEdit from './components/AuthorEdit';
import AuthorDetail from './components/AuthorDetail';
import MyBooks from './components/MyBooks';
import PrivateRoute from './components/PrivateRoute';
import './App.css';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import axios from 'axios';
import API_BASE_URL from './config';

function App() {
    const [currentUser, setCurrentUser] = useState(null);
    const [overdueBooksCount, setOverdueBooksCount] = useState(0);

    useEffect(() => {
        const user = localStorage.getItem('currentUser');
        if (user) {
            setCurrentUser(JSON.parse(user));
            fetchOverdueBooksCount(JSON.parse(user).accessToken);
        }
    }, []);

    const fetchOverdueBooksCount = async (token) => {
        try {
            const response = await axios.get(`${API_BASE_URL}/books/user-overdue`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setOverdueBooksCount(response.data.length);
        } catch (error) {
            console.error('Error fetching overdue books:', error);
        }
    };

    const handleLogout = () => {
        localStorage.removeItem('currentUser');
        setCurrentUser(null);
        setOverdueBooksCount(0); // —брасываем счетчик при выходе
    };

    return (
        <Router>
            <div className="App">
                <header>
                    <div className="user-info">
                        {currentUser ? (
                            <>
                                <p>Logged in as: {currentUser.username}</p>
                                <button onClick={handleLogout}>Logout</button>
                                {overdueBooksCount > 0 && (
                                    <p style={{ color: 'red' }}>
                                        You have {overdueBooksCount} overdue book(s)!
                                    </p>
                                )}
                            </>
                        ) : (
                            <>
                                <Link to="/login">Log in</Link>
                                <Link to="/register">Register</Link>
                            </>
                        )}
                    </div>
                    <nav>
                        <ToastContainer />
                        <Link to="/books">Books</Link>
                        <Link to="/authors">Authors</Link>
                        <Link to="/user">My Books</Link>
                    </nav>
                </header>
                <main>
                    <Routes>
                        <Route path="/register" element={<Register />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/" element={<Home />} />
                        <Route path="/books" element={<BookList />} />
                        <Route path="/books/:id" element={<BookDetail />} />
                        <Route path="/books/edit/:id" element={<PrivateRoute><BookEdit /></PrivateRoute>} />
                        <Route path="/books/add" element={<PrivateRoute><BookEdit /></PrivateRoute>} />
                        <Route path="/authors" element={<AuthorList />} />
                        <Route path="/authors/add" element={<PrivateRoute><AuthorEdit /></PrivateRoute>} />
                        <Route path="/authors/:id" element={<AuthorDetail />} />
                        <Route path="/authors/edit/:id" element={<PrivateRoute><AuthorEdit /></PrivateRoute>} />
                        <Route path="/user" element={<PrivateRoute><MyBooks /></PrivateRoute>} />
                    </Routes>
                </main>
            </div>
        </Router>
    );
}

const Home = () => {
    return (
        <div className="home">
            <h1>Welcome to the Library Management System!</h1>
            <p>Explore our collection of books and authors.</p>
        </div>
    );
};

export default App;