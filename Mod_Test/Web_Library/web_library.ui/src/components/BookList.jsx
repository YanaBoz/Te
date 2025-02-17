import { useState, useEffect } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import API_BASE_URL from '../config';

const BookList = () => {
    const [books, setBooks] = useState([]);
    const [filteredBooks, setFilteredBooks] = useState([]);
    const [displayedBooks, setDisplayedBooks] = useState([]);
    const [loading, setLoading] = useState(false);
    const [isAdmin, setIsAdmin] = useState(false);

    const [genres, setGenres] = useState([]); // Добавляем жанры
    const [searchQuery, setSearchQuery] = useState('');
    const [genreFilter, setGenreFilter] = useState('');
    const [authorFilter, setAuthorFilter] = useState('');

    const [pageNumber, setPageNumber] = useState(1);
    const pageSize = 10;

    const getCurrentUser = () => JSON.parse(localStorage.getItem('currentUser'));
    const currentUser = getCurrentUser();
    const token = currentUser ? currentUser.accessToken : '';

    useEffect(() => {
        fetchUserRole();
        fetchBooks();
        fetchGenres(); // Загружаем жанры при монтировании
    }, []);

    const fetchUserRole = async () => {
        if (!token) return;
        try {
            const response = await axios.get(`${API_BASE_URL}/auth/profile`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setIsAdmin(response.data.role === 'Admin');
        } catch (error) {
            console.error('Error fetching user role:', error);
        }
    };

    const fetchBooks = async () => {
        setLoading(true);
        try {
            const response = await axios.get(`${API_BASE_URL}/books`);
            setBooks(response.data.books);
            setFilteredBooks(response.data.books);
        } catch (error) {
            console.error('Error fetching books:', error);
        } finally {
            setLoading(false);
        }
    };

    const fetchGenres = async () => {
        try {
            const response = await axios.get(`${API_BASE_URL}/books/genres`);
            setGenres(response.data); // Сохраняем жанры в состояние
        } catch (error) {
            console.error('Error fetching genres:', error);
        }
    };

    // Фильтрация книг
    useEffect(() => {
        let filtered = books;

        if (searchQuery) {
            filtered = filtered.filter((book) =>
                book.title.toLowerCase().includes(searchQuery.toLowerCase())
            );
        }

        if (genreFilter) {
            filtered = filtered.filter((book) => book.genreName === genreFilter);
        }

        if (authorFilter) {
            filtered = filtered.filter((book) =>
                book.authorName?.toLowerCase().includes(authorFilter.toLowerCase())
            );
        }

        setFilteredBooks(filtered);
        setPageNumber(1);
    }, [searchQuery, genreFilter, authorFilter, books]);

    useEffect(() => {
        const startIndex = (pageNumber - 1) * pageSize;
        const endIndex = startIndex + pageSize;
        setDisplayedBooks(filteredBooks.slice(startIndex, endIndex));
    }, [filteredBooks, pageNumber]);

    return (
        <div className="body">
            <h1>Books List</h1>

            {isAdmin && (
                <Link to="/books/add">
                    <button>Add New Book</button>
                </Link>
            )}

            {/* Фильтры */}
            <div>
                <input
                    type="text"
                    placeholder="Search by title..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                />

                <select onChange={(e) => setGenreFilter(e.target.value)}>
                    <option value="">All Genres</option>
                    {genres.map((genre) => (
                        <option key={genre.id} value={genre.name}>
                            {genre.name}
                        </option>
                    ))}
                </select>

                <input
                    type="text"
                    placeholder="Filter by author"
                    value={authorFilter}
                    onChange={(e) => setAuthorFilter(e.target.value)}
                />
            </div>

            {/* Список книг */}
            {loading ? (
                <p>Loading...</p>
            ) : (
                <>
                    <ul className="book">
                        {displayedBooks.map((book) => (
                            <li key={book.id}>
                                <h2>{book.title}</h2>
                                <p>Genre: {book.genreName}</p>
                                <p>Author: {book.authorName}</p>
                                <p>Quantity: {book.quantity}</p>
                                <Link to={`/books/${book.id}`}>View Details</Link>
                            </li>
                        ))}
                    </ul>

                    {/* Пагинация */}
                    <div>
                        <button
                            onClick={() => setPageNumber((prev) => Math.max(prev - 1, 1))}
                            disabled={pageNumber === 1}
                        >
                            Previous
                        </button>
                        <button
                            onClick={() =>
                                setPageNumber((prev) =>
                                    prev * pageSize < filteredBooks.length ? prev + 1 : prev
                                )
                            }
                            disabled={pageNumber * pageSize >= filteredBooks.length}
                        >
                            Next
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default BookList;
