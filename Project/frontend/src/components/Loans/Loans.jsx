import React, { useState } from 'react';
import styles from './Loans.module.css';
import { loanService } from '../../services/loanService';

const Loans = () => {
  const [amount, setAmount] = useState(1000);
  const [seconds, setSeconds] = useState(12);
  const [loading, setLoading] = useState(false);
  const rate = 36; 

  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const userId = user.id || user.Id;

  const calculateAnnuityPayment = () => {
    const monthlyRate = rate / 12 / 100;
    
    const pow = Math.pow(1 + monthlyRate, seconds);
    const payment = amount * (monthlyRate * pow) / (pow - 1);
    
    return isFinite(payment) ? payment : 0;
  };

  const perSecondPayment = calculateAnnuityPayment();
  const totalAmount = (perSecondPayment * seconds).toFixed(2);

  const handleTakeLoan = async () => {
    if (!userId) return alert("Пользователь не авторизован");
    
    setLoading(true);
    try {
      await loanService.takeLoan({
        userId: userId,
        amount: parseFloat(amount),
        termInSeconds: seconds,
        interestRate: rate
      });
      alert('Кредит успешно оформлен!');
      window.location.href = '/profile';
    } catch (err) {
      alert(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Кредитование</h1>
      <div className={styles.card}>
        <div className={styles.row}>
          <div className={styles.inputGroup}>
            <label className={styles.label}>Сумма кредита</label>
            <input 
              type="number" 
              className={styles.input} 
              value={amount}
              onChange={(e) => setAmount(Number(e.target.value))}
            />
          </div>

          <div className={styles.inputGroup}>
            <label className={styles.label}>Срок возврата</label>
            <select 
              className={styles.select}
              value={seconds}
              onChange={(e) => setSeconds(Number(e.target.value))}
            >
              <option value={12}>12 секунд</option>
              <option value={24}>24 секунды</option>
              <option value={36}>36 секунд</option>
              <option value={48}>48 секунд</option>
              <option value={60}>60 секунд</option>
            </select>
          </div>
        </div>

        <div className={styles.resultBox}>
          <div className={styles.resultInfo}>
            <span className={styles.resultLabel}>{rate}% годовых</span>
            <span className={styles.resultValue}>
              - {perSecondPayment.toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} ₽/сек
            </span>
          </div>
          <div className={styles.resultInfo} style={{textAlign: 'right'}}>
            <span className={styles.resultLabel}>К возврату (всего)</span>
            <span className={styles.resultValue} style={{color: '#5c57af'}}>
              {Number(totalAmount).toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} ₽
            </span>
          </div>
        </div>

        <button 
          className={styles.submitBtn} 
          onClick={handleTakeLoan}
          disabled={loading || amount <= 0}
        >
          {loading ? 'Оформление...' : 'Оформить кредит'}
        </button>

        <p className={styles.testNotice}>
          * В расчетах используется аннуитетная схема. 1 месяц = 1 секунда.
        </p>
      </div>
    </div>
  );
};

export default Loans;