import { useState, useEffect } from 'react';
import { getAgingReport } from '../services/analyticsService';
import LoadingSpinner from '../components/LoadingSpinner';

export default function AgingDashboardPage() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    getAgingReport().then(setData).catch(() => setError('Failed to load aging report.')).finally(() => setLoading(false));
  }, []);

  const formatCurrency = (amount) => new Intl.NumberFormat('en-IN', {
    style: 'currency', currency: 'INR'
  }).format(amount);

  if (loading) return <LoadingSpinner />;
  if (error) return <div className="alert alert-error">{error}</div>;

  const total = data?.totalOutstanding || 1;
  const currentPct = ((data?.current || 0) / total * 100).toFixed(1);
  const thirtyPct = ((data?.oneToThirtyDays || 0) / total * 100).toFixed(1);
  const sixtyPct = ((data?.thirtyOneToSixtyDays || 0) / total * 100).toFixed(1);
  const plusPct = ((data?.sixtyPlusDays || 0) / total * 100).toFixed(1);

  return (
    <div className="animate-in">
      <div className="page-header">
        <div>
          <h1>Aging Dashboard</h1>
          <p className="subtitle">Invoice aging analysis by overdue buckets</p>
        </div>
      </div>

      <div className="stats-grid">
        <div className="stat-card green">
          <div className="stat-label">Current (Not Due)</div>
          <div className="stat-value money">{formatCurrency(data?.current || 0)}</div>
        </div>
        <div className="stat-card blue">
          <div className="stat-label">1–30 Days Overdue</div>
          <div className="stat-value money">{formatCurrency(data?.oneToThirtyDays || 0)}</div>
        </div>
        <div className="stat-card purple">
          <div className="stat-label">31–60 Days Overdue</div>
          <div className="stat-value money">{formatCurrency(data?.thirtyOneToSixtyDays || 0)}</div>
        </div>
        <div className="stat-card red">
          <div className="stat-label">60+ Days Overdue</div>
          <div className="stat-value money">{formatCurrency(data?.sixtyPlusDays || 0)}</div>
        </div>
      </div>

      <div className="card" style={{ marginBottom: '20px' }}>
        <h3 className="card-title" style={{ marginBottom: '16px' }}>Aging Distribution</h3>
        <div className="aging-bar">
          {data?.current > 0 && <div style={{ width: `${currentPct}%`, background: 'var(--accent-green)' }}>{currentPct}%</div>}
          {data?.oneToThirtyDays > 0 && <div style={{ width: `${thirtyPct}%`, background: 'var(--accent-blue)' }}>{thirtyPct}%</div>}
          {data?.thirtyOneToSixtyDays > 0 && <div style={{ width: `${sixtyPct}%`, background: 'var(--accent-purple)' }}>{sixtyPct}%</div>}
          {data?.sixtyPlusDays > 0 && <div style={{ width: `${plusPct}%`, background: 'var(--accent-red)' }}>{plusPct}%</div>}
        </div>
        <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '16px', flexWrap: 'wrap', gap: '12px' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <div style={{ width: '12px', height: '12px', borderRadius: '3px', background: 'var(--accent-green)' }}></div>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>Current</span>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <div style={{ width: '12px', height: '12px', borderRadius: '3px', background: 'var(--accent-blue)' }}></div>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>1–30 Days</span>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <div style={{ width: '12px', height: '12px', borderRadius: '3px', background: 'var(--accent-purple)' }}></div>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>31–60 Days</span>
          </div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <div style={{ width: '12px', height: '12px', borderRadius: '3px', background: 'var(--accent-red)' }}></div>
            <span style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>60+ Days</span>
          </div>
        </div>
      </div>

      <div className="card">
        <h3 className="card-title" style={{ marginBottom: '12px' }}>Summary</h3>
        <div style={{ fontSize: '14px', color: 'var(--text-secondary)', lineHeight: 2 }}>
          <div>Total Outstanding: <strong className="money" style={{ color: 'var(--accent-red)' }}>{formatCurrency(data?.totalOutstanding || 0)}</strong></div>
          <div>Total Overdue Invoices: <strong style={{ color: 'var(--text-primary)' }}>{data?.totalOverdueInvoices || 0}</strong></div>
        </div>
      </div>
    </div>
  );
}
