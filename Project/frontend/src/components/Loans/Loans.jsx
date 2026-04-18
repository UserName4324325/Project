import React, { use, useState } from 'react';
import styles from './Loans.module.css';
import { loanService } from '../../services/loanService';

const Loans = () => {
  const [amount, setAmount] = useState('');
  const [seconds, setSeconds] = useState(12);
  const [loading, setLoading] = useState(false);
  const rate = 36; 
  const MAX_LOAN = 10000000;

  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const userId = user.id || user.Id;

  const formatDisplay = (val) => {
    if (!val && val !== 0) return "";
    let parts = val.toString().replace(/ /g, "").replace(",", ".").split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, " ");
    if (parts[1]) parts[1] = parts[1].substring(0, 2);
    return parts.join(",");
  };

  const getCleanNumber = (val) => {
    const clean = val.toString().replace(/ /g, "").replace(",", ".");
    return parseFloat(clean) || 0;
  };

  const calculateAnnuityPayment = () => {
    const numAmount = getCleanNumber(amount);
    const monthlyRate = rate / 12 / 100;
    
    const pow = Math.pow(1 + monthlyRate, seconds);
    const payment = numAmount * (monthlyRate * pow) / (pow - 1);
    
    return isFinite(payment) ? payment : 0;
  };

  const perSecondPayment = calculateAnnuityPayment();
  const totalAmount = (perSecondPayment * seconds);

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

    if (numericValue > MAX_LOAN) {
      numericValue = MAX_LOAN;
      setAmount(formatDisplay(numericValue));
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

  const handleTakeLoan = async () => {
    if (!userId) return alert("Пользователь не авторизован");
    
    const cleanAmount = getCleanNumber(amount);

    if (user.balance < 0) return alert("На кой тебе кредит, если у тебя отрицательный баланс? Погаси сначала долги.");
    if (cleanAmount <= 0) {
      alert("Сумма должна быть больше 0")
      return ;
    }

    setLoading(true);

    try {
      await loanService.takeLoan({
        userId: userId,
        amount: cleanAmount,
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
            <label className={styles.label}>Сумма кредита (до 10 млн ₽)</label>
            <input 
              type="text"
              className={styles.input} 
              value={amount}
              onChange={handleAmountChange}
              placeholder="0,00"
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
              - {formatDisplay(perSecondPayment.toFixed(2))} ₽/сек
            </span>
          </div>
          <div className={styles.resultInfo} style={{textAlign: 'right'}}>
            <span className={styles.resultLabel}>К возврату (всего)</span>
            <span className={styles.resultValue} style={{color: '#5c57af'}}>
              {formatDisplay(totalAmount.toFixed(2))} ₽
            </span>
          </div>
        </div>

        <button 
          className={styles.submitBtn} 
          onClick={handleTakeLoan}
          disabled={loading}
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