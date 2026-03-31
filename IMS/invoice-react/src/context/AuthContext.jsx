import { createContext, useContext, useState, useEffect } from 'react';
import { getStoredUser, getStoredToken, storeAuth, logout as authLogout } from '../services/authService';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(getStoredUser());
  const [token, setToken] = useState(getStoredToken());

  const login = (data) => {
    storeAuth(data);
    setUser({ username: data.username, role: data.role });
    setToken(data.token);
  };

  const logout = () => {
    authLogout();
    setUser(null);
    setToken(null);
  };

  const isAuthenticated = !!token;
  const hasRole = (...roles) => user && roles.includes(user.role);

  return (
    <AuthContext.Provider value={{ user, token, login, logout, isAuthenticated, hasRole }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
