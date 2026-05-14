import ResourcePage from '../components/common/ResourcePage';
import { administratorsApi } from '../api/administrators';

const columns = [
  {
    key: 'name',
    label: 'Name',
    render: (a) => (
      <div className="cell-stack">
        <span className="cell-strong">{a.firstName} {a.lastName}</span>
        <span className="cell-muted">{a.email}</span>
      </div>
    ),
  },
  { key: 'department', label: 'Department', render: (a) => a.department ?? '—' },
  { key: 'phoneNumber', label: 'Phone', render: (a) => a.phoneNumber ?? '—' },
  {
    key: 'isActive',
    label: 'Status',
    render: (a) => (
      <span className={`status-pill ${a.isActive ? 'status-active' : 'status-inactive'}`}>
        {a.isActive ? 'Active' : 'Inactive'}
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
  { name: 'Department', label: 'Department' },
];

const emptyValues = Object.fromEntries(fields.map((f) => [f.name, '']));

const buildCreateDto = (v) => ({
  FirstName: v.FirstName,
  LastName: v.LastName,
  Username: v.Username,
  Email: v.Email,
  Password: v.Password,
  PhoneNumber: v.PhoneNumber || null,
  Department: v.Department || null,
});

const buildUpdateDto = (v) => ({
  FirstName: v.FirstName || null,
  LastName: v.LastName || null,
  Email: v.Email || null,
  PhoneNumber: v.PhoneNumber || null,
  Department: v.Department || null,
});

export default function AdministratorsPage() {
  return (
    <ResourcePage
      title="Administrators"
      subtitle="Hospital administrators registered in the IVF API."
      itemNoun="Administrator"
      api={administratorsApi}
      columns={columns}
      fields={fields}
      emptyValues={emptyValues}
      buildCreateDto={buildCreateDto}
      buildUpdateDto={buildUpdateDto}
      searchFields={['firstName', 'lastName', 'email', 'department']}
    />
  );
}
