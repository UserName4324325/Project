// src/components/Footer/Footer.jsx
import React from 'react';
import styles from './Footer.module.css';

const Footer = () => {
  return (
    <footer className={styles.footer}>
      <div className={styles.container}>
        <p className={styles.copyright}>
          © 2026 ООО ТМЫВ. Все права защищены.
        </p>
        <div className={styles.contacts}>
          <span>Контакты: </span>
          <a href="mailto:support@OOOTMYV.ru" className={styles.link}>
            support@otmyv.ru
          </a>
          <span className={styles.divider}>|</span>
          <a href="tel:+78001234567" className={styles.link}>
            +7 (800) 123-45-67
          </a>
        </div>
      </div>
    </footer>
  );
};

export default Footer;