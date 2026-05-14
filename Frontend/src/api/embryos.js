import axiosClient from './axiosClient';

const RESOURCE = '/Embryo';

export const embryosApi = {
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  getById: (id) => axiosClient.get(`${RESOURCE}/${id}`).then((r) => r.data),
  getByCycle: (cycleId) => axiosClient.get(`${RESOURCE}/cycle/${cycleId}`).then((r) => r.data),
  addDevelopment: (dto) => axiosClient.post(`${RESOURCE}/development`, dto).then((r) => r.data),
  getDevelopment: (embryoId) => axiosClient.get(`${RESOURCE}/${embryoId}/development`).then((r) => r.data),
  cryopreserve: (dto) => axiosClient.post(`${RESOURCE}/cryopreservation`, dto).then((r) => r.data),
  addInstruction: (dto) => axiosClient.post(`${RESOURCE}/instruction`, dto).then((r) => r.data),
};
