import { useState } from 'react'
import CustomerFormFields from './CustomerFormFields'

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

const CustomerForm = ({ initialData = null, onSubmit, onCancel }) => {
  const isEditing = initialData !== null
  const [formData, setFormData] = useState({
    name: initialData?.name ?? '', email: initialData?.email ?? '',
    company: initialData?.company ?? '', phone: initialData?.phone ?? '',
  })
  const [errors, setErrors] = useState({})

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }))
  }

  const validate = () => {
    const newErrors = {}
    if (!formData.name.trim())    newErrors.name    = 'Name is required'
    if (!formData.email.trim())   newErrors.email   = 'Email is required'
    else if (!EMAIL_REGEX.test(formData.email)) newErrors.email = 'Please enter a valid email address'
    if (!formData.company.trim()) newErrors.company = 'Company is required'
    return newErrors
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    const validationErrors = validate()
    if (Object.keys(validationErrors).length > 0) { setErrors(validationErrors); return }
    onSubmit(formData)
  }

  return (
    <div className="form-container">
      <div className="form-header">
        <h2 className="form-title">{isEditing ? 'Edit Customer' : 'Add Customer'}</h2>
        <p className="form-subtitle">{isEditing ? 'Update the customer details below.' : 'Fill in the details to add a new customer.'}</p>
      </div>
      <form onSubmit={handleSubmit} noValidate>
        <CustomerFormFields formData={formData} errors={errors} onChange={handleChange} />
        <div className="form-actions">
          <button type="button" className="btn-cancel" onClick={onCancel}>Cancel</button>
          <button type="submit" className="btn-submit">{isEditing ? 'Save Changes' : 'Add Customer'}</button>
        </div>
      </form>
    </div>
  )
}

export default CustomerForm
