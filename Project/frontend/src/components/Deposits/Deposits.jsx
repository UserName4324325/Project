import React, { useState, useEffect } from 'react';
import styles from './Deposits.module.css'; 
import { depositService } from '../../services/depositService';

const Deposits = () => {
  const [amount, setAmount] = useState(1000);
  const [seconds, setSeconds] = useState(12);
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(false); 

  const rate = 4;
  const calculateProfit = () => {
    const profit = Number(amount) * (rate / 100) * (seconds / 12);
    return isNaN(profit) || profit < 0 ? "0.00" : profit.toFixed(2);
  };
  
  const total = Number(amount) + parseFloat(calculateProfit());

  useEffect(() => {
    const userData = localStorage.getItem('user');
    if (userData && userData !== "undefined") {
      try {
        const parsedUser = JSON.parse(userData);
        setUser(parsedUser);
      } catch (e) {
        console.error("Ошибка парсинга данных пользователя", e);
      }
    }
  }, []);

  const handleOpenDeposit = async () => {

    if (!user) {
      alert("Пожалуйста, войдите в систему, чтобы открыть вклад!");
      return;
    }

    const depositAmount = parseFloat(amount);


    if (isNaN(depositAmount) || depositAmount <= 0) {
      alert("Введите корректную сумму вклада.");
      return;
    }


    if (user.balance < depositAmount) {
      alert(`Недостаточно средств. Ваш баланс: ${user.balance.toLocaleString('ru-RU')} ₽`);
      return;
    }


    setIsLoading(true);

    try {

      const userId = user.id || user.Id;

      const depositData = {
        UserId: userId,
        Amount: depositAmount,
        TermInSeconds: seconds,
        InterestRate: rate
      };

      console.log("Отправка вклада:", depositData);


      await depositService.openDeposit(depositData);
      
      const updatedUser = {
        ...user,
        balance: user.balance - depositAmount
      };
      localStorage.setItem('user', JSON.stringify(updatedUser));

      alert(`Вклад на сумму ${depositAmount} ₽ успешно открыт! Деньги вернутся через ${seconds} секунд.`);
      window.location.href = '/profile'; 

    } catch (error) {
      if (error.message.includes('replace')) {
          alert("Ошибка авторизации. Попробуйте перезайти в аккаунт.");
      } else {
          alert("Ошибка при открытии вклада: " + error.message);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Вклады</h1>

      <div className={styles.card}>
        <div className={styles.row}>
          <div className={styles.inputGroup}>
            <label className={styles.label}>Сумма вклада</label>
            <input 
              className={styles.input} 
              type="number"
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              placeholder="1000"
              min="1"
            />
          </div>

          <div className={styles.inputGroup}>
            <label className={styles.label}>Срок размещения</label>
            <select 
              className={styles.select}
              value={seconds}
              onChange={(e) => setSeconds(Number(e.target.value))}
            >
              <option value={12}>12 секунд</option>
              <option value={24}>24 секунд</option>
              <option value={36}>36 секунд</option>
              <option value={48}>48 секунд</option>
              <option value={60}>60 секунд</option>
            </select>
          </div>
        </div>

        <div className={styles.resultBox}>
          <div className={styles.resultInfo}>
            <span className={styles.resultLabel}>{rate}% годовых</span>
            <span className={styles.resultValue}>+ {calculateProfit()} ₽</span>
          </div>
          <div className={styles.resultInfo} style={{textAlign: 'right'}}>
            <span className={styles.resultLabel}>Итого:</span>
            <span className={styles.resultValue} style={{color: '#5c57af'}}>
              {total.toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} ₽
            </span>
          </div>
        </div>

        <button 
          className={styles.submitBtn}
          onClick={handleOpenDeposit}
          disabled={isLoading}
        >
          {isLoading ? "Обработка..." : "Открыть вклад прямо сейчас"}
        </button>

        <p className={styles.testNotice}>
          * 1 месяц вклада приравнен к 1 секунде реального времени.
        </p>
      </div>
    </div>
  );
};

export default Deposits;