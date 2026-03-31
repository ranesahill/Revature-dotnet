import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { getInvoiceById, deleteInvoice } from '../services/invoiceService';
import { useAuth } from '../context/AuthContext';
import StatusBadge from '../components/StatusBadge';
import LoadingSpinner from '../components/LoadingSpinner';

export default function InvoiceDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { hasRole } = useAuth();
  const [invoice, setInvoice] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetch = async () => {
      try {
        const data = await getInvoiceById(id);
        setInvoice(data);
      } catch (err) {
        setError('Invoice not found.');
      } finally {
        setLoading(false);
      }
    };
    fetch();
  }, [id]);

  const handleDelete = async () => {
    if (!confirm('Are you sure you want to delete this invoice?')) return;
    try {
      await deleteInvoice(id);
      navigate('/');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete invoice.');
    }
  };

  const formatCurrency = (amount) => new Intl.NumberFormat('en-IN', {
    style: 'currency', currency: 'INR'
  }).format(amount);

  if (loading) return <LoadingSpinner />;
  if (error) return <div className="alert alert-error">{error}</div>;
  if (!invoice) return null;

  const isPaid = invoice.status === 'Paid';

  return (
    <div className="animate-in">
      <div className="page-header">
        <div>
          <h1>{invoice.invoiceNumber}</h1>
          <p className="subtitle">Invoice Details</p>
        </div>
        <div style={{ display: 'flex', gap: '8px' }}>
          {!isPaid && hasRole('FinanceUser', 'Admin') && (
            <Link to={`/invoices/${id}/payment`} className="btn btn-success">💳 Add Payment</Link>
          )}
          {hasRole('Admin') && (
            <button className="btn btn-danger" onClick={handleDelete}>🗑️ Delete</button>
          )}
        </div>
      </div>

      <div className="detail-grid">
        <div className="card">
          <h3 className="card-title" style={{ marginBottom: '16px' }}>Invoice Information</h3>
          <div className="detail-item"><label>Invoice Number</label><div className="value">{invoice.invoiceNumber}</div></div>
          <div className="detail-item" style={{ marginTop: '12px' }}><label>Customer</label><div className="value">{invoice.customerName || `Customer #${invoice.customerId}`}</div></div>
          <div className="detail-item" style={{ marginTop: '12px' }}><label>Status</label><div className="value"><StatusBadge status={invoice.status} /></div></div>
          <div className="detail-item" style={{ marginTop: '12px' }}><label>Created</label><div className="value">{new Date(invoice.createdDate).toLocaleDateString()}</div></div>
        </div>
        <div className="card">
          <h3 className="card-title" style={{ marginBottom: '16px' }}>Dates & Amounts</h3>
          <div className="detail-item"><label>Invoice Date</label><div className="value">{new Date(invoice.invoiceDate).toLocaleDateString()}</div></div>
          <div className="detail-item" style={{ marginTop: '12px' }}><label>Due Date</label><div className="value">{new Date(invoice.dueDate).toLocaleDateString()}</div></div>
          <div className="detail-item" style={{ marginTop: '12px' }}><label>Grand Total</label><div className="value money" style={{ fontSize: '22px', color: 'var(--accent-blue)' }}>{formatCurrency(invoice.grandTotal)}</div></div>
          <div className="detail-item" style={{ marginTop: '12px' }}><label>Outstanding Balance</label>
            <div className={`value money ${invoice.outstandingBalance > 0 ? 'money-negative' : 'money-positive'}`} style={{ fontSize: '22px' }}>
              {formatCurrency(invoice.outstandingBalance)}
            </div>
          </div>
        </div>
      </div>

      {/* Line Items */}
      <div className="card" style={{ marginBottom: '20px' }}>
        <h3 className="card-title" style={{ marginBottom: '16px' }}>Line Items</h3>
        <div className="table-container">
          <table>
            <thead>
              <tr><th>Description</th><th>Qty</th><th>Unit Price</th><th>Discount</th><th>Tax</th><th>Total</th></tr>
            </thead>
            <tbody>
              {(invoice.lineItems || []).map((item) => (
                <tr key={item.lineItemId}>
                  <td>{item.description}</td>
                  <td>{item.quantity}</td>
                  <td className="money">{formatCurrency(item.unitPrice)}</td>
                  <td className="money">{formatCurrency(item.discount)}</td>
                  <td className="money">{formatCurrency(item.tax)}</td>
                  <td className="money" style={{ fontWeight: 600 }}>{formatCurrency(item.lineTotal)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '16px', gap: '16px', fontSize: '14px' }}>
          <span style={{ color: 'var(--text-muted)' }}>SubTotal: <strong>{formatCurrency(invoice.subTotal)}</strong></span>
          <span style={{ color: 'var(--accent-red)' }}>Discount: -<strong>{formatCurrency(invoice.discountAmount)}</strong></span>
          <span style={{ color: 'var(--accent-blue)', fontSize: '16px', fontWeight: 700 }}>Grand Total: {formatCurrency(invoice.grandTotal)}</span>
        </div>
      </div>

      {/* Payments */}
      <div className="card">
        <div className="card-header">
          <h3 className="card-title">Payments</h3>
          {!isPaid && <Link to={`/invoices/${id}/payment`} className="btn btn-sm btn-success">+ Add Payment</Link>}
        </div>
        <div className="table-container">
          <table>
            <thead>
              <tr><th>Date</th><th>Method</th><th>Reference</th><th>Amount</th></tr>
            </thead>
            <tbody>
              {(invoice.payments || []).length === 0 ? (
                <tr><td colSpan="4" style={{ textAlign: 'center', color: 'var(--text-muted)', padding: '24px' }}>No payments recorded</td></tr>
              ) : invoice.payments.map((p) => (
                <tr key={p.paymentId}>
                  <td>{new Date(p.paymentDate).toLocaleDateString()}</td>
                  <td>{p.paymentMethod}</td>
                  <td>{p.referenceNumber || '—'}</td>
                  <td className="money money-positive" style={{ fontWeight: 600 }}>{formatCurrency(p.paymentAmount)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
