import { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { labTestsApi } from '../api/labTests';
import { sampleCustodyApi } from '../api/sampleCustody';
import { donationBankApi } from '../api/donationBank';
import { CustodyEventType, CustodyEventTypeLabel, ScreeningStatusLabel } from '../api/enums';
import './ClinicalPages.css';

export default function LabPage() {
  const { user } = useAuth();
  const [queue, setQueue] = useState([]);
  const [error, setError] = useState(null);
  const [custodyId, setCustodyId] = useState('');
  const [custodyEvents, setCustodyEvents] = useState([]);
  const [custodyForm, setCustodyForm] = useState({
    SampleIdentifier: '',
    EventType: CustodyEventType.Received,
    DestinationRecipient: '',
    AdditionalNotes: '',
  });

  const reload = () => labTestsApi.getQueue().then(setQueue).catch((e) => setError(e.message));
  useEffect(() => { reload(); }, []);

  const loadCustody = async () => {
    if (!custodyId.trim()) return;
    try {
      setCustodyEvents(await sampleCustodyApi.getBySample(custodyId.trim()));
    } catch (err) {
      setError(err.message);
    }
  };

  const submitCustody = async () => {
    try {
      await sampleCustodyApi.create({
        TechnicianId: user.userId,
        SampleIdentifier: custodyForm.SampleIdentifier,
        EventType: Number(custodyForm.EventType),
        DestinationRecipient: custodyForm.DestinationRecipient || null,
        AdditionalNotes: custodyForm.AdditionalNotes || null,
      });
      setCustodyId(custodyForm.SampleIdentifier);
      setCustodyEvents(await sampleCustodyApi.getBySample(custodyForm.SampleIdentifier));
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="clinical-page">
      <div className="clinical-toolbar"><h2>Lab queue</h2><button type="button" className="btn btn-ghost" onClick={reload}>Refresh</button></div>
      {error && <div className="form-banner-error">{error}</div>}
      <section className="detail-panel">
        <h3>Pending orders ({queue.length})</h3>
        {queue.map((o) => <div key={o.id} className="list-row">Order {String(o.id).slice(0, 8)} — status {o.status}</div>)}
      </section>
      <section className="detail-panel">
        <h3>Chain of custody</h3>
        <div className="inline-form">
          <input value={custodyForm.SampleIdentifier} onChange={(e) => setCustodyForm((f) => ({ ...f, SampleIdentifier: e.target.value }))} placeholder="Sample ID" />
          <select value={custodyForm.EventType} onChange={(e) => setCustodyForm((f) => ({ ...f, EventType: e.target.value }))}>
            {Object.entries(CustodyEventTypeLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
          <button type="button" className="btn btn-primary" onClick={submitCustody}>Log event</button>
        </div>
        <div className="search-row" style={{ marginTop: '0.75rem' }}>
          <input value={custodyId} onChange={(e) => setCustodyId(e.target.value)} placeholder="Lookup sample…" />
          <button type="button" className="btn btn-ghost" onClick={loadCustody}>Load history</button>
        </div>
        {custodyEvents.map((ev) => <div key={ev.id} className="list-row">{CustodyEventTypeLabel[ev.eventType]}</div>)}
      </section>
    </div>
  );
}
