import './PatientHeader.css';

export default function PatientHeader({ patient, dark, onToggleDark }) {
  return (
    <header className="patient-header">
      <div className="patient-avatar">
        <svg viewBox="0 0 24 24"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 3.6-7 8-7s8 3 8 7"/></svg>
      </div>

      <div className="patient-identity">
        <span className="patient-name">{patient.name}</span>
        <span className="patient-id">Patient ID: {patient.patientSystemId}</span>
      </div>

      <div className="header-divider" />

      <div className="header-chips">
        <div className="header-chip">
          <span className="chip-label">Age</span>
          <span className="chip-value">{patient.age} years</span>
        </div>

        <div className="header-divider" />

        <div className="header-chip">
          <span className="chip-label">Cycle Day</span>
          <span className="chip-value">Day {patient.cycleDay}</span>
        </div>

        <div className="header-divider" />

        <div className="header-chip">
          <span className="chip-label">Current Protocol</span>
          <span className="chip-value">{patient.protocol}</span>
        </div>

        <div className="header-divider" />

        <div className="header-chip">
          <span className="chip-label">Viral Status</span>
          <span className={`chip-value ${patient.viralStatus === 'Negative' ? 'viral-negative' : ''}`}>
            {patient.viralStatus}
          </span>
        </div>
      </div>

      <div className="header-actions">
        <button className="theme-toggle" onClick={onToggleDark} title="Toggle theme">
          {dark ? (
            <svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="5"/><line x1="12" y1="1" x2="12" y2="3"/><line x1="12" y1="21" x2="12" y2="23"/><line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/><line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/><line x1="1" y1="12" x2="3" y2="12"/><line x1="21" y1="12" x2="23" y2="12"/><line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/><line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/></svg>
          ) : (
            <svg viewBox="0 0 24 24"><path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/></svg>
          )}
        </button>
      </div>
    </header>
  );
}
