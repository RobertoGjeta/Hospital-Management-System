import ResourcePage from '../components/common/ResourcePage';
import { doctorsApi } from '../api/doctors';

const columns = [
  {
    key: 'name',
    label: 'Name',
    render: (d) => (
      <div className="cell-stack">
        <span className="cell-strong">Dr. {d.firstName} {d.lastName}</span>
        <span className="cell-muted">{d.email}</span>
      </div>
    ),
  },
  { key: 'specialization', label: 'Specialization', render: (d) => d.specialization ?? '—' },
  { key: 'licenseNumber', label: 'License #', render: (d) => d.licenseNumber ?? '—' },
  { key: 'phoneNumber', label: 'Phone', render: (d) => d.phoneNumber ?? '—' },
  {
    key: 'isActive',
    label: 'Status',
    render: (d) => (
      <span className={`status-pill ${d.isActive ? 'status-active' : 'status-inactive'}`}>
        {d.isActive ? 'Active' : 'Inactive'}
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
  { name: 'Specialization', label: 'Specialization', required: true },
  { name: 'LicenseNumber', label: 'License number', required: true },
  { name: 'Qualifications', label: 'Qualifications', type: 'textarea', rows: 3 },
];

const emptyValues = Object.fromEntries(fields.map((f) => [f.name, '']));

const buildCreateDto = (v) => ({
  FirstName: v.FirstName,
  LastName: v.LastName,
  Username: v.Username,
  Email: v.Email,
  Password: v.Password,
  PhoneNumber: v.PhoneNumber || null,
  Specialization: v.Specialization,
  LicenseNumber: v.LicenseNumber,
  Qualifications: v.Qualifications || null,
});

const buildUpdateDto = (v) => ({
  FirstName: v.FirstName || null,
  LastName: v.LastName || null,
  Email: v.Email || null,
  PhoneNumber: v.PhoneNumber || null,
  Specialization: v.Specialization || null,
  LicenseNumber: v.LicenseNumber || null,
  Qualifications: v.Qualifications || null,
});

export default function DoctorsPage() {
  return (
    <ResourcePage
      title="Doctors"
      subtitle="Clinical staff registered in the IVF API."
      itemNoun="Doctor"
      api={doctorsApi}
      columns={columns}
      fields={fields}
      emptyValues={emptyValues}
      buildCreateDto={buildCreateDto}
      buildUpdateDto={buildUpdateDto}
      searchFields={['firstName', 'lastName', 'email', 'specialization', 'licenseNumber']}
    />
  );
}
