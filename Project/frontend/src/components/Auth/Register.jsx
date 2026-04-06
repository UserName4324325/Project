// src/components/Auth/Register.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import styles from './Auth.module.css';

const Register = () => {
  return (
    <div className={styles.wrapper}>
      <div className={styles.card}>
        <h1 className={styles.title}>Создать аккаунт</h1>
        <p className={styles.subtitle}>Регистрация в ООО ТМЫВ</p>
        
        <form className={styles.form} onSubmit={(e) => e.preventDefault()}>
          <div className={styles.formGroup}>
            <label className={styles.label}>Имя</label>
            <input type="text" placeholder="Иван Петров" className={styles.input} />
          </div>
          
        <div className={styles.formGroup}>
            <label className={styles.label}>Email</label>
            <input type="email" placeholder="your@email.com" className={styles.input} />
          </div>

          <div className={styles.formGroup}>
            <label className={styles.label}>Пароль</label>
            <input type="password" placeholder="••••••" className={styles.input} />
          </div>

          <div className={styles.formGroup}>
            <label className={styles.label}>Подтвердите пароль</label>
            <input type="password" placeholder="••••••" className={styles.input} />
          </div>
          
          <button type="submit" className={styles.btnPrimary}>Создать аккаунт</button>
        </form>
        
        <p className={styles.footerText}>
          Уже есть аккаунт? <Link to="/login" className={styles.link}>Войти</Link>
        </p>
      </div>
    </div>
  );
};

export default Register;