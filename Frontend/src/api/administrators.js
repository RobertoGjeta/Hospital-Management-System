import axiosClient from './axiosClient';

const RESOURCE = '/Admin';

export const administratorsApi = {
  getAll: () => axiosClient.get(RESOURCE).then((r) => r.data),
  getById: (id) => axiosClient.get(`${RESOURCE}/${id}`).then((r) => r.data),
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  update: (id, dto) => axiosClient.put(`${RESOURCE}/${id}`, dto).then((r) => r.data),
  delete: (id) => axiosClient.delete(`${RESOURCE}/${id}`).then((r) => r.data),
};
