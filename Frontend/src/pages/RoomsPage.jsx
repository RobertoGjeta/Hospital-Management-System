import ResourcePage from '../components/common/ResourcePage';
import { roomsApi } from '../api/rooms';

const columns = [
  { key: 'roomName', label: 'Room', render: (r) => r.roomName ?? '—' },
  { key: 'roomType', label: 'Type', render: (r) => r.roomType ?? '—' },
  { key: 'isMaintenance', label: 'Maintenance', render: (r) => (r.isMaintenance ? 'Yes' : 'No') },
];

const fields = [
  { name: 'RoomName', label: 'Room name', required: true },
  { name: 'RoomType', label: 'Room type', required: true },
  {
    name: 'IsMaintenance',
    label: 'In maintenance',
    type: 'select',
    options: [{ value: 'false', label: 'No' }, { value: 'true', label: 'Yes' }],
  },
];

const emptyValues = { RoomName: '', RoomType: '', IsMaintenance: 'false' };

const buildCreateDto = (v) => ({
  RoomName: v.RoomName,
  RoomType: v.RoomType,
  IsMaintenance: v.IsMaintenance === 'true',
});

const buildUpdateDto = (v) => ({
  RoomName: v.RoomName || null,
  RoomType: v.RoomType || null,
  IsMaintenance: v.IsMaintenance === '' ? null : v.IsMaintenance === 'true',
});

export default function RoomsPage() {
  return (
    <ResourcePage
      title="Rooms"
      subtitle="Procedure rooms and assignment targets for appointments."
      itemNoun="Room"
      api={roomsApi}
      columns={columns}
      fields={fields}
      emptyValues={emptyValues}
      buildCreateDto={buildCreateDto}
      buildUpdateDto={buildUpdateDto}
      searchFields={['roomName', 'roomType']}
    />
  );
}
