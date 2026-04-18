import React, { useState, useEffect, use } from 'react';
import styles from './Deposits.module.css'; 
import { depositService } from '../../services/depositService';

const Deposits = () => {
  const [amount, setAmount] = useState('');
  const [seconds, setSeconds] = useState(12);
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(false); 

  const rate = 4;

  const formatDisplay = (val) => {
    if (!val) return "";
    let parts = val.toString().replace(/ /g, "").replace(",", ".").split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, " "); 
    if (parts[1]) parts[1] = parts[1].substring(0, 2);
    return parts.join(",");
  };

  const getCleanNumber = (val) => {
    return parseFloat(val.toString().replace(/ /g, "").replace(",", ".")) || 0;
  };

  const calculateProfit = () => {
    const numAmount = getCleanNumber(amount);
    const profit = numAmount * (rate / 100) * (seconds / 12);
    return isNaN(profit) || profit < 0 ? 0 : profit;
  };
  
  const total = getCleanNumber(amount) + calculateProfit();

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

  const handleAmountChange = (e) => {
    let inputVal = e.target.value;

    inputVal = inputVal.replace(/[^0-9., ]/g, "");


    if (inputVal.startsWith('0') && inputVal.length > 1) {
      if (inputVal[1] !== ',' && inputVal[1] !== '.') {
        inputVal = inputVal.substring(1);
      }
    }

    let numericValue = getCleanNumber(inputVal);

    if (user.balance < 0){
      numericValue = 0;
      setAmount(formatDisplay(numericValue.toFixed(2)));
      return;
    }

    if (user && numericValue > user.balance) {
      numericValue = user.balance;
      setAmount(formatDisplay(numericValue.toFixed(2)));
      return;
    }

    if (inputVal.includes('.') || inputVal.includes(',')) {
     const parts = inputVal.replace(",", ".").split(".");
      if (parts[1] && parts[1].length > 2) {
        return; 
      }
    }

    setAmount(formatDisplay(inputVal));
  };
  const handleOpenDeposit = async () => {
    if (!user) {
      alert("Пожалуйста, войдите в систему, чтобы открыть вклад!");
      return;
    }

    const depositAmount = getCleanNumber(amount);

    if (user.balance < 0) {
      alert("Вклад? Я уже тебе написал. Ты нищий. Ты нам должен. Ты Лох.");
      return;
    }
    if (depositAmount <= 0) {
      alert("Введите корректную сумму вклада.");
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

      await depositService.openDeposit(depositData);
      
      const updatedUser = { ...user, balance: user.balance - depositAmount };
      localStorage.setItem('user', JSON.stringify(updatedUser));
      setUser(updatedUser);

      alert(`Вклад успешно открыт!`);
      window.location.href = '/profile'; 
    } catch (error) {
      alert("Ошибка: " + error.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Вклады</h1>

      <div className={styles.card}>
        {user && user.balance >= 0 && (
          <p className={styles.balanceInfo}>
            Доступно: <strong>{formatDisplay(user.balance.toFixed(2))} ₽</strong>
          </p>
        )}
        {user && user.balance < 0 && (
          <p className={styles.balanceInfo}>
            <span style={{color:"red"}}>Ты нищий. Ты нам должен. Ты Лох.</span>
          </p>
        )}
        
        <div className={styles.row}>
          <div className={styles.inputGroup}>
            <label className={styles.label}>Сумма вклада</label>
            <input 
              className={styles.input} 
              type="text"
              value={amount}
              onChange={handleAmountChange}
              placeholder="0,00"
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
            <span className={styles.resultValue}>+ {formatDisplay(calculateProfit().toFixed(2))} ₽</span>
          </div>
          <div className={styles.resultInfo} style={{textAlign: 'right'}}>
            <span className={styles.resultLabel}>Итого:</span>
            <span className={styles.resultValue} style={{color: '#5c57af'}}>
              {formatDisplay(total.toFixed(2))} ₽
            </span>
          </div>
        </div>

        <button className={styles.submitBtn} onClick={handleOpenDeposit} disabled={isLoading}>
          {isLoading ? "Обработка..." : "Открыть вклад прямо сейчас"}
        </button>

        <p className={styles.testNotice}>* 1 месяц вклада приравнен к 1 секунде реального времени.</p>
      </div>
    </div>
  );
};

export default Deposits;