import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { notificationsApi } from '../../api/notifications';
import './TopBar.css';

export default function TopBar({ title, subtitle, dark, onToggleDark }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [unread, setUnread] = useState([]);

  useEffect(() => {
    if (!user?.userId) return;
    notificationsApi.getUnread(user.userId).then(setUnread).catch(() => setUnread([]));
  }, [user]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const markRead = async (id) => {
    await notificationsApi.markRead(id);
    setUnread((list) => list.filter((n) => n.id !== id));
  };

  return (
    <header className="topbar">
      <span className="topbar-titles" style={{ display: 'flex', flexDirection: 'column' }}>
        <h1 className="topbar-title">{title}</h1>
        {subtitle && <p className="topbar-subtitle">{subtitle}</p>}
      </span>

      <span className="topbar-actions" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
        {unread.length > 0 && (
          <span className="notif-wrap">
            <span className="notif-badge">{unread.length}</span>
            <span className="notif-dropdown">
              {unread.map((n) => (
                <button key={n.id} type="button" className="notif-item" onClick={() => markRead(n.id)}>
                  {n.message ?? n.title ?? 'Notification'}
                </button>
              ))}
            </span>
          </span>
        )}
        {user && <span className="topbar-user">{user.username} · {user.role}</span>}
        <button type="button" className="btn btn-ghost" onClick={handleLogout}>Logout</button>
        <button className="theme-toggle" onClick={onToggleDark} title="Toggle theme" aria-label="Toggle theme">
          {dark ? (
            <svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="5"/></svg>
          ) : (
            <svg viewBox="0 0 24 24"><path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/></svg>
          )}
        </button>
      </span>
    </header>
  );
}
