// src/hooks/useCustomers.js
// Custom hook — encapsulates fetch-all-customers logic.
// Any page that needs the full customer list can call this instead of
// duplicating the useEffect + loading + error boilerplate.

import { useState, useEffect } from 'react'
import { getCustomers } from '../api/customersApi'

const useCustomers = () => {
  const [customers, setCustomers] = useState([])
  const [loading, setLoading]     = useState(true)
  const [error, setError]         = useState(null)

  const fetchData = async () => {
    try {
      setLoading(true)
      setError(null)
      const response = await getCustomers()
      setCustomers(response.data)
    } catch (err) {
      setError('Failed to load customers. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  // NEVER DO THIS
  // useEffect(() => {
  //   fetchData()
  // })


  // Executes exactly ones
  useEffect(() => {
    fetchData()
  }, [])


  // useEffect(() => {
  //   // error handling
  // }, [error])

  // Expose refetch so pages can re-trigger after mutations
  return { customers, loading, error, refetch: fetchData }
}

export default useCustomers
