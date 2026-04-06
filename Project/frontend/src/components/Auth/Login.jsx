// src/components/Auth/Login.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import styles from './Auth.module.css';

const Login = () => {
  return (
    <div className={styles.wrapper}>
      <div className={styles.card}>
        <h1 className={styles.title}>Вход</h1>
        <p className={styles.subtitle}>Войдите в ваш аккаунт</p>
        
        <form className={styles.form} onSubmit={(e) => e.preventDefault()}>
          <div className={styles.formGroup}>
            <label className={styles.label}>Email</label>
            <input type="email" placeholder="your@email.com" className={styles.input} />
          </div>
          
          <div className={styles.formGroup}>
            <label className={styles.label}>Пароль</label>
            <input type="password" placeholder="••••••" className={styles.input} />
          </div>
          
          <button type="submit" className={styles.btnPrimary}>Войти</button>
        </form>
        
        <p className={styles.footerText}>
          Нет аккаунта? <Link to="/register" className={styles.link}>Зарегистрироваться</Link>
        </p>
      </div>
    </div>
  );
};

export default Login;