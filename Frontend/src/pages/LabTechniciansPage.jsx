import ResourcePage from '../components/common/ResourcePage';
import { labTechniciansApi } from '../api/labTechnicians';

const columns = [
  {
    key: 'name',
    label: 'Name',
    render: (t) => (
      <div className="cell-stack">
        <span className="cell-strong">{t.firstName} {t.lastName}</span>
        <span className="cell-muted">{t.email}</span>
      </div>
    ),
  },
  { key: 'technicianId', label: 'Technician ID', render: (t) => t.technicianId ?? '—' },
  { key: 'phoneNumber', label: 'Phone', render: (t) => t.phoneNumber ?? '—' },
  {
    key: 'isActive',
    label: 'Status',
    render: (t) => (
      <span className={`status-pill ${t.isActive ? 'status-active' : 'status-inactive'}`}>
        {t.isActive ? 'Active' : 'Inactive'}
      </span>
    ),
  },
];

const fields = [
  { name: 'FirstName', label: 'First name', required: true },
  { name: 'LastName', label: 'Last name', required: true },
  { name: 'Username', label: 'Username (email)', type: 'email', required: true, createOnly: true, hint: 'Backend validates as email' },
  { name: 'Email', label: 'Email', type: 'email', required: true },
  { name: 'Password', label: 'Password', type: 'password', required: true, createOnly: true, hint: 'Min. 8 characters' },
  { name: 'PhoneNumber', label: 'Phone number' },
  { name: 'TechnicianId', label: 'Technician ID', required: true },
];

const emptyValues = Object.fromEntries(fields.map((f) => [f.name, '']));

const buildCreateDto = (v) => ({
  FirstName: v.FirstName,
  LastName: v.LastName,
  Username: v.Username,
  Email: v.Email,
  Password: v.Password,
  PhoneNumber: v.PhoneNumber || null,
  TechnicianId: v.TechnicianId,
});

const buildUpdateDto = (v) => ({
  FirstName: v.FirstName || null,
  LastName: v.LastName || null,
  Email: v.Email || null,
  PhoneNumber: v.PhoneNumber || null,
  TechnicianId: v.TechnicianId || null,
});

export default function LabTechniciansPage() {
  return (
    <ResourcePage
      title="Lab Technicians"
      subtitle="Laboratory staff registered in the IVF API."
      itemNoun="Lab Technician"
      api={labTechniciansApi}
      columns={columns}
      fields={fields}
      emptyValues={emptyValues}
      buildCreateDto={buildCreateDto}
      buildUpdateDto={buildUpdateDto}
      searchFields={['firstName', 'lastName', 'email', 'technicianId']}
    />
  );
}
