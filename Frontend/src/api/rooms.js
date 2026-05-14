import axiosClient from './axiosClient';

const RESOURCE = '/Room';

export const roomsApi = {
  getAll: () => axiosClient.get(RESOURCE).then((r) => r.data),
  getById: (id) => axiosClient.get(`${RESOURCE}/${id}`).then((r) => r.data),
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  update: (id, dto) => axiosClient.put(`${RESOURCE}/${id}`, dto).then((r) => r.data),
  delete: (id) => axiosClient.delete(`${RESOURCE}/${id}`).then((r) => r.data),
  assign: (dto) => axiosClient.post(`${RESOURCE}/assign`, dto).then((r) => r.data),
};
