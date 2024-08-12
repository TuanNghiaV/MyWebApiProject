import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Link } from 'react-router-dom';
import axios from 'axios';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import backgroundImage from '../assets/cool_background.jpg';

const api = 'https://localhost:2031';

const UrlShortener = () => {
    const [originalUrl, setOriginalUrl] = useState('');
    const [customShortUrl, setCustomShortUrl] = useState('');
    const [error, setError] = useState('');

    const handleShorten = async () => {
        if (!originalUrl) {
            setError('Please enter a valid URL');
            return;
        }
        try {
            const response = await axios.post(`${api}/shorten`, { originalUrl, customShortUrl });
            setOriginalUrl('');
            setCustomShortUrl('');
            setError('');
            toast.success(`URL shortened successfully! Short URL: ${response.data.shortUrl}`);
        } catch (err) {
            if (err.response && err.response.data && err.response.data.message) {
                setError(err.response.data.message);
                toast.error(err.response.data.message);
            } else {
                setError('An unexpected error occurred');
                toast.error('An unexpected error occurred');
            }
        }
    };

    return (
        <div
            className="d-flex justify-content-center align-items-center min-vh-100"
            style={{
                backgroundImage: `url(${backgroundImage})`,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
                fontFamily: "'Poppins', sans-serif",
            }}
        >
            <div className="container text-center">
                <nav className="navbar navbar-expand-lg navbar-light mb-4" style={{ backgroundColor: '#D1E9F6' }}>
                    <div className="container-fluid">
                        <Link className="navbar-brand" to="/">URL Shortener</Link>
                        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="collapse navbar-collapse" id="navbarNav">
                            <ul className="navbar-nav">
                                <li className="nav-item">
                                    <Link className="nav-link" to="/">Home</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/list">List</Link>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>

                {error && <div className="alert alert-danger">{`Error: ${error}`}</div>}
                <div className="d-flex justify-content-center">
                    <div className="card" style={{ width: '100%', maxWidth: '500px' }}>
                        <div className="card-body">
                            <div className="form-group">
                                <h1 className="mb-4">Shorten a URL</h1>
                                <br></br> <br></br>
                                <input
                                    type="text"
                                    className="form-control mb-3 custom-form-control"
                                    placeholder="Enter original URL"
                                    value={originalUrl}
                                    onChange={(e) => setOriginalUrl(e.target.value)}
                                />
                                <br/>
                                <input
                                    type="text"
                                    className="form-control mb-3 custom-form-control"
                                    placeholder="Enter short URL (optional)"
                                    value={customShortUrl}
                                    onChange={(e) => setCustomShortUrl(e.target.value)}
                                />
                                <br/>
                                <button onClick={handleShorten} className="btn btn-primary btn-block">Shorten URL</button>
                            </div>
                        </div>
                    </div>
                </div>
                <ToastContainer />
            </div>
        </div>
    );
};

export default UrlShortener;
