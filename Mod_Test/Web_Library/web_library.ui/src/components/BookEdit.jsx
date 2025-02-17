import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axios from 'axios';
import API_BASE_URL from '../config';

const BookEdit = () => {
    const [isbn, setIsbn] = useState('');
    const [title, setTitle] = useState('');
    const [genre, setGenre] = useState('');
    const [description, setDescription] = useState('');
    const [quantity, setQuantity] = useState(0);
    const [authorId, setAuthorId] = useState('');
    const [imageUrl, setImageUrl] = useState('');
    const [genres, setGenres] = useState([]); 
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();
    const { id } = useParams();

    const getCurrentUser = () => JSON.parse(localStorage.getItem('currentUser'));
    const currentUser = getCurrentUser();
    const token = currentUser ? currentUser.accessToken : '';

    useEffect(() => {
        const fetchGenres = async () => {
            try {
                const response = await axios.get(`${API_BASE_URL}/books/genres`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setGenres(response.data);
            } catch (err) {
                console.error("Ошибка загрузки жанров", err);
                setError("Ошибка загрузки жанров.");
            }
        };

        const fetchBook = async () => {
            if (id) {
                try {
                    const response = await axios.get(`${API_BASE_URL}/books/${id}`, {
                        headers: { Authorization: `Bearer ${token}` }
                    });
                    const book = response.data;
                    setIsbn(book.isbn);
                    setTitle(book.title);
                    setGenre(book.genreID);  // Используем ID жанра
                    setDescription(book.description);
                    setQuantity(book.quantity);
                    setAuthorId(book.authorID);
                    setImageUrl(book.imageUrl || '');
                } catch (err) {
                    setError("Ошибка загрузки книги.");
                } finally {
                    setLoading(false);
                }
            } else {
                setLoading(false);
            }
        };

        fetchGenres();
        fetchBook();
    }, [id, token]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        const book = { isbn, title, genreID: genre, description, quantity, authorId, imageUrl };

        try {
            if (id) {
                await axios.put(`${API_BASE_URL}/books/${id}`, book, {
                    headers: { Authorization: `Bearer ${token}` }
                });
            } else {
                await axios.post(`${API_BASE_URL}/books`, book, {
                    headers: { Authorization: `Bearer ${token}` }
                });
            }
            navigate('/books');
        } catch (err) {
            setError("Ошибка сохранения книги.");
        }
    };

    if (loading) {
        return <p>Загрузка...</p>;
    }

    return (
        <div>
            <h2>{id ? 'Редактировать книгу' : 'Добавить книгу'}</h2>
            <form onSubmit={handleSubmit}>
                <input type="text" placeholder="ISBN" value={isbn} onChange={(e) => setIsbn(e.target.value)} required />
                <input type="text" placeholder="Название" value={title} onChange={(e) => setTitle(e.target.value)} required />

                {/* Выпадающий список жанров */}
                <select value={genre} onChange={(e) => setGenre(e.target.value)} required>
                    <option value="">Выберите жанр</option>
                    {genres.map((g) => (
                        <option key={g.id} value={g.id}>
                            {g.name}
                        </option>
                    ))}
                </select>

                <textarea placeholder="Описание" value={description} onChange={(e) => setDescription(e.target.value)} />
                <input type="number" placeholder="Количество" value={quantity} onChange={(e) => setQuantity(e.target.value)} required />
                <input type="text" placeholder="ID Автора" value={authorId} onChange={(e) => setAuthorId(e.target.value)} required />
                <input type="text" placeholder="URL изображения" value={imageUrl} onChange={(e) => setImageUrl(e.target.value)} />
                <button type="submit">{id ? 'Обновить' : 'Добавить'}</button>
            </form>
            {error && <p className="error">{error}</p>}
        </div>
    );
};

export default BookEdit;
