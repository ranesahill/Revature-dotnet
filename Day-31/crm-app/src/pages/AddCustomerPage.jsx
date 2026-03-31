import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import CustomerForm from '../components/forms/CustomerForm'
import { createCustomer } from '../api/customersApi'

const AddCustomerPage = () => {
  const navigate = useNavigate()
  const [submitError, setSubmitError] = useState(null)

  const handleSubmit = async (formData) => {
    try {
      setSubmitError(null)
      await createCustomer(formData)
      navigate('/customers')
    } catch (err) {
      setSubmitError('Failed to create customer. Please try again.')
    }
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <h1 className="page-title">Add Customer</h1>
      </div>
      {submitError && <p className="submit-error">{submitError}</p>}
      <CustomerForm
        initialData={null}
        onSubmit={handleSubmit}
        onCancel={() => navigate(-1)}
      />
    </div>
  )
}

export default AddCustomerPage
