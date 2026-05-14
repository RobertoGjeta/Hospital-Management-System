import { useCallback, useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { patientsApi } from '../api/patients';
import { ivfCyclesApi } from '../api/ivfCycles';
import { embryosApi } from '../api/embryos';
import PatientHeader from '../components/cycle/PatientHeader';
import CryoAlert from '../components/cycle/CryoAlert';
import { CyclePhaseLabel } from '../api/enums';
import { mockStimulationData, mockPrognosis, mockTanks } from '../data/mockCycleData';
import './ClinicalPages.css';

function ageFromDob(dob) {
  if (!dob) return '—';
  const d = new Date(dob);
  const now = new Date();
  let age = now.getFullYear() - d.getFullYear();
  if (now < new Date(now.getFullYear(), d.getMonth(), d.getDate())) age -= 1;
  return age;
}

export default function CycleDashboard({ dark, onToggleDark }) {
  const { user } = useAuth();
  const [patients, setPatients] = useState([]);
  const [selectedPatientId, setSelectedPatientId] = useState('');
  const [patient, setPatient] = useState(null);
  const [cycle, setCycle] = useState(null);
  const [embryos, setEmbryos] = useState([]);
  const [error, setError] = useState(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    if (!user?.userId) return;
    patientsApi.getByDoctor(user.userId).then(setPatients).catch((e) => setError(e.message));
  }, [user]);

  const loadCycle = useCallback(async (patientId) => {
    if (!patientId) return;
    setError(null);
    try {
      const p = await patientsApi.getById(patientId);
      setPatient(p);
      const cycles = await ivfCyclesApi.getByPatient(patientId);
      const active = cycles.find((c) => c.isActive) ?? cycles[0] ?? null;
      setCycle(active);
      if (active) {
        const emb = await embryosApi.getByCycle(active.id);
        setEmbryos(emb);
      } else {
        setEmbryos([]);
      }
    } catch (err) {
      setError(err.message);
    }
  }, []);

  useEffect(() => {
    if (selectedPatientId) loadCycle(selectedPatientId);
  }, [selectedPatientId, loadCycle]);

  const startCycle = async () => {
    if (!selectedPatientId || !user?.userId) return;
    setBusy(true);
    try {
      await ivfCyclesApi.create({ PatientId: selectedPatientId, DoctorId: user.userId });
      await loadCycle(selectedPatientId);
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  };

  const advancePhase = async () => {
    if (!cycle || !user?.userId) return;
    setBusy(true);
    try {
      const updated = await ivfCyclesApi.advancePhase(cycle.id, user.userId, { Justification: 'Phase advance from dashboard' });
      setCycle(updated);
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  };

  const headerPatient = patient
    ? {
        name: `${patient.firstName} ${patient.lastName}`,
        patientSystemId: patient.patientSystemId,
        age: ageFromDob(patient.dateOfBirth),
        cycleDay: cycle ? CyclePhaseLabel[cycle.currentPhase] : '—',
        protocol: cycle?.outcome ?? 'Active cycle',
        viralStatus: '—',
      }
    : null;

  return (
    <div className="cycle-layout">
      <div className="clinical-toolbar">
        <select value={selectedPatientId} onChange={(e) => setSelectedPatientId(e.target.value)}>
          <option value="">Select patient…</option>
          {patients.map((p) => (
            <option key={p.id} value={p.id}>{p.firstName} {p.lastName} ({p.patientSystemId})</option>
          ))}
        </select>
        <div className="search-row">
          <button type="button" className="btn btn-primary" onClick={startCycle} disabled={!selectedPatientId || busy}>New cycle</button>
          <button type="button" className="btn btn-ghost" onClick={advancePhase} disabled={!cycle || busy}>Advance phase</button>
        </div>
      </div>

      {error && <div className="form-banner-error">{error}</div>}
      {headerPatient && <PatientHeader patient={headerPatient} dark={dark} onToggleDark={onToggleDark} />}

      {cycle && (
        <div className="phase-bar">
          {Object.entries(CyclePhaseLabel).map(([value, label]) => (
            <span key={value} className={`phase-chip${Number(value) === cycle.currentPhase ? ' active' : ''}`}>{label}</span>
          ))}
        </div>
      )}

      <section className="detail-panel">
        <h3>Embryos {cycle ? `(${embryos.length})` : ''}</h3>
        {embryos.length === 0 ? <p className="muted">No embryos for this cycle</p> : embryos.map((e) => (
          <div key={e.id} className="list-row">{e.identifier} — status {e.status}</div>
        ))}
      </section>

      <p className="demo-banner">Demo data below — stimulation chart and prognosis have no backend API yet.</p>
      <CryoAlert tanks={mockTanks} />
      <section className="detail-panel">
        <h3>Stimulation (demo)</h3>
        <p className="muted">Latest E2: {mockStimulationData.at(-1)?.estradiol} pg/mL — {mockPrognosis.ohss.message}</p>
      </section>
    </div>
  );
}
