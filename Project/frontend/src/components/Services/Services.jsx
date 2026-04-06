import React from 'react';
import styles from './Services.module.css';
import { Link } from 'react-router-dom';

const servicesData = [
  {
    icon: '↗',
    title: 'Вклады',
    desc: 'До 8% годовых',
    buttonText: 'Подробнее',
    path: '/vklady',
  },
  {
    icon: '$',
    title: 'Кредиты',
    desc: 'От 5.9%',
    buttonText: 'Рассчитать',
    path: '/kredity', 
  },
  {
    icon: '👤',
    title: 'Кабинет',
    desc: 'Все счета онлайн',
    buttonText: 'Войти',
    path: '/login', 
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

            <Link to={service.path} style={{ textDecoration: 'none', width: '100%' }}>
              <button className={styles.cardButton}>{service.buttonText}</button>
            </Link>

          </div>
        ))}
      </div>
    </section>
  );
};

export default Services;