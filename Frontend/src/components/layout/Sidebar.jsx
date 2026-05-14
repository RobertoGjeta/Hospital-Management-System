import { NavLink } from 'react-router-dom';
import RoleGate from '../auth/RoleGate';
import './Sidebar.css';

const navItems = [
  { to: '/patient', label: 'Patient', roles: ['Administrator', 'Doctor', 'Patient'], icon: 'patient' },
  { to: '/cycle', label: 'Cycle', roles: ['Administrator', 'Doctor'], icon: 'cycle' },
  { to: '/lab', label: 'Lab', roles: ['Administrator', 'Doctor', 'LabTechnician'], icon: 'lab' },
  { to: '/cryo', label: 'Cryo', roles: ['Administrator', 'LabTechnician'], icon: 'cryo' },
  { to: '/revenue', label: 'Revenue', roles: ['Administrator', 'Doctor', 'Patient'], icon: 'revenue' },
  { to: '/admin/patients', label: 'Admin: Patients', roles: ['Administrator'], icon: 'admin' },
  { to: '/admin/doctors', label: 'Admin: Doctors', roles: ['Administrator'], icon: 'admin' },
  { to: '/admin/lab-technicians', label: 'Admin: Lab', roles: ['Administrator'], icon: 'admin' },
  { to: '/admin/administrators', label: 'Admin: Staff', roles: ['Administrator'], icon: 'admin' },
  { to: '/admin/rooms', label: 'Admin: Rooms', roles: ['Administrator'], icon: 'admin' },
];

function Icon({ type }) {
  if (type === 'cycle') return <svg viewBox="0 0 24 24"><polyline points="2,12 6,6 10,14 14,9 18,15 22,10"/></svg>;
  if (type === 'lab') return <svg viewBox="0 0 24 24"><path d="M9 3h6m-3 0v6l4 8H8l4-8V3z"/></svg>;
  if (type === 'cryo') return <svg viewBox="0 0 24 24"><line x1="12" y1="2" x2="12" y2="22"/></svg>;
  if (type === 'revenue') return <svg viewBox="0 0 24 24"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>;
  if (type === 'admin') return <svg viewBox="0 0 24 24"><path d="M12 2l8 4v6c0 5-3.5 9-8 10-4.5-1-8-5-8-10V6l8-4z"/></svg>;
  return <svg viewBox="0 0 24 24"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 3.6-7 8-7s8 3 8 7"/></svg>;
}

export default function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="sidebar-brand" title="IVF Management">
        <svg viewBox="0 0 24 24" aria-hidden="true">
          <path d="M12 21s-7-4.5-7-10a4 4 0 0 1 7-2.6A4 4 0 0 1 19 11c0 5.5-7 10-7 10z" />
        </svg>
      </div>
      <nav className="sidebar-nav">
        {navItems.map((item) => (
          <RoleGate key={item.to} roles={item.roles}>
            <NavLink
              to={item.to}
              className={({ isActive }) => `sidebar-item${isActive ? ' active' : ''}`}
            >
              <span className="sidebar-icon"><Icon type={item.icon} /></span>
              <span className="sidebar-label">{item.label}</span>
            </NavLink>
          </RoleGate>
        ))}
      </nav>
    </aside>
  );
}
