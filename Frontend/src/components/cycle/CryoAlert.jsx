import './CryoAlert.css';

export default function CryoAlert({ tanks }) {
  return (
    <div className="cryo-alert">
      <div className="cryo-alert-message">
        <span className="cryo-alert-icon">
          <svg viewBox="0 0 24 24"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
        </span>
        <span className="cryo-alert-text">Cryo-Storage Alert: Action Required</span>
      </div>

      <div className="cryo-tanks">
        {tanks.map((tank) => (
          <div key={tank.id} className="tank-chip">
            <span className={tank.status === 'low' ? 'tank-icon-low' : 'tank-icon-ok'}>
              <svg viewBox="0 0 24 24"><line x1="12" y1="2" x2="12" y2="22"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>
            </span>
            <span>{tank.id}</span>
            <span className="tank-level">{tank.level}% N₂</span>
            {tank.status === 'low' && <span className="tank-badge-low">Low</span>}
          </div>
        ))}
      </div>

      <button className="cryo-view-btn">View Details</button>
    </div>
  );
}
