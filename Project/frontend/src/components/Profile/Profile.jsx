import React, { useState, useEffect, useCallback } from 'react';
import styles from './Profile.module.css';
import { depositService } from '../../services/depositService';
import { loanService } from '../../services/loanService';
import { userService } from '../../services/userService';

const Profile = () => {
  const [activeTab, setActiveTab] = useState('overview');
  const [deposits, setDeposits] = useState([]);
  const [loans, setLoans] = useState([]);
  const [loading, setLoading] = useState(false);
  
  const userFromStorage = JSON.parse(localStorage.getItem('user') || '{}');
  const [balance, setBalance] = useState(userFromStorage.balance || 0);


  const refreshData = useCallback(async () => {
    try {
      const [depositsData, loansData, userData] = await Promise.all([
        depositService.getUserDeposits(userFromStorage.id),
        loanService.getUserLoans(userFromStorage.id),
        userService.getUser()
      ]);

      if (depositsData) setDeposits(depositsData);
      if (loansData) setLoans(loansData);

      if (userData) {
        setBalance(userData.balance);
        localStorage.setItem('user', JSON.stringify({ 
          ...userFromStorage, 
          balance: userData.balance 
        }));
      }
    } catch (err) {
      console.error("Ошибка при обновлении данных:", err);
    }
  }, [userFromStorage.id]);

  useEffect(() => {
    setLoading(true);
    refreshData().finally(() => setLoading(false));
  }, [refreshData]);

  useEffect(() => {
    const timers = deposits.map(dep => {
      const start = new Date(dep.startDate || dep.StartDate).getTime();
      const term = (dep.termInSeconds || dep.TermInSeconds) * 1000;
      const endTime = start + term;
      const delay = Math.max(endTime - Date.now() + 2000, 1000); 

      return setTimeout(() => refreshData(), delay);
    });

    return () => timers.forEach(t => clearTimeout(t));
  }, [deposits, refreshData]);

  useEffect(() => {
    const hasActiveLoans = loans.some(l => !(l.isPaid || l.IsPaid));
    
    if (hasActiveLoans) {
      const interval = setInterval(() => {
        refreshData();
      }, 1000);
      
      return () => clearInterval(interval);
    }
  }, [loans, refreshData]);

  return (
    <div className={styles.profileContainer}>
      <section className={styles.header}>
        <h1>Личный кабинет</h1>
        <p className={styles.welcome}>Рады видеть вас, <span>{userFromStorage.fullName}</span>!</p>
      </section>

      <div className={styles.balanceCard}>
        <div className={styles.balanceInfo}>
          <span className={styles.label}>Ваш общий баланс</span>
          <h2 className={styles.amount}>{balance.toLocaleString()} ₽</h2>
        </div>
      </div>

      <nav className={styles.tabs}>
        <button className={activeTab === 'overview' ? styles.activeTab : ''} onClick={() => setActiveTab('overview')}>Обзор</button>
        <button className={activeTab === 'deposits' ? styles.activeTab : ''} onClick={() => setActiveTab('deposits')}>Вклады</button>
        <button className={activeTab === 'loans' ? styles.activeTab : ''} onClick={() => setActiveTab('loans')}>Кредиты</button>
      </nav>

      <div className={styles.content}>
        {activeTab === 'overview' && (
          <div className={styles.grid}>
            <div className={styles.statsCard}>
              <h3>Активные вклады</h3>
              <p className={styles.statsNumber}>{deposits.filter(d => !(d.isClosed || d.IsClosed)).length}</p>
            </div>
            <div className={styles.statsCard}>
              <h3>Активные кредиты</h3>
              <p className={styles.statsNumber}>
                {loans.filter(l => !(l.isPaid || l.IsPaid)).length}
              </p>
            </div>
          </div>
        )}
        
        {activeTab === 'deposits' && (
           <div className={styles.depositsSection}>
            {loading ? (
               <div className={styles.emptyState}>Загрузка...</div>
            ) : deposits.length > 0 ? (
               <div className={styles.tableWrapper}>
                 <table className={styles.depositTable}>
                   <thead>
                     <tr>
                      <th>Сумма</th>
                      <th>Прибыль</th>
                      <th>Итого</th>
                      <th>Дата открытия</th>
                      <th>Срок</th>
                      <th>Статус</th>
                     </tr>
                   </thead>
                   <tbody>
                    {[...deposits]
                      .map((dep) => (
                        <tr key={dep.id || dep.Id}>
                          <td>{(dep.amount || 0).toLocaleString()} ₽</td>
                          <td className={styles.profitText}>
                            +{(dep.profit || 0).toLocaleString()} ₽
                          </td>
                          <td className={styles.totalText}>
                            {((dep.amount || 0) + (dep.profit || 0)).toLocaleString()} ₽
                          </td>
                          <td>
                            {dep.startDate 
                              ? new Date(dep.startDate).toLocaleString('ru-RU', {
                                  day: '2-digit',
                                  month: '2-digit',
                                  year: 'numeric',
                                  hour: '2-digit',
                                  minute: '2-digit',
                                  second: '2-digit'
                                }) 
                              : '—'}
                          </td>
                          <td>
                            {(dep.term || dep.TermInSeconds || dep.termInSeconds || 0).toLocaleString()} сек.
                          </td>
                          <td>
                            <span className={dep.isClosed ? styles.statusClosed : styles.statusActive}>
                              {dep.isClosed ? 'Завершен' : 'В работе'}
                            </span>
                          </td>
                        </tr>
                      ))}
                  </tbody>
                 </table>
               </div>
            ) : <div className={styles.emptyState}>Нет вкладов.</div>}
           </div>
        )}

        {activeTab === 'loans' && (
          <div className={styles.depositsSection}>
            {loans.length > 0 ? (
              <div className={styles.tableWrapper}>
                <table className={styles.depositTable}>
                  <thead>
                    <tr>
                      <th>Общий долг</th>
                      <th>Осталось</th>
                      <th>Платеж/сек</th>
                      <th>Дата открытия</th>
                      <th>Срок</th>
                      <th>Статус</th>
                    </tr>
                  </thead>
                  <tbody>
                    {loans.map((loan) => {
                      const total = loan.totalAmount || loan.TotalAmount || 0;
                      const remaining = loan.remainingAmount || loan.RemainingAmount || 0;
                      const perSec = loan.perSecondPayment || loan.PerSecondPayment || 0;
                      const isPaid = loan.isPaid || loan.IsPaid || false;

                      return (
                        <tr key={loan.id || loan.Id}>
                          <td>{total.toLocaleString()} ₽</td>
                          <td style={{ fontWeight: 'bold', color: '#e74c3c' }}>
                            {remaining.toLocaleString()} ₽
                          </td>
                          <td>{perSec.toFixed(2)} ₽</td>
                          <td>{loan.startDate 
                              ? new Date(loan.startDate).toLocaleString('ru-RU', {
                                  day: '2-digit',
                                  month: '2-digit',
                                  year: 'numeric',
                                  hour: '2-digit',
                                  minute: '2-digit',
                                  second: '2-digit'
                                }) 
                              : '—'}
                          </td>
                          <td>{(loan.term || loan.TermInSeconds || loan.termInSeconds || 0).toLocaleString()} сек.</td>
                          <td>
                            <span className={isPaid ? styles.statusClosed : styles.statusActive}>
                              {isPaid ? 'Погашен' : 'Выплачивается'}
                            </span>
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className={styles.emptyState}>У вас нет задолженностей по кредитам.</div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default Profile;