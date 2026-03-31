import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createInvoice } from '../services/invoiceService';

export default function CreateInvoicePage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const [form, setForm] = useState({
    customerId: 1,
    invoiceDate: new Date().toISOString().split('T')[0],
    dueDate: new Date(Date.now() + 30 * 86400000).toISOString().split('T')[0],
    taxAmount: 0,
    discountAmount: 0,
    lineItems: [{ description: '', quantity: 1, unitPrice: 0, discount: 0, tax: 0 }]
  });

  const updateField = (field, value) => setForm({ ...form, [field]: value });

  const addLineItem = () => {
    setForm({
      ...form,
      lineItems: [...form.lineItems, { description: '', quantity: 1, unitPrice: 0, discount: 0, tax: 0 }]
    });
  };

  const removeLineItem = (index) => {
    if (form.lineItems.length <= 1) return;
    setForm({ ...form, lineItems: form.lineItems.filter((_, i) => i !== index) });
  };

  const updateLineItem = (index, field, value) => {
    const items = [...form.lineItems];
    items[index] = { ...items[index], [field]: value };
    setForm({ ...form, lineItems: items });
  };

  const calcLineTotal = (item) => (item.quantity * item.unitPrice) - item.discount + item.tax;
  const calcSubTotal = () => form.lineItems.reduce((sum, item) => sum + calcLineTotal(item), 0);
  const calcGrandTotal = () => calcSubTotal() - form.discountAmount;

  const formatCurrency = (amount) => new Intl.NumberFormat('en-IN', {
    style: 'currency', currency: 'INR'
  }).format(amount);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const payload = {
        ...form,
        customerId: parseInt(form.customerId),
        taxAmount: parseFloat(form.taxAmount) || 0,
        discountAmount: parseFloat(form.discountAmount) || 0,
        lineItems: form.lineItems.map(li => ({
          ...li,
          quantity: parseInt(li.quantity),
          unitPrice: parseFloat(li.unitPrice),
          discount: parseFloat(li.discount) || 0,
          tax: parseFloat(li.tax) || 0
        }))
      };
      const result = await createInvoice(payload);
      navigate(`/invoices/${result.invoiceId}`);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create invoice.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="animate-in">
      <div className="page-header">
        <div>
          <h1>Create Invoice</h1>
          <p className="subtitle">Generate a new invoice with line items</p>
        </div>
      </div>

      {error && <div className="alert alert-error">⚠️ {error}</div>}

      <form onSubmit={handleSubmit}>
        <div className="card" style={{ marginBottom: '20px' }}>
          <h3 className="card-title" style={{ marginBottom: '20px' }}>Invoice Details</h3>
          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Customer ID</label>
              <select className="form-select" value={form.customerId} onChange={(e) => updateField('customerId', e.target.value)}>
                <option value="1">Acme Corporation</option>
                <option value="2">Globex Industries</option>
                <option value="3">Initech Solutions</option>
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Invoice Date</label>
              <input className="form-input" type="date" value={form.invoiceDate}
                onChange={(e) => updateField('invoiceDate', e.target.value)} required />
            </div>
            <div className="form-group">
              <label className="form-label">Due Date</label>
              <input className="form-input" type="date" value={form.dueDate}
                onChange={(e) => updateField('dueDate', e.target.value)} required />
            </div>
          </div>
          <div className="form-row">
            <div className="form-group">
              <label className="form-label">Invoice Discount (₹)</label>
              <input className="form-input" type="number" step="0.01" min="0"
                value={form.discountAmount} onChange={(e) => updateField('discountAmount', parseFloat(e.target.value) || 0)} />
            </div>
          </div>
        </div>

        <div className="card" style={{ marginBottom: '20px' }}>
          <div className="card-header">
            <h3 className="card-title">Line Items</h3>
            <button type="button" className="btn btn-sm btn-secondary" onClick={addLineItem}>+ Add Item</button>
          </div>
          <div className="line-items-container">
            {form.lineItems.map((item, i) => (
              <div className="line-item-row" key={i}>
                <div className="form-group" style={{ margin: 0 }}>
                  <label className="form-label">Description</label>
                  <input className="form-input" placeholder="Item description" value={item.description}
                    onChange={(e) => updateLineItem(i, 'description', e.target.value)} required />
                </div>
                <div className="form-group" style={{ margin: 0 }}>
                  <label className="form-label">Qty</label>
                  <input className="form-input" type="number" min="1" value={item.quantity}
                    onChange={(e) => updateLineItem(i, 'quantity', parseInt(e.target.value) || 1)} />
                </div>
                <div className="form-group" style={{ margin: 0 }}>
                  <label className="form-label">Unit Price</label>
                  <input className="form-input" type="number" step="0.01" min="0" value={item.unitPrice}
                    onChange={(e) => updateLineItem(i, 'unitPrice', parseFloat(e.target.value) || 0)} />
                </div>
                <div className="form-group" style={{ margin: 0 }}>
                  <label className="form-label">Discount</label>
                  <input className="form-input" type="number" step="0.01" min="0" value={item.discount}
                    onChange={(e) => updateLineItem(i, 'discount', parseFloat(e.target.value) || 0)} />
                </div>
                <div className="form-group" style={{ margin: 0 }}>
                  <label className="form-label">Tax</label>
                  <input className="form-input" type="number" step="0.01" min="0" value={item.tax}
                    onChange={(e) => updateLineItem(i, 'tax', parseFloat(e.target.value) || 0)} />
                </div>
                <div className="line-item-total">{formatCurrency(calcLineTotal(item))}</div>
                <div style={{ paddingTop: '28px' }}>
                  <button type="button" className="btn btn-sm btn-danger" onClick={() => removeLineItem(i)}
                    disabled={form.lineItems.length <= 1}>✕</button>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="card" style={{ marginBottom: '20px' }}>
          <h3 className="card-title" style={{ marginBottom: '16px' }}>Summary</h3>
          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end', gap: '8px' }}>
            <div style={{ fontSize: '14px', color: 'var(--text-secondary)' }}>
              SubTotal: <strong className="money">{formatCurrency(calcSubTotal())}</strong>
            </div>
            <div style={{ fontSize: '14px', color: 'var(--text-secondary)' }}>
              Discount: <strong className="money" style={{ color: 'var(--accent-red)' }}>-{formatCurrency(form.discountAmount)}</strong>
            </div>
            <div style={{ fontSize: '20px', fontWeight: 800, color: 'var(--text-primary)', paddingTop: '8px', borderTop: '1px solid var(--border-color)' }}>
              Grand Total: <span className="money" style={{ color: 'var(--accent-blue)' }}>{formatCurrency(calcGrandTotal())}</span>
            </div>
          </div>
        </div>

        <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
          <button type="button" className="btn btn-secondary" onClick={() => navigate('/')}>Cancel</button>
          <button type="submit" className="btn btn-primary btn-lg" disabled={loading}>
            {loading ? 'Creating...' : '✨ Create Invoice'}
          </button>
        </div>
      </form>
    </div>
  );
}
