const SearchBar = ({ searchTerm, onSearchChange }) => (
  <div className="search-bar">
    <span className="search-icon">🔍</span>
    <input type="text" className="search-input" placeholder="Search by name or company..."
      value={searchTerm} onChange={(e) => onSearchChange(e.target.value)} />
    {searchTerm && (
      <button className="search-clear" onClick={() => onSearchChange('')} aria-label="Clear search">✕</button>
    )}
  </div>
)

export default SearchBar
