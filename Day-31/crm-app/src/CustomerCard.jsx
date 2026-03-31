const CustomerCard = ({ id, name, email, company, isActive, toggleIsActive }) => {
    console.log(`### Id : ${id}`)
    const initials = name
        .split(' ')
        .map((part) => part[0])
        .join('')
        .toUpperCase()

    return (
        <div className="customer-card focus-visible">
            <div className="customer-avatar">{initials}</div>

            <div className="customer-info">
                <h3 className="customer-name">{id}</h3>
                <h3 className="customer-name">{name}</h3>
                <p className="customer-email">{email}</p>
                <p className="customer-company">{company}</p>
            </div>

            <span
                className={`badge ${isActive ? 'badge-active' : 'badge-inactive'}`}
                onClick={() => toggleIsActive(id)}>
                {isActive ? 'Active' : 'Inactive'}
            </span>
        </div>
    )
}

export default CustomerCard
