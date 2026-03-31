import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import CustomerForm from '../components/forms/CustomerForm'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import { getCustomer, updateCustomer } from '../api/customersApi'

const EditCustomerPage = () => {
  const { id } = useParams()
  const navigate = useNavigate()

  const [customer, setCustomer]       = useState(null)
  const [loading, setLoading]         = useState(true)
  const [error, setError]             = useState(null)
  const [submitError, setSubmitError] = useState(null)

  useEffect(() => {
    const fetchCustomer = async () => {
      try {
        setLoading(true)
        setError(null)
        const response = await getCustomer(id)
        setCustomer(response.data)
      } catch (err) {
        setError('Failed to load customer.')
      } finally {
        setLoading(false)
      }
    }
    fetchCustomer()
  }, [id])

  const handleSubmit = async (formData) => {
    try {
      setSubmitError(null)
      await updateCustomer(id, formData)
      navigate(`/customers/${id}`)
    } catch (err) {
      setSubmitError('Failed to update customer. Please try again.')
    }
  }

  if (loading) return <LoadingSpinner />
  if (error)   return <ErrorMessage message={error} />

  return (
    <div className="page-container">
      <div className="page-header">
        <h1 className="page-title">Edit Customer</h1>
      </div>
      {submitError && <p className="submit-error">{submitError}</p>}
      <CustomerForm
        initialData={customer}
        onSubmit={handleSubmit}
        onCancel={() => navigate(-1)}
      />
    </div>
  )
}

export default EditCustomerPage
