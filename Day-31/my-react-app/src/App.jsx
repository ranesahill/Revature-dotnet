import React from "react";
import CustomerList from "./CustomerList";

function App() {
  const customers = [
    { id: 1, name: "Jane Doe", email: "jane.doe@acme.com", company: "Acme Corp", isActive: true },
    { id: 2, name: "Bob Wilson", email: "bob@globex.com", company: "Globex Inc", isActive: true },
    { id: 3, name: "Sara Chen", email: "s.chen@initech.com", company: "Initech", isActive: false },
    { id: 4, name: "Mike Ross", email: "mike@piedpiper.com", company: "Pied Piper", isActive: true },
    { id: 5, name: "Anna Bell", email: "anna@hooli.com", company: "Hooli", isActive: true },
  ];

  return (
    <div className="app">
      <header className="app-header">
        <h1>Customers</h1>
      </header>

      <main className="app-main">
        <CustomerList list={customers} />
      </main>
    </div>
  );
}

export default App;