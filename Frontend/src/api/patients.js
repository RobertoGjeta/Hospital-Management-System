import axiosClient from './axiosClient';

const RESOURCE = '/Patient';

export const patientsApi = {
  getAll: () => axiosClient.get(RESOURCE).then((r) => r.data),
  getById: (id) => axiosClient.get(`${RESOURCE}/${id}`).then((r) => r.data),
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  update: (id, dto) => axiosClient.put(`${RESOURCE}/${id}`, dto).then((r) => r.data),
  delete: (id) => axiosClient.delete(`${RESOURCE}/${id}`).then((r) => r.data),
  search: (params) => axiosClient.get(`${RESOURCE}/search`, { params }).then((r) => r.data),
  getByDoctor: (doctorId) => axiosClient.get(`${RESOURCE}/doctor/${doctorId}`).then((r) => r.data),
  updateContact: (patientId, dto) => axiosClient.patch(`${RESOURCE}/${patientId}/contact`, dto).then((r) => r.data),
};
