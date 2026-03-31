import React from "react";
import CustomerCard from "./CustomerCard";

const CustomerList = ({ list }) => {
  return (
    <>
      <div>Customer Count : {list.length}</div>

      {list.map((customer) => (
        <CustomerCard key={customer.id} {...customer} />
      ))}
    </>
  );
};

export default CustomerList;