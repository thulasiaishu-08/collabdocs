import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';

const QUICK_USERS = [
  { username: 'alice', password: 'password1', color: '#6366f1' },
  { username: 'bob',   password: 'password2', color: '#10b981' },
];

export default function Login() {
  const { login } = useAuth();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError]       = useState('');
  const [loading, setLoading]   = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const data = await api.login(username.trim(), password);
      login({ id: data.userId, username: data.username }, data.token);
    } catch (err) {
      setError(err.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  }

  async function quickLogin(u) {
    setError('');
    setLoading(true);
    try {
      const data = await api.login(u.username, u.password);
      login({ id: data.userId, username: data.username }, data.token);
    } catch (err) {
      setError(err.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-logo">📝</div>
        <h1 className="login-title">CollabDocs</h1>
        <p className="login-subtitle">Collaborative document editing</p>

        <div className="quick-login">
          <p className="quick-login-label">Quick login (demo)</p>
          <div className="quick-login-buttons">
            {QUICK_USERS.map(u => (
              <button
                key={u.username}
                className="quick-btn"
                style={{ '--accent': u.color }}
                onClick={() => quickLogin(u)}
                disabled={loading}
              >
                <span className="quick-avatar">{u.username[0].toUpperCase()}</span>
                {u.username}
              </button>
            ))}
          </div>
        </div>

        <div className="login-divider"><span>or sign in manually</span></div>

        <form onSubmit={handleSubmit} className="login-form">
          <div className="field">
            <label htmlFor="username">Username</label>
            <input
              id="username"
              type="text"
              value={username}
              onChange={e => setUsername(e.target.value)}
              placeholder="alice or bob"
              autoComplete="username"
              required
            />
          </div>
          <div className="field">
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              placeholder="password1 or password2"
              autoComplete="current-password"
              required
            />
          </div>
          {error && <p className="form-error">{error}</p>}
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Signing in…' : 'Sign in'}
          </button>
        </form>
      </div>
    </div>
  );
}
