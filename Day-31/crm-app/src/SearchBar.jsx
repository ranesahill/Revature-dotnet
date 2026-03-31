import React from 'react'

// const SearchBar = (prop) => {
const SearchBar = ({ onSearchChange }) => {

    const onSearchClear = () => {
        onSearchChange('');
    }

    return (<div className="search-bar">
        <span className="search-icon">🔍</span>
        <input
            type="text"
            className="search-input"
            placeholder="Search by name or company..."
            onChange={(e) => { onSearchChange(e.target.value) }}
        />
        <button onClick={onSearchClear}>Clear</button>
    </div>
    );


}
export default SearchBar