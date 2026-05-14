import axiosClient from './axiosClient';

const RESOURCE = '/Appointment';

export const appointmentsApi = {
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  reschedule: (id, dto) => axiosClient.put(`${RESOURCE}/${id}/reschedule`, dto).then((r) => r.data),
  cancel: (id, dto) => axiosClient.put(`${RESOURCE}/${id}/cancel`, dto).then((r) => r.data),
  getByPatient: (patientId, status) =>
    axiosClient.get(`${RESOURCE}/patient/${patientId}`, { params: status != null ? { status } : {} }).then((r) => r.data),
  getByDoctor: (doctorId, from, to) =>
    axiosClient.get(`${RESOURCE}/doctor/${doctorId}`, { params: { from, to } }).then((r) => r.data),
};
