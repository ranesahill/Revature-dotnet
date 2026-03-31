import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { user, logout, hasRole } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="sidebar">
      <div className="sidebar-brand">
        <div className="logo-icon">₹</div>
        <div>
          <h2>InvoiceIQ</h2>
          <span className="subtitle">Finance Management</span>
        </div>
      </div>

      <nav className="sidebar-nav">
        <span className="nav-section-title">Invoices</span>
        <NavLink to="/" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`} end>
          <span className="nav-icon">📋</span> Invoice List
        </NavLink>
        {hasRole('FinanceUser', 'Admin') && (
          <NavLink to="/invoices/create" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <span className="nav-icon">➕</span> Create Invoice
          </NavLink>
        )}

        {hasRole('FinanceManager', 'Admin') && (
          <>
            <span className="nav-section-title">Analytics</span>
            <NavLink to="/analytics/aging" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
              <span className="nav-icon">📊</span> Aging Dashboard
            </NavLink>
            <NavLink to="/analytics/revenue" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
              <span className="nav-icon">💰</span> Revenue Dashboard
            </NavLink>
          </>
        )}
      </nav>

      <div className="sidebar-footer">
        <div className="user-info">
          <span className="username">{user?.username}</span>
          <span className="role">{user?.role}</span>
        </div>
        <button className="btn btn-sm btn-secondary" onClick={handleLogout}>Logout</button>
      </div>
    </div>
  );
}
