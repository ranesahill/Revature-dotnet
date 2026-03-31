import { useState } from 'react'
import { Link } from 'react-router-dom'
import CustomerList from '../components/CustomerList'
import SearchBar from '../components/SearchBar'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import useCustomers from '../hooks/useCustomers'
import { updateCustomer, deleteCustomer } from '../api/customersApi'

const CustomersPage = () => {
  const [searchTerm, setSearchTerm] = useState('')
  const { customers, loading, error, refetch } = useCustomers()

  const filteredCustomers = customers.filter(
    (c) =>
      c.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      c.company.toLowerCase().includes(searchTerm.toLowerCase())
  )

  const handleToggleActive = async (id, currentStatus) => {
    try {
      await updateCustomer(id, { isActive: !currentStatus })
      refetch()
    } catch (err) {
      console.error('Failed to toggle status:', err)
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this customer? This cannot be undone.')) return
    try {
      await deleteCustomer(id)
      refetch()
    } catch (err) {
      console.error('Failed to delete customer:', err)
    }
  }

  if (loading) return <LoadingSpinner />
  if (error)   return <ErrorMessage message={error} onRetry={refetch} />

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">Customers</h1>
          <p className="page-subtitle">
            {filteredCustomers.length} of {customers.length} customers
          </p>
        </div>
        <Link to="/customers/new" className="btn-primary">+ Add Customer</Link>
      </div>
      <SearchBar searchTerm={searchTerm} onSearchChange={setSearchTerm} />
      <CustomerList
        customers={filteredCustomers}
        onToggleActive={handleToggleActive}
        onDelete={handleDelete}
      />
    </div>
  )
}

export default CustomersPage
