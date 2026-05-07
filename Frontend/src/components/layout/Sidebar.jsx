import { NavLink } from 'react-router-dom';
import './Sidebar.css';

const navItems = [
  {
    to: '/patient',
    label: 'Patient',
    icon: (
      <svg viewBox="0 0 24 24"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 3.6-7 8-7s8 3 8 7"/></svg>
    ),
  },
  {
    to: '/cycle',
    label: 'Cycle',
    icon: (
      <svg viewBox="0 0 24 24"><polyline points="2,12 6,6 10,14 14,9 18,15 22,10"/></svg>
    ),
  },
  {
    to: '/lab',
    label: 'Lab / Embryology',
    icon: (
      <svg viewBox="0 0 24 24"><path d="M9 3h6m-3 0v6l4 8H8l4-8V3z"/><line x1="6" y1="17" x2="18" y2="17"/></svg>
    ),
  },
  {
    to: '/cryo',
    label: 'Cryo-Store',
    icon: (
      <svg viewBox="0 0 24 24"><line x1="12" y1="2" x2="12" y2="22"/><path d="M4.93 6.93L19.07 17.07M19.07 6.93L4.93 17.07"/><path d="M2 12h20"/><path d="M9 5l3-3 3 3M9 19l3 3 3-3M5 9l-3 3 3 3M19 9l3 3-3 3"/></svg>
    ),
  },
  {
    to: '/revenue',
    label: 'Revenue',
    icon: (
      <svg viewBox="0 0 24 24"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>
    ),
  },
];

export default function Sidebar() {
  return (
    <aside className="sidebar">
      <nav className="sidebar-nav">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) => `sidebar-item${isActive ? ' active' : ''}`}
          >
            <span className="sidebar-icon">{item.icon}</span>
            <span className="sidebar-label">{item.label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
