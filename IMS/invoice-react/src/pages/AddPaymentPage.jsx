import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getInvoiceById, applyPayment } from '../services/invoiceService';
import LoadingSpinner from '../components/LoadingSpinner';

export default function AddPaymentPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [invoice, setInvoice] = useState(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const [form, setForm] = useState({
    paymentAmount: '',
    paymentDate: new Date().toISOString().split('T')[0],
    paymentMethod: 'BankTransfer',
    referenceNumber: ''
  });

  useEffect(() => {
    getInvoiceById(id).then(setInvoice).catch(() => setError('Invoice not found.')).finally(() => setLoading(false));
  }, [id]);

  const formatCurrency = (amount) => new Intl.NumberFormat('en-IN', {
    style: 'currency', currency: 'INR'
  }).format(amount);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {
      await applyPayment(id, {
        ...form,
        paymentAmount: parseFloat(form.paymentAmount)
      });
      navigate(`/invoices/${id}`);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to apply payment.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="animate-in">
      <div className="page-header">
        <div>
          <h1>Add Payment</h1>
          <p className="subtitle">Invoice: {invoice?.invoiceNumber}</p>
        </div>
      </div>

      {error && <div className="alert alert-error">⚠️ {error}</div>}

      <div className="detail-grid" style={{ marginBottom: '24px' }}>
        <div className="stat-card blue">
          <div className="stat-label">Grand Total</div>
          <div className="stat-value money">{formatCurrency(invoice?.grandTotal || 0)}</div>
        </div>
        <div className="stat-card red">
          <div className="stat-label">Outstanding Balance</div>
          <div className="stat-value money">{formatCurrency(invoice?.outstandingBalance || 0)}</div>
        </div>
        <div className="stat-card green">
          <div className="stat-label">Total Paid</div>
          <div className="stat-value money">{formatCurrency((invoice?.grandTotal || 0) - (invoice?.outstandingBalance || 0))}</div>
        </div>
      </div>

      <div className="card">
        <h3 className="card-title" style={{ marginBottom: '20px' }}>Payment Details</h3>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Payment Amount (₹) *</label>
              <input id="payment-amount" className="form-input" type="number" step="0.01" min="0.01"
                max={invoice?.outstandingBalance} placeholder="Enter amount"
                value={form.paymentAmount} onChange={(e) => setForm({ ...form, paymentAmount: e.target.value })} required />
            </div>
            <div className="form-group">
              <label className="form-label">Payment Date *</label>
              <input className="form-input" type="date" value={form.paymentDate}
                onChange={(e) => setForm({ ...form, paymentDate: e.target.value })} required />
            </div>
          </div>
          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Payment Method *</label>
              <select className="form-select" value={form.paymentMethod}
                onChange={(e) => setForm({ ...form, paymentMethod: e.target.value })}>
                <option value="Cash">Cash</option>
                <option value="CreditCard">Credit Card</option>
                <option value="BankTransfer">Bank Transfer</option>
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Reference Number</label>
              <input className="form-input" type="text" placeholder="Transaction reference"
                value={form.referenceNumber} onChange={(e) => setForm({ ...form, referenceNumber: e.target.value })} />
            </div>
          </div>
          <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end', marginTop: '16px' }}>
            <button type="button" className="btn btn-secondary" onClick={() => navigate(`/invoices/${id}`)}>Cancel</button>
            <button id="payment-submit" type="submit" className="btn btn-primary" disabled={submitting}>
              {submitting ? 'Processing...' : '💳 Apply Payment'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
