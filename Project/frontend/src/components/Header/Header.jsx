import React from 'react';
import { Link } from 'react-router-dom';
import styles from './Header.module.css';
import { authService } from '../../services/authService';

const Header = () => {
  const isAuthenticated = authService.isAuthenticated();
  
  const userJson = localStorage.getItem('user');
  let user = null;
  
  if (userJson && userJson !== "undefined") {
    try {
      user = JSON.parse(userJson);
    } catch (e) {
      console.error("Ошибка парсинга:", e);
    }
  }

  return (
    <header className={styles.header}>
      <div className={styles.container}>
        <div className={styles.logo}>
          <span style={{color: "#5c57af"}}>ООО</span> ТМЫВ
        </div>
        
        <nav className={styles.nav}>
          <Link to="/" className={styles.navLink}>Главная</Link>
          <Link to="/vklady" className={styles.navLink}>Вклады</Link>
          <Link to="/kredity" className={styles.navLink}>Кредиты</Link>
        </nav>
        
        <div className={styles.authButtons}>
          {isAuthenticated ? (
            <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
              
              <Link to="/profile">
                <button className={styles.btnLogin}>Личный кабинет</button>
              </Link>

              <button 
                className={styles.btnLogin} 
                onClick={() => { authService.logout(); }}
              >
                Выйти
              </button>

              <span className={styles.userName}>
                {user?.fullName || "Пользователь"}
              </span>
            </div>
          ) : (
            <div style={{ display: 'flex', gap: '10px' }}>
              <Link to="/login" style={{ textDecoration: 'none' }}>
                <button className={styles.btnLogin}>Вход</button>
              </Link>
              <Link to="/register" style={{ textDecoration: 'none' }}>
                <button className={styles.btnLogin}>Регистрация</button>
              </Link>
            </div>
          )}
        </div>
      </div>
    </header>
  );
};

export default Header;