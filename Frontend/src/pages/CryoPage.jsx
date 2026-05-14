import { useEffect, useState } from 'react';
import { donationBankApi } from '../api/donationBank';
import { ScreeningStatusLabel } from '../api/enums';
import './ClinicalPages.css';

export default function CryoPage() {
  const [samples, setSamples] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    donationBankApi.getAssignable().then(setSamples).catch((e) => setError(e.message));
  }, []);

  return (
    <div className="clinical-page">
      <h2>Cryo &amp; donation bank</h2>
      {error && <div className="form-banner-error">{error}</div>}
      <p className="demo-banner">Tank nitrogen levels in cycle view are demo-only. Assignable samples below are live API data.</p>
      <section className="detail-panel">
        <h3>Assignable samples ({samples.length})</h3>
        {samples.length === 0 ? (
          <p className="muted">No assignable samples</p>
        ) : (
          samples.map((s) => (
            <div key={s.id} className="list-row">
              {s.sampleType} — donor {String(s.donorId).slice(0, 8)} — {ScreeningStatusLabel[s.screeningStatus]}
            </div>
          ))
        )}
      </section>
    </div>
  );
}
