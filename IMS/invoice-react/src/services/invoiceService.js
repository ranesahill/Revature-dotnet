import api from './api';

export const getInvoices = (page = 1, pageSize = 10) =>
  api.get(`/invoices?page=${page}&pageSize=${pageSize}`).then(r => r.data);

export const getInvoiceById = (id) =>
  api.get(`/invoices/${id}`).then(r => r.data);

export const createInvoice = (data) =>
  api.post('/invoices', data).then(r => r.data);

export const updateInvoice = (id, data) =>
  api.put(`/invoices/${id}`, data).then(r => r.data);

export const deleteInvoice = (id) =>
  api.delete(`/invoices/${id}`);

export const changeInvoiceStatus = (id, status) =>
  api.patch(`/invoices/${id}/status`, { status });

// Line Items
export const getLineItems = (invoiceId) =>
  api.get(`/invoices/${invoiceId}/items`).then(r => r.data);

export const addLineItem = (invoiceId, data) =>
  api.post(`/invoices/${invoiceId}/items`, data).then(r => r.data);

export const updateLineItem = (invoiceId, itemId, data) =>
  api.put(`/invoices/${invoiceId}/items/${itemId}`, data).then(r => r.data);

export const deleteLineItem = (invoiceId, itemId) =>
  api.delete(`/invoices/${invoiceId}/items/${itemId}`);

// Payments
export const getPayments = (invoiceId) =>
  api.get(`/invoices/${invoiceId}/payments`).then(r => r.data);

export const applyPayment = (invoiceId, data) =>
  api.post(`/invoices/${invoiceId}/payments`, data).then(r => r.data);
