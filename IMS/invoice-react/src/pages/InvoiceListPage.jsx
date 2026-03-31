import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getInvoices } from '../services/invoiceService';
import StatusBadge from '../components/StatusBadge';
import LoadingSpinner from '../components/LoadingSpinner';

export default function InvoiceListPage() {
  const [data, setData] = useState({ items: [], totalCount: 0, page: 1, pageSize: 10, totalPages: 0 });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchInvoices = async (page = 1) => {
    setLoading(true);
    try {
      const result = await getInvoices(page, 10);
      setData(result);
    } catch (err) {
      setError('Failed to load invoices.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchInvoices(); }, []);

  const formatCurrency = (amount) => new Intl.NumberFormat('en-IN', {
    style: 'currency', currency: 'INR'
  }).format(amount);

  if (loading) return <LoadingSpinner />;

  return (
    <div className="animate-in">
      <div className="page-header">
        <div>
          <h1>Invoices</h1>
          <p className="subtitle">Manage and track all invoices</p>
        </div>
        <Link to="/invoices/create" className="btn btn-primary">+ New Invoice</Link>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Invoice #</th>
                <th>Customer</th>
                <th>Date</th>
                <th>Due Date</th>
                <th>Status</th>
                <th>Total</th>
                <th>Outstanding</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {data.items.length === 0 ? (
                <tr><td colSpan="8" style={{ textAlign: 'center', padding: '40px', color: 'var(--text-muted)' }}>No invoices found. Create your first invoice!</td></tr>
              ) : data.items.map((inv) => (
                <tr key={inv.invoiceId} className={inv.isOverdue ? 'row-overdue' : ''}>
                  <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{inv.invoiceNumber}</td>
                  <td>{inv.customerName || '—'}</td>
                  <td>{new Date(inv.invoiceDate).toLocaleDateString()}</td>
                  <td>{new Date(inv.dueDate).toLocaleDateString()}</td>
                  <td><StatusBadge status={inv.status} /></td>
                  <td className="money">{formatCurrency(inv.grandTotal)}</td>
                  <td className={`money ${inv.outstandingBalance > 0 ? 'money-negative' : 'money-positive'}`}>
                    {formatCurrency(inv.outstandingBalance)}
                  </td>
                  <td>
                    <Link to={`/invoices/${inv.invoiceId}`} className="btn btn-sm btn-secondary">View</Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {data.totalPages > 1 && (
          <div className="pagination">
            <button disabled={!data.hasPrevious} onClick={() => fetchInvoices(data.page - 1)}>← Prev</button>
            {Array.from({ length: data.totalPages }, (_, i) => (
              <button key={i + 1} className={data.page === i + 1 ? 'active' : ''} onClick={() => fetchInvoices(i + 1)}>
                {i + 1}
              </button>
            ))}
            <button disabled={!data.hasNext} onClick={() => fetchInvoices(data.page + 1)}>Next →</button>
          </div>
        )}
      </div>
    </div>
  );
}
