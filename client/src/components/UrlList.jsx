import React, { useState, useEffect } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import backgroundImage from '../assets/cool_background.jpg';

const api = 'https://localhost:2031';

const UrlList = () => {
    const [urls, setUrls] = useState([]);
    const [error, setError] = useState('');

    useEffect(() => {
        fetchUrls();
    }, []);

    const fetchUrls = async () => {
        try {
            const { data } = await axios.get(`${api}/all`);
            setUrls(data);
        } catch (err) {
            setError('Error fetching URLs');
            toast.error('Error fetching URLs');
        }
    };

    const deleteUrl = async (shortUrl) => {
        const confirmed = window.confirm("Are you sure you want to delete this URL?");
        if (!confirmed) return;

        try {
            await axios.delete(`${api}/${shortUrl}`);
            fetchUrls();
        } catch (err) {
            setError(err.message);
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
            <div className="container">
                <nav className="navbar navbar-expand-lg navbar-light mb-4" style={{ backgroundColor: '#D1E9F6' }}>
                    <div className="container-fluid">
                        <a className="navbar-brand" href="#">URL Shortener</a>
                        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="collapse navbar-collapse" id="navbarNav">
                            <ul className="navbar-nav">
                                <li className="nav-item">
                                    <a className="nav-link" href="/">Home</a>
                                </li>
                                <li className="nav-item">
                                    <a className="nav-link" href="/list">List</a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>

                {error && <div className="alert alert-danger text-center">{`Error: ${error}`}</div>}
                <div className="d-flex justify-content-center">
                    <div className="card" style={{ width: '100%', maxWidth: '800px' }}>
                        <div className="card-body">
                            <h2 className="text-center mb-4">All URLs</h2>
                            <table className="table table-striped">
                                <thead className="bg-light sticky-top">
                                    <tr>
                                        <th className="text-center">Original URL</th>
                                        <th className="text-center">Short URL</th>
                                        <th className="text-center">Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {urls.map((url) => (
                                        <tr key={url.shortUrl}>
                                            <td className="text-center">
                                                <a href={url.originalUrl} target="_blank" rel="noopener noreferrer">
                                                    {url.originalUrl}
                                                </a>
                                            </td>
                                            <td className="text-center">
                                                <a href={`${api}/${url.shortUrl}`} target="_blank" rel="noopener noreferrer">
                                                    {`${api}/${url.shortUrl}`}
                                                </a>
                                            </td>
                                            <td className="text-center">
                                                <button onClick={() => deleteUrl(url.shortUrl)} className="btn btn-danger">Delete</button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <ToastContainer />
            </div>
        </div>
    );
};

export default UrlList;
