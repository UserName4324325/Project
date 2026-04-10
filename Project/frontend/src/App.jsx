import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Header from './components/Header/Header';
import Hero from './components/Hero/Hero';
import Services from './components/Services/Services';
import Footer from './components/Footer/Footer';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './components/Auth/Login'; 
import Register from './components/Auth/Register'; 
import Deposits from './components/Deposits/Deposits';
import Loans from './components/Loans/Loans';
import Profile from './components/Profile/Profile';
import './index.css';

function App() {
  return (
    <div className="App">
      <Header />
      
      <main>
        <Routes>
          <Route path="/" element={
            <>
              <Hero />
              <Services />
            </>
          } />
        
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          
          <Route 
            path="/vklady" 
            element={
              <ProtectedRoute>
                <Deposits />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/kredity" 
            element={
              <ProtectedRoute>
                <Loans />
              </ProtectedRoute>
            } 
          />
          <Route 
            path="/profile" 
            element={
              <ProtectedRoute>
                <Profile />
              </ProtectedRoute>
            } 
          />

        </Routes>
      </main>

      <Footer />
    </div>
  );
}

export default App;