// src/App.jsx
import React from 'react';
import { Routes, Route } from 'react-router-dom'; // Импортируем компоненты роутера
import Header from './components/Header/Header';
import Hero from './components/Hero/Hero';
import Services from './components/Services/Services';
import Footer from './components/Footer/Footer';
import './index.css';

function App() {
  return (
    <div className="App">
      <Header />
      
      <Routes>
        <Route path="/" element={
          <main>
            <Hero />
            <Services />
          </main>
        } />

        {/* Страница Вклады (путь "/vklady") */}
        <Route path="/vklady" element={
          <div style={{ padding: '100px 20px', textAlign: 'center' }}>
            <h1>Страница со вкладами</h1>
            <p>Тут будет список всех вкладов банка.</p>
          </div>
        } />
        
        {/* Страница Кредиты (путь "/kredity") */}
        <Route path="/kredity" element={
          <div style={{ padding: '100px 20px', textAlign: 'center' }}>
            <h1>Страница с кредитами</h1>
            <p>Тут будет кредитный калькулятор.</p>
          </div>
        } />
      </Routes>

      <Footer />
    </div>
  );
}

export default App;