import api from './api';

export const getAgingReport = () =>
  api.get('/invoices/analytics/aging').then(r => r.data);

export const getRevenueSummary = () =>
  api.get('/invoices/analytics/revenue-summary').then(r => r.data);

export const getDso = (days = 90) =>
  api.get(`/invoices/analytics/dso?days=${days}`).then(r => r.data);

export const getOutstanding = () =>
  api.get('/invoices/analytics/outstanding').then(r => r.data);
