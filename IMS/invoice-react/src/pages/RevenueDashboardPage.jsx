import { useState, useEffect } from 'react';
import { getRevenueSummary, getDso } from '../services/analyticsService';
import LoadingSpinner from '../components/LoadingSpinner';

export default function RevenueDashboardPage() {
  const [revenue, setRevenue] = useState(null);
  const [dso, setDso] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    Promise.all([getRevenueSummary(), getDso()])
      .then(([rev, d]) => { setRevenue(rev); setDso(d); })
      .catch(() => setError('Failed to load analytics.'))
      .finally(() => setLoading(false));
  }, []);

  const formatCurrency = (amount) => new Intl.NumberFormat('en-IN', {
    style: 'currency', currency: 'INR'
  }).format(amount);

  if (loading) return <LoadingSpinner />;
  if (error) return <div className="alert alert-error">{error}</div>;

  const collectionRate = revenue?.totalRevenue > 0
    ? ((revenue.totalPaid / revenue.totalRevenue) * 100).toFixed(1)
    : 0;

  return (
    <div className="animate-in">
      <div className="page-header">
        <div>
          <h1>Revenue Dashboard</h1>
          <p className="subtitle">Financial overview and key performance indicators</p>
        </div>
      </div>

      <div className="stats-grid">
        <div className="stat-card blue">
          <div className="stat-label">Total Revenue</div>
          <div className="stat-value money">{formatCurrency(revenue?.totalRevenue || 0)}</div>
        </div>
        <div className="stat-card green">
          <div className="stat-label">Total Collected</div>
          <div className="stat-value money">{formatCurrency(revenue?.totalPaid || 0)}</div>
        </div>
        <div className="stat-card red">
          <div className="stat-label">Outstanding</div>
          <div className="stat-value money">{formatCurrency(revenue?.totalOutstanding || 0)}</div>
        </div>
        <div className="stat-card purple">
          <div className="stat-label">DSO (Days Sales Outstanding)</div>
          <div className="stat-value">{dso?.daysSalesOutstanding || 0} days</div>
        </div>
      </div>

      <div className="detail-grid">
        <div className="card">
          <h3 className="card-title" style={{ marginBottom: '20px' }}>Invoice Statistics</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Total Invoices</span>
              <span style={{ fontWeight: 700, fontSize: '18px' }}>{revenue?.totalInvoices || 0}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Paid Invoices</span>
              <span style={{ fontWeight: 700, fontSize: '18px', color: 'var(--accent-green)' }}>{revenue?.paidInvoices || 0}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Overdue Invoices</span>
              <span style={{ fontWeight: 700, fontSize: '18px', color: 'var(--accent-red)' }}>{revenue?.overdueInvoices || 0}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Partially Paid</span>
              <span style={{ fontWeight: 700, fontSize: '18px', color: 'var(--accent-amber)' }}>{revenue?.partiallyPaidInvoices || 0}</span>
            </div>
          </div>
        </div>

        <div className="card">
          <h3 className="card-title" style={{ marginBottom: '20px' }}>Collection Performance</h3>
          <div style={{ textAlign: 'center', padding: '20px 0' }}>
            <div style={{ fontSize: '48px', fontWeight: 800, background: 'var(--gradient-primary)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
              {collectionRate}%
            </div>
            <p style={{ color: 'var(--text-muted)', marginTop: '8px', fontSize: '14px' }}>Collection Rate</p>
            <div style={{
              height: '8px', background: 'rgba(255,255,255,0.05)', borderRadius: '4px',
              marginTop: '20px', overflow: 'hidden'
            }}>
              <div style={{
                height: '100%', width: `${collectionRate}%`,
                background: 'var(--gradient-primary)', borderRadius: '4px',
                transition: 'width 1s ease-out'
              }}></div>
            </div>
          </div>

          <div style={{ marginTop: '20px', padding: '16px', background: 'rgba(255,255,255,0.02)', borderRadius: '8px' }}>
            <h4 style={{ fontSize: '13px', color: 'var(--text-muted)', marginBottom: '12px' }}>DSO Analysis</h4>
            <div style={{ fontSize: '14px', color: 'var(--text-secondary)', lineHeight: 2 }}>
              <div>Total Outstanding: <strong>{formatCurrency(dso?.totalOutstanding || 0)}</strong></div>
              <div>Total Credit Sales: <strong>{formatCurrency(dso?.totalCreditSales || 0)}</strong></div>
              <div>Period: <strong>{dso?.numberOfDays || 90} days</strong></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
