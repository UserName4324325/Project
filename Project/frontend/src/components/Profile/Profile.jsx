import React, { useState, useEffect, useCallback, useMemo } from 'react';
import styles from './Profile.module.css';
import { depositService } from '../../services/depositService';
import { loanService } from '../../services/loanService';
import { userService } from '../../services/userService';
import { remittanceService } from '../../services/remittanceService';

const Profile = () => {
  const [activeTab, setActiveTab] = useState('overview');
  const [loading, setLoading] = useState(false);
  
  const [deposits, setDeposits] = useState([]);
  const [loans, setLoans] = useState([]);
  const [remittances, setRemittances] = useState([]);
  const [users, setUsers] = useState([]);
  const [balance, setBalance] = useState(0);

  const [remittancesForm, setRemittancesForm] = useState({ recipientId: '', amount: '' });
  const userFromStorage = useMemo(() => JSON.parse(localStorage.getItem('user') || '{}'), []);


  const fetchAllData = useCallback(async () => {
    try {
      const [dep, loan, user, hist, allUsers] = await Promise.all([
        depositService.getUserDeposits(userFromStorage.id),
        loanService.getUserLoans(userFromStorage.id),
        userService.getUser(),
        remittanceService.getHistory(userFromStorage.id),
        userService.getAllUsers()
      ]);

      if (dep) setDeposits(dep);
      if (loan) setLoans(loan);
      if (hist) setRemittances(hist);
      if (allUsers) setUsers(allUsers);
      if (user) {
        setBalance(user.balance);
        localStorage.setItem('user', JSON.stringify({ ...userFromStorage, balance: user.balance }));
      }
    } catch (err) {
      console.error("Ошибка загрузки данных:", err);
    }
  }, [userFromStorage.id]);


  const fetchLightData = useCallback(async () => {
    try {
      const [dep, loan] = await Promise.all([
        depositService.getUserDeposits(userFromStorage.id),
        loanService.getUserLoans(userFromStorage.id),
      ]);
      if (loan) setLoans(loan);
      if (dep) setDeposits(dep);
    } catch (err) {
      console.error("Ошибка фонового обновления:", err);
    }
  }, [userFromStorage.id]);


  const formatDisplay = (val) => {
    if (!val && val !== 0) return "";
    let valStr = val.toString().replace('.', ',');
    let [integer, decimal] = valStr.split(',');
    integer = integer.replace(/\s/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, " ");
    return decimal !== undefined ? `${integer},${decimal.substring(0, 2)}` : integer;
  };

  const getCleanNumber = (val) => {
    const clean = val.toString().replace(/\s/g, "").replace(",", ".");
    return parseFloat(clean) || 0;
  };

  const handleRemittanceAmountChange = (e) => {
    let inputVal = e.target.value;
    inputVal = inputVal.replace(/[^\d.,]/g, "");

    if (inputVal.startsWith('0') && inputVal.length > 1) {
      if (inputVal[1] !== ',' && inputVal[1] !== '.') inputVal = inputVal.substring(1);
    }

    let raw = inputVal.replace(",", ".");
    const parts = raw.split(".");
    if (parts.length > 2 || (parts[1] && parts[1].length > 2)) return;

    const numericValue = parseFloat(raw) || 0;
    if (numericValue > balance) {
        setRemittancesForm(prev => ({ ...prev, amount: formatDisplay(balance) }));
        return;
    }

    if (inputVal.endsWith(",") || inputVal.endsWith(".")) {
      setRemittancesForm(prev => ({ ...prev, amount: formatDisplay(parts[0]) + "," }));
    } else {
      setRemittancesForm(prev => ({ ...prev, amount: formatDisplay(raw) }));
    }
  };

  const handleRemittance = async (e) => {
    e.preventDefault();
    const cleanAmount = getCleanNumber(remittancesForm.amount);
    if (cleanAmount <= 0) return alert("Сумма должна быть больше 0");

    try {
      await remittanceService.remittance({
        senderId: userFromStorage.id,
        recipientId: remittancesForm.recipientId,
        amount: cleanAmount
      });
      alert("Перевод успешно выполнен!");
      setRemittancesForm({ recipientId: '', amount: '' });
      fetchAllData();
    } catch (err) {
      alert(err.message);
    }
  };

  useEffect(() => {
    setLoading(true);
    fetchAllData().finally(() => setLoading(false));
  }, [fetchAllData]);

  useEffect(() => {
    const hasActiveLoans = loans.some(l => !(l.isPaid || l.IsPaid));
    const hasActiveDeposits = deposits.some(d => !(d.isClosed || d.IsClosed));

    if (!hasActiveLoans && !hasActiveDeposits) return;
    

    const interval = setInterval(fetchLightData, 1000);
    return () => clearInterval(interval);
  }, [loans, deposits, fetchLightData]);

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
        {['overview', 'deposits', 'loans', 'remittances'].map(tab => (
          <button 
            key={tab}
            className={activeTab === tab ? styles.activeTab : ''} 
            onClick={() => setActiveTab(tab)}
          >
            {tab === 'overview' ? 'Обзор' : tab === 'deposits' ? 'Вклады' : tab === 'loans' ? 'Кредиты' : 'Переводы'}
          </button>
        ))}
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
              <p className={styles.statsNumber}>{loans.filter(l => !(l.isPaid || l.IsPaid)).length}</p>
            </div>
          </div>
        )}

        {activeTab === 'deposits' && (
          <div className={styles.depositsSection}>
            {loading ? <div className={styles.emptyState}>Загрузка...</div> : deposits.length > 0 ? (
              <div className={styles.tableWrapper}>
                <table className={styles.depositTable}>
                  <thead><tr><th>Сумма</th><th>Прибыль</th><th>Итого</th><th>Дата открытия</th><th>Срок</th><th>Статус</th></tr></thead>
                  <tbody>
                    {deposits.map((dep) => (
                      <tr key={dep.id || dep.Id}>
                        <td>{(dep.amount || 0).toLocaleString()} ₽</td>
                        <td className={styles.profitText}>+{(dep.profit || 0).toLocaleString()} ₽</td>
                        <td className={styles.totalText}>{((dep.amount || 0) + (dep.profit || 0)).toLocaleString()} ₽</td>
                        <td>{dep.startDate ? new Date(dep.startDate).toLocaleString('ru-RU') : '—'}</td>
                        <td>{(dep.term || dep.TermInSeconds || dep.termInSeconds || 0).toLocaleString()} сек.</td>
                        <td><span className={dep.isClosed ? styles.statusClosed : styles.statusActive}>{dep.isClosed ? 'Завершен' : 'В работе'}</span></td>
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
                  <thead><tr><th>Общий долг</th><th>Осталось</th><th>Платеж/сек</th><th>Дата</th><th>Срок</th><th>Статус</th></tr></thead>
                  <tbody>
                    {loans.map(loan => (
                      <tr key={loan.id || loan.Id}>
                        <td>{(loan.totalAmount || loan.TotalAmount || 0).toLocaleString()} ₽</td>
                        <td style={{ fontWeight: 'bold', color: '#e74c3c' }}>{(loan.remainingAmount || loan.RemainingAmount || 0).toLocaleString()} ₽</td>
                        <td>{(loan.perSecondPayment || loan.PerSecondPayment || 0).toFixed(2)} ₽</td>
                        <td>{loan.startDate ? new Date(loan.startDate).toLocaleString('ru-RU') : '—'}</td>
                        <td>{(loan.term || loan.TermInSeconds || loan.termInSeconds || 0).toLocaleString()} сек.</td>
                        <td><span className={(loan.isPaid || loan.IsPaid) ? styles.statusClosed : styles.statusActive}>{(loan.isPaid || loan.IsPaid) ? 'Погашен' : 'Выплачивается'}</span></td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : <div className={styles.emptyState}>Нет активных кредитов.</div>}
          </div>
        )}

        {activeTab === 'remittances' && (
          <div className={styles.depositsSection}>
            <form onSubmit={handleRemittance} className={styles.transferForm}>
              <h3>Отправить деньги</h3>
              <div className={styles.inputGroup}>
                <select className={styles.select} value={remittancesForm.recipientId} onChange={(e) => setRemittancesForm({ ...remittancesForm, recipientId: e.target.value })} required>
                  <option value="">Выберите получателя</option>
                  {users.map(user => (
                    <option key={user.id} value={user.id}>{user.fullName} ({user.email})</option>
                  ))}
                </select>
                <input type="text" placeholder="Сумма" value={remittancesForm.amount} onChange={handleRemittanceAmountChange} required />
              </div>
              <button type="submit" className={styles.btnAction}>Перевести</button>
            </form>

            <div className={styles.tableWrapper}>
              <table className={styles.depositTable}>
                <thead><tr><th>Контрагент</th><th>Дата</th><th>Сумма</th></tr></thead>
                <tbody>
                  {remittances.map(r => (
                    <tr key={r.id}>
                      <td>{r.counterpartyFullName}</td>
                      <td>{r.date ? new Date(r.date).toLocaleString('ru-RU') : '—'}</td>
                      <td style={{ color: r.isIncoming ? '#27ae60' : '#e74c3c' }}>{r.isIncoming ? '+' : '-'}{r.amount.toLocaleString()} ₽</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Profile;