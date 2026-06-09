import { createContext, useContext, useState } from 'react';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    try {
      const saved = localStorage.getItem('ce_user');
      return saved ? JSON.parse(saved) : null;
    } catch {
      return null;
    }
  });

  function login(userData, token) {
    localStorage.setItem('ce_token', token);
    localStorage.setItem('ce_user', JSON.stringify(userData));
    setUser(userData);
  }

  function logout() {
    localStorage.removeItem('ce_token');
    localStorage.removeItem('ce_user');
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
