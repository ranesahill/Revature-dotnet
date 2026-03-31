import CustomerCard from './CustomerCard'

const CustomerList = ({ customers, onToggleActive, onDelete }) => {
  if (customers.length === 0) return <p className="empty-state">No customers match your search.</p>

  return (
    <div className="customer-list">
      {customers.map((customer) => (
        <CustomerCard
          key={customer.id}
          id={customer.id}
          name={customer.name}
          email={customer.email}
          company={customer.company}
          isActive={customer.isActive}
          onToggleActive={() => onToggleActive(customer.id, customer.isActive)}
          onDelete={() => onDelete(customer.id)}
        />
      ))}
    </div>
  )
}

export default CustomerList
