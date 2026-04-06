// src/components/Header/Header.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import styles from './Header.module.css';

const Header = () => {
  return (
    <header className={styles.header}>
      <div className={styles.container}>
        <div className={styles.logo}><span style={{color: "#5c57af"}}>ООО</span> ТМЫВ</div>
        
        <nav className={styles.nav}>
          <Link to="/" className={styles.navLink}>Главная</Link>
          <Link to="/vklady" className={styles.navLink}>Вклады</Link>
          <Link to="/kredity" className={styles.navLink}>Кредиты</Link>
        </nav>
        
        <div className={styles.authButtons}>
          <Link to="/login" style={{ textDecoration: 'none' }}>
            <button className={styles.btnLogin}>Вход</button>
          </Link>
          
          <Link to="/register" style={{ textDecoration: 'none' }}>
            <button className={styles.btnLogin}>Регистрация</button>
          </Link>
        </div>

      </div>
    </header>
  );
};

export default Header;