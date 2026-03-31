import { Routes, Route, Navigate } from 'react-router-dom'
import Navbar from './components/Navbar'
import CustomersPage from './pages/CustomersPage'
import CustomerDetailPage from './pages/CustomerDetailPage'
import AddCustomerPage from './pages/AddCustomerPage'
import EditCustomerPage from './pages/EditCustomerPage'
import './index.css'

// App owns nothing — no customers state, no handlers.
// Each page manages its own data lifecycle via useEffect + API calls.
function App() {
  return (
    <div className="app">
      <Navbar />
      <main className="app-main">
        <Routes>
          <Route path="/" element={<Navigate to="/customers" replace />} />
          <Route path="/customers"          element={<CustomersPage />} />
          <Route path="/customers/new"      element={<AddCustomerPage />} />
          <Route path="/customers/:id"      element={<CustomerDetailPage />} />
          <Route path="/customers/:id/edit" element={<EditCustomerPage />} />
        </Routes>
      </main>
    </div>
  )
}

export default App
