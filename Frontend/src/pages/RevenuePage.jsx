import { useEffect, useState } from 'react';
import { billingApi } from '../api/billing';
import { serviceCatalogApi } from '../api/serviceCatalog';
import { BillStatusLabel } from '../api/enums';
import './ClinicalPages.css';

export default function RevenuePage() {
  const [services, setServices] = useState([]);
  const [bills, setBills] = useState([]);
  const [error, setError] = useState(null);
  const [patientId, setPatientId] = useState('');

  useEffect(() => {
    serviceCatalogApi.getAll().then(setServices).catch((e) => setError(e.message));
  }, []);

  const loadBills = async () => {
    if (!patientId.trim()) return;
    try {
      setBills(await billingApi.getByPatient(patientId.trim()));
    } catch (err) {
      setError(err.message);
    }
  };

  const openBill = async (id) => {
    try {
      const bill = await billingApi.getById(id);
      alert(`Invoice ${bill.invoiceNumber}\nTotal: ${bill.totalDue}\nStatus: ${BillStatusLabel[bill.status]}`);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="clinical-page">
      <h2>Revenue &amp; billing</h2>
      {error && <div className="form-banner-error">{error}</div>}

      <section className="detail-panel">
        <h3>Service catalog ({services.length})</h3>
        {services.map((s) => (
          <div key={s.id} className="list-row">{s.name} — {s.price}</div>
        ))}
      </section>

      <section className="detail-panel">
        <h3>Patient bills</h3>
        <div className="search-row">
          <input value={patientId} onChange={(e) => setPatientId(e.target.value)} placeholder="Patient GUID" />
          <button type="button" className="btn btn-primary" onClick={loadBills}>Load bills</button>
        </div>
        {bills.map((b) => (
          <button key={b.id} type="button" className="card-item" onClick={() => openBill(b.id)}>
            {b.invoiceNumber} — {BillStatusLabel[b.status]} — {b.totalDue}
          </button>
        ))}
      </section>
    </div>
  );
}
