const CustomerFormFields = ({ formData, errors, onChange }) => (
  <div className="form-fields">
    <div className="form-field">
      <label htmlFor="name" className="form-label">Full Name <span className="required">*</span></label>
      <input id="name" name="name" type="text" className={`form-input ${errors.name ? 'input-error' : ''}`}
        value={formData.name} onChange={onChange} placeholder="e.g. Jane Doe" />
      {errors.name && <span className="error-message">⚠ {errors.name}</span>}
    </div>
    <div className="form-field">
      <label htmlFor="email" className="form-label">Email Address <span className="required">*</span></label>
      <input id="email" name="email" type="email" className={`form-input ${errors.email ? 'input-error' : ''}`}
        value={formData.email} onChange={onChange} placeholder="e.g. jane@acme.com" />
      {errors.email && <span className="error-message">⚠ {errors.email}</span>}
    </div>
    <div className="form-field">
      <label htmlFor="company" className="form-label">Company <span className="required">*</span></label>
      <input id="company" name="company" type="text" className={`form-input ${errors.company ? 'input-error' : ''}`}
        value={formData.company} onChange={onChange} placeholder="e.g. Acme Corp" />
      {errors.company && <span className="error-message">⚠ {errors.company}</span>}
    </div>
    <div className="form-field">
      <label htmlFor="phone" className="form-label">Phone <span className="optional">(optional)</span></label>
      <input id="phone" name="phone" type="tel" className="form-input"
        value={formData.phone} onChange={onChange} placeholder="e.g. 415-555-0100" />
    </div>
  </div>
)

export default CustomerFormFields
