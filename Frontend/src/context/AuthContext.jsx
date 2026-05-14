import { createContext, useCallback, useContext, useMemo, useState } from 'react';
import { authApi } from '../api/auth';
import { getStoredToken, setStoredToken } from '../api/axiosClient';

const USER_KEY = 'ivf_auth_user';

function loadUser() {
  try {
    const raw = sessionStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(loadUser);
  const [token, setToken] = useState(getStoredToken);

  const login = useCallback(async (usernameOrEmail, password) => {
    const result = await authApi.login({ UsernameOrEmail: usernameOrEmail, Password: password });
    const session = {
      token: result.token,
      role: result.role,
      userId: result.userId,
      username: result.username,
      expiresAt: result.expiresAt,
    };
    setStoredToken(session.token);
    sessionStorage.setItem(USER_KEY, JSON.stringify(session));
    setToken(session.token);
    setUser(session);
    return session;
  }, []);

  const logout = useCallback(() => {
    setStoredToken(null);
    sessionStorage.removeItem(USER_KEY);
    setToken(null);
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({ user, token, role: user?.role ?? null, login, logout, isAuthenticated: Boolean(token) }),
    [user, token, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
