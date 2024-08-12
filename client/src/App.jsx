import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import UrlShortener from './components/UrlShortener';
import UrlList from './components/UrlList';
import axios from 'axios';
import 'react-toastify/dist/ReactToastify.css';
import './App.css';

const api = 'https://localhost:2031';

const App = () => {
    const handleShorten = async (originalUrl, customShortUrl) => {
        await axios.post(`${api}/shorten`, {
            originalUrl,
            customShortUrl
        });
    };

    return (
        <Router>
            <Routes>
                <Route path="/" element={<UrlShortener onShorten={handleShorten} />} />
                <Route path="/list" element={<UrlList />} />
            </Routes>
        </Router>
    );
};

export default App;
