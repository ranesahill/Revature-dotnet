import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Navbar from './components/Navbar';
import LoginPage from './pages/LoginPage';
import InvoiceListPage from './pages/InvoiceListPage';
import CreateInvoicePage from './pages/CreateInvoicePage';
import InvoiceDetailPage from './pages/InvoiceDetailPage';
import AddPaymentPage from './pages/AddPaymentPage';
import AgingDashboardPage from './pages/AgingDashboardPage';
import RevenueDashboardPage from './pages/RevenueDashboardPage';
import './index.css';

function AppLayout() {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Routes><Route path="*" element={<LoginPage />} /></Routes>;
  }

  return (
    <div className="app-container">
      <Navbar />
      <main className="main-content">
        <Routes>
          <Route path="/" element={
            <ProtectedRoute><InvoiceListPage /></ProtectedRoute>
          } />
          <Route path="/invoices/create" element={
            <ProtectedRoute roles={['FinanceUser', 'Admin']}><CreateInvoicePage /></ProtectedRoute>
          } />
          <Route path="/invoices/:id" element={
            <ProtectedRoute><InvoiceDetailPage /></ProtectedRoute>
          } />
          <Route path="/invoices/:id/payment" element={
            <ProtectedRoute roles={['FinanceUser', 'Admin']}><AddPaymentPage /></ProtectedRoute>
          } />
          <Route path="/analytics/aging" element={
            <ProtectedRoute roles={['FinanceManager', 'Admin']}><AgingDashboardPage /></ProtectedRoute>
          } />
          <Route path="/analytics/revenue" element={
            <ProtectedRoute roles={['FinanceManager', 'Admin']}><RevenueDashboardPage /></ProtectedRoute>
          } />
          <Route path="/login" element={<Navigate to="/" replace />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppLayout />
      </AuthProvider>
    </BrowserRouter>
  );
}
