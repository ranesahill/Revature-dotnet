import { NavLink } from 'react-router-dom'

const Navbar = () => (
  <nav className="navbar">
    <span className="nav-brand">CRM App</span>
    <div className="nav-links">
      <NavLink to="/customers" className={({ isActive }) => isActive ? 'nav-link nav-link--active' : 'nav-link'}>
        Customers
      </NavLink>
    </div>
  </nav>
)

export default Navbar
