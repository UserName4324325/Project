// src/components/Hero/Hero.jsx
import React from 'react';
import { Link } from 'react-router-dom';
import styles from './Hero.module.css';

const Hero = () => {
  return (
    <section className={styles.hero}>
      <div className={styles.container}>
        <h1 className={styles.title}>
          Умный банк для <br />
          <span className={styles.highlight}>ваших целей</span>
        </h1>
        <p className={styles.subtitle}>
          Откройте вклад или получите кредит за 10 минут
        </p>
        
        <div className={styles.actions}>
          <Link to="/vklady" style={{ textDecoration: 'none' }}>
            <button className={styles.btnSecondary}>Открыть вклад</button>
          </Link>

          <Link to="/kredity" style={{ textDecoration: 'none' }}>
            <button className={styles.btnSecondary}>Взять кредит</button>
          </Link>
        </div>
      </div>
    </section>
  );
};

export default Hero;