import { useState, useEffect } from 'react'
import { useParams, useNavigate, Link } from 'react-router-dom'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import { getCustomer, updateCustomer, deleteCustomer } from '../api/customersApi'

const CustomerDetailPage = () => {
  const { id } = useParams()
  const navigate = useNavigate()

  const [customer, setCustomer] = useState(null)
  const [loading, setLoading]   = useState(true)
  const [error, setError]       = useState(null)

  const fetchCustomer = async () => {
    try {
      setLoading(true)
      setError(null)
      const response = await getCustomer(id)
      setCustomer(response.data)
    } catch (err) {
      setError('Failed to load customer. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  // Re-fetch whenever :id changes in the URL
  useEffect(() => {
    fetchCustomer()
  }, [id])

  const handleToggleActive = async () => {
    try {
      await updateCustomer(id, { isActive: !customer.isActive })
      fetchCustomer()
    } catch (err) {
      console.error('Failed to toggle status:', err)
    }
  }

  const handleDelete = async () => {
    if (!window.confirm(`Delete ${customer?.name}? This cannot be undone.`)) return
    try {
      await deleteCustomer(id)
      navigate('/customers')
    } catch (err) {
      console.error('Failed to delete customer:', err)
    }
  }

  if (loading)   return <LoadingSpinner />
  if (error)     return <ErrorMessage message={error} onRetry={fetchCustomer} />
  if (!customer) return null

  const initials = customer.name.split(' ').map((p) => p[0]).join('').toUpperCase()

  return (
    <div className="page-container">
      <div className="detail-header">
        <Link to="/customers" className="back-link">← Back</Link>
        <div className="detail-actions">
          <Link to={`/customers/${id}/edit`} className="btn-primary">Edit</Link>
          <button className="btn-danger" onClick={handleDelete}>Delete</button>
        </div>
      </div>

      <div className="detail-card">
        <div className="detail-avatar">{initials}</div>
        <h2 className="detail-name">{customer.name}</h2>
        <span
          className={`badge ${customer.isActive ? 'badge-active' : 'badge-inactive'}`}
          onClick={handleToggleActive}
          title="Click to toggle status"
        >
          {customer.isActive ? 'Active' : 'Inactive'}
        </span>

        <div className="detail-fields">
          <div className="detail-field">
            <span className="detail-label">Email</span>
            <span className="detail-value">{customer.email}</span>
          </div>
          <div className="detail-field">
            <span className="detail-label">Company</span>
            <span className="detail-value">{customer.company}</span>
          </div>
          <div className="detail-field">
            <span className="detail-label">Phone</span>
            <span className="detail-value">{customer.phone || '—'}</span>
          </div>
        </div>
      </div>
    </div>
  )
}

export default CustomerDetailPage
