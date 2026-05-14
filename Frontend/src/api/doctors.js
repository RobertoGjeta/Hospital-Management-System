import axiosClient from './axiosClient';

const RESOURCE = '/Doctor';

export const doctorsApi = {
  getAll: () => axiosClient.get(RESOURCE).then((r) => r.data),
  getById: (id) => axiosClient.get(`${RESOURCE}/${id}`).then((r) => r.data),
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  update: (id, dto) => axiosClient.put(`${RESOURCE}/${id}`, dto).then((r) => r.data),
  delete: (id) => axiosClient.delete(`${RESOURCE}/${id}`).then((r) => r.data),
  getActive: () => axiosClient.get(`${RESOURCE}/active`).then((r) => r.data),
  deactivate: (id) => axiosClient.put(`${RESOURCE}/${id}/deactivate`).then((r) => r.data),
  getAvailability: (doctorId, from, to) =>
    axiosClient.get(`${RESOURCE}/${doctorId}/availability`, { params: { from, to } }).then((r) => r.data),
  setAvailability: (doctorId, slots) =>
    axiosClient.put(`${RESOURCE}/${doctorId}/availability`, slots).then((r) => r.data),
};
