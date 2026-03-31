import React from "react";

const CustomerCard = ({ name, email, company, isActive }) => {
  return (
    <div className="customer-card">
      <h2>{name}</h2>
      <p>{email}</p>
      <p>{company}</p>
      <p>{isActive ? "Active" : "Inactive"}</p>
    </div>
  );
};

export default CustomerCard;