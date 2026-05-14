import { useEffect, useMemo, useState } from 'react';
import ResourcePage from '../components/common/ResourcePage';
import { patientsApi } from '../api/patients';
import { doctorsApi } from '../api/doctors';
import { Gender, GenderLabel, BillingType, BillingTypeLabel } from '../api/enums';

const formatDate = (iso) => (iso ? new Date(iso).toLocaleDateString() : '—');

const baseColumns = [
  {
    key: 'name',
    label: 'Name',
    render: (p) => (
      <span className="cell-stack" style={{ display: 'flex', flexDirection: 'column' }}>
        <span className="cell-strong">{p.firstName} {p.lastName}</span>
        <span className="cell-muted">{p.email}</span>
      </span>
    ),
  },
  { key: 'patientSystemId', label: 'Patient ID', render: (p) => p.patientSystemId ?? '—' },
  { key: 'nationalIdNumber', label: 'National ID', render: (p) => p.nationalIdNumber ?? '—' },
  { key: 'gender', label: 'Gender', render: (p) => GenderLabel[p.gender] ?? '—' },
  { key: 'dateOfBirth', label: 'DOB', render: (p) => formatDate(p.dateOfBirth) },
  { key: 'billingType', label: 'Billing', render: (p) => BillingTypeLabel[p.billingType] ?? '—' },
  {
    key: 'isActive',
    label: 'Status',
    render: (p) => (
      <span className={`status-pill ${p.isActive ? 'status-active' : 'status-inactive'}`}>
        {p.isActive ? 'Active' : 'Inactive'}
      </span>
    ),
  },
];

const baseFields = [
  { name: 'FirstName', label: 'First name', required: true },
  { name: 'LastName', label: 'Last name', required: true },
  { name: 'Username', label: 'Username (email)', type: 'email', required: true, createOnly: true, hint: 'Backend validates as email' },
  { name: 'Email', label: 'Email', type: 'email', required: true },
  { name: 'Password', label: 'Password', type: 'password', required: true, createOnly: true, hint: 'Min. 8 characters' },
  { name: 'PhoneNumber', label: 'Phone number' },
  { name: 'DateOfBirth', label: 'Date of birth', type: 'date', required: true, createOnly: true },
  {
    name: 'Gender',
    label: 'Gender',
    type: 'select',
    required: true,
    createOnly: true,
    options: [
      { value: Gender.Male, label: 'Male' },
      { value: Gender.Female, label: 'Female' },
      { value: Gender.Other, label: 'Other' },
    ],
  },
  { name: 'NationalIdNumber', label: 'National ID', required: true, createOnly: true },
  { name: 'Address', label: 'Address' },
  {
    name: 'BillingType',
    label: 'Billing type',
    type: 'select',
    options: [
      { value: BillingType.Insurance, label: 'Insurance' },
      { value: BillingType.SelfPay, label: 'Self-Pay' },
    ],
  },
  { name: 'InsuranceProvider', label: 'Insurance provider' },
  { name: 'InsurancePolicyNumber', label: 'Policy number' },
  { name: 'KnownAllergies', label: 'Known allergies' },
  { name: 'MedicalHistoryNotes', label: 'Medical history', type: 'textarea', rows: 3 },
];

const buildCreateDto = (v) => ({
  FirstName: v.FirstName,
  LastName: v.LastName,
  Username: v.Username,
  Email: v.Email,
  Password: v.Password,
  PhoneNumber: v.PhoneNumber || null,
  DateOfBirth: v.DateOfBirth,
  Gender: Number(v.Gender),
  NationalIdNumber: v.NationalIdNumber,
  Address: v.Address || null,
  BillingType: v.BillingType === '' ? BillingType.Insurance : Number(v.BillingType),
  InsuranceProvider: v.InsuranceProvider || null,
  InsurancePolicyNumber: v.InsurancePolicyNumber || null,
  MedicalHistoryNotes: v.MedicalHistoryNotes || null,
  KnownAllergies: v.KnownAllergies || null,
  AssignedDoctorId: v.AssignedDoctorId === '' ? null : v.AssignedDoctorId,
});

const buildUpdateDto = (v) => ({
  FirstName: v.FirstName || null,
  LastName: v.LastName || null,
  Email: v.Email || null,
  PhoneNumber: v.PhoneNumber || null,
  Address: v.Address || null,
  BillingType: v.BillingType === '' ? null : Number(v.BillingType),
  InsuranceProvider: v.InsuranceProvider || null,
  InsurancePolicyNumber: v.InsurancePolicyNumber || null,
  MedicalHistoryNotes: v.MedicalHistoryNotes || null,
  KnownAllergies: v.KnownAllergies || null,
  AssignedDoctorId: v.AssignedDoctorId === '' ? null : v.AssignedDoctorId,
});

export default function PatientsPage() {
  const [doctorOptions, setDoctorOptions] = useState([{ value: '', label: 'None' }]);

  useEffect(() => {
    doctorsApi.getActive().then((docs) => {
      setDoctorOptions([
        { value: '', label: 'None' },
        ...docs.map((d) => ({ value: d.id, label: `Dr. ${d.firstName} ${d.lastName}` })),
      ]);
    }).catch(() => {});
  }, []);

  const fields = useMemo(() => [
    ...baseFields,
    { name: 'AssignedDoctorId', label: 'Assigned doctor', type: 'select', options: doctorOptions },
  ], [doctorOptions]);

  const emptyValues = useMemo(
    () => Object.fromEntries(fields.map((f) => [f.name, ''])),
    [fields]
  );

  return (
    <ResourcePage
      title="Patients"
      subtitle="Register and manage patient records connected to the IVF API."
      itemNoun="Patient"
      api={patientsApi}
      columns={baseColumns}
      fields={fields}
      emptyValues={emptyValues}
      buildCreateDto={buildCreateDto}
      buildUpdateDto={buildUpdateDto}
      searchFields={['firstName', 'lastName', 'email', 'patientSystemId', 'nationalIdNumber']}
    />
  );
}
