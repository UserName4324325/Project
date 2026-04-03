// src/components/Services/Services.jsx
import React from 'react';
import styles from './Services.module.css';
import { Link } from 'react-router-dom';

// Данные для карточек (для удобства)
const servicesData = [
  {
    icon: '↗', // Упрощенный символ стрелки
    title: 'Вклады',
    desc: 'До 8% годовых',
    buttonText: 'Подробнее',
    ink: 'vklady',
  },
  {
    icon: '$', // Упрощенный символ доллара
    title: 'Кредиты',
    desc: 'От 5.9%',
    buttonText: 'Рассчитать',
    ink: 'kredity',
  },
  {
    icon: '👤', // Упрощенный символ пользователя
    title: 'Кабинет',
    desc: 'Все счета онлайн',
    buttonText: 'Войти',
    link: '',
  },
];

const Services = () => {
  return (
    <section className={styles.services}>
      <div className={styles.container}>
        {servicesData.map((service, index) => (
          <div key={index} className={styles.card}>
            <div className={styles.iconWrapper}>
              <span className={styles.icon}>{service.icon}</span>
            </div>
            <h3 className={styles.cardTitle}>{service.title}</h3>
            <p className={styles.cardDesc}>{service.desc}</p>
            <button className={styles.cardButton}>{service.buttonText}</button>
          </div>
        ))}
      </div>
    </section>
  );
};

export default Services;