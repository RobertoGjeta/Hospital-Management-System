import { useCallback, useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { patientsApi } from '../api/patients';
import { medicalRecordsApi } from '../api/medicalRecords';
import { appointmentsApi } from '../api/appointments';
import { labTestsApi } from '../api/labTests';
import { RecordEntryType, RecordEntryTypeLabel } from '../api/enums';
import './ClinicalPages.css';

export default function PatientPage() {
  const { user, role } = useAuth();
  const [query, setQuery] = useState('');
  const [patients, setPatients] = useState([]);
  const [selected, setSelected] = useState(null);
  const [records, setRecords] = useState([]);
  const [appts, setAppts] = useState([]);
  const [reports, setReports] = useState([]);
  const [error, setError] = useState(null);
  const [note, setNote] = useState({ EntryType: RecordEntryType.ClinicalNote, Content: '' });

  const loadPatientDetail = useCallback(async (patient) => {
    setSelected(patient);
    setError(null);
    try {
      const [recs, apts, reps] = await Promise.all([
        role === 'Doctor' ? medicalRecordsApi.getByPatient(patient.id) : Promise.resolve([]),
        appointmentsApi.getByPatient(patient.id),
        labTestsApi.getReleasedForPatient(patient.id),
      ]);
      setRecords(recs);
      setAppts(apts);
      setReports(reps);
    } catch (err) {
      setError(err.message);
    }
  }, [role]);

  useEffect(() => {
    if (role === 'Patient' && user?.userId) {
      patientsApi.getById(user.userId).then(loadPatientDetail).catch((e) => setError(e.message));
    }
  }, [role, user, loadPatientDetail]);

  const search = async () => {
    setError(null);
    try {
      const data = await patientsApi.search({ Name: query });
      setPatients(data);
    } catch (err) {
      setError(err.message);
    }
  };

  const saveNote = async () => {
    if (!selected) return;
    try {
      await medicalRecordsApi.create({
        PatientId: selected.id,
        AuthoringDoctorId: user.userId,
        EntryType: Number(note.EntryType),
        Content: note.Content,
      });
      const recs = await medicalRecordsApi.getByPatient(selected.id);
      setRecords(recs);
      setNote({ EntryType: RecordEntryType.ClinicalNote, Content: '' });
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="clinical-page">
      <div className="clinical-toolbar">
        <h2>Patient workspace</h2>
        {role !== 'Patient' && (
          <div className="search-row">
            <input value={query} onChange={(e) => setQuery(e.target.value)} placeholder="Search patients…" />
            <button type="button" className="btn btn-primary" onClick={search}>Search</button>
          </div>
        )}
      </div>

      {error && <div className="form-banner-error">{error}</div>}

      {role !== 'Patient' && patients.length > 0 && (
        <div className="card-list">
          {patients.map((p) => (
            <button key={p.id} type="button" className="card-item" onClick={() => loadPatientDetail(p)}>
              <strong>{p.firstName} {p.lastName}</strong>
              <span>{p.patientSystemId}</span>
            </button>
          ))}
        </div>
      )}

      {selected && (
        <div className="detail-grid">
          <section className="detail-panel">
            <h3>Profile</h3>
            <p><strong>{selected.firstName} {selected.lastName}</strong></p>
            <p>{selected.email}</p>
            <p>ID: {selected.patientSystemId}</p>
            <p>Phone: {selected.phoneNumber ?? '—'}</p>
          </section>

          <section className="detail-panel">
            <h3>Appointments</h3>
            {appts.length === 0 ? <p className="muted">No appointments</p> : appts.map((a) => (
              <div key={a.id} className="list-row">{new Date(a.startsAt).toLocaleString()} — status {a.status}</div>
            ))}
          </section>

          {role === 'Doctor' && (
            <section className="detail-panel">
              <h3>Medical records</h3>
              {records.map((r) => (
                <div key={r.id} className="list-row">
                  <strong>{RecordEntryTypeLabel[r.entryType]}</strong>: {r.content}
                </div>
              ))}
              <div className="inline-form">
                <select value={note.EntryType} onChange={(e) => setNote((n) => ({ ...n, EntryType: e.target.value }))}>
                  {Object.entries(RecordEntryTypeLabel).map(([v, l]) => (
                    <option key={v} value={v}>{l}</option>
                  ))}
                </select>
                <textarea value={note.Content} onChange={(e) => setNote((n) => ({ ...n, Content: e.target.value }))} rows={2} placeholder="Clinical note…" />
                <button type="button" className="btn btn-primary" onClick={saveNote}>Add note</button>
              </div>
            </section>
          )}

          <section className="detail-panel">
            <h3>Released lab results</h3>
            {reports.length === 0 ? <p className="muted">No released reports</p> : reports.map((r) => (
              <div key={r.id} className="list-row">
                Report {String(r.id).slice(0, 8)}… {r.isAbnormal ? 'Abnormal' : 'Normal'}
              </div>
            ))}
          </section>
        </div>
      )}
    </div>
  );
}
