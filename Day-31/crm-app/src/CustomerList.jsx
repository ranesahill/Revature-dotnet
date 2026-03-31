import CustomerCard from './CustomerCard'

const CustomerList = ({ customers, toggleIsActive }) => {
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
          toggleIsActive={toggleIsActive}
        />
      ))}
    </div>
  )
}

export default CustomerList
